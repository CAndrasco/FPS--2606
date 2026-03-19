using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.GameCenter;

public class enemyAI_1 : MonoBehaviour, IDamage
{
    [Header("---- Unity Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Transform armPivot1;
    [SerializeField] Transform armPivot2;
    [SerializeField] LayerMask flashlightCheckIgnore;

    [Header("---- Enemy Settings ----")]
    [SerializeField] int HP;
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int armRotateSpeed;
    [SerializeField] int sprintSpeed;
    [SerializeField] int roamPauseTime;
    [SerializeField] int roamDistance;
    [SerializeField] float flashlightSlowMultiplier = 0.2f;
    [SerializeField] float flashlightCheckDistance = 20f;
    [SerializeField] int cameraFOV;

    [Header("---- Attack Settings ----")]
    [SerializeField] int attackDamage = 10;
    [SerializeField] float attackRate = 1.5f;
    [SerializeField] float attackRange = 1.5f;

    float attackTimer;

    Color OGcolor;

    bool playerInRange;
    bool isDead = false;

    float roamTimer;
    float angleToPlayer;
    float stoppingDistanceOG;
    float OGSpeed;

    Vector3 playerDir;
    Vector3 startingPos;

    // DAMAGE FIX
    bool canTakeDamage = true;
    float damageCooldown = 0.05f;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (model == null)
            model = GetComponentInChildren<Renderer>();

        OGcolor = model.material.color;
        OGSpeed = agent.speed;
        startingPos = transform.position;
        stoppingDistanceOG = agent.stoppingDistance;
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        if (HitByFlashlight())
        {
            agent.speed = OGSpeed * flashlightSlowMultiplier;
        }
        else
        {
            agent.speed = OGSpeed;
        }

        if (agent == null)
            return;

        if (agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;
        }

        if (!playerInRange)
        {
            CheckRoam();
        }

        float distanceToPlayer = Vector3.Distance(transform.position, gamemanager.instance.player.transform.position);

        if (distanceToPlayer <= attackRange)
        {
            TryAttack();
        }

        if (playerInRange)
        {
            CanSeePlayer();
        }
    }

    void TryAttack()
    {
        if (attackTimer >= attackRate)
        {
            gamemanager.instance.playerScript.TakeDamage(attackDamage);
            attackTimer = 0f;
        }
    }

    void CheckRoam()
    {
        if (agent.remainingDistance < 0.01f && roamTimer >= roamPauseTime)
        {
            Roam();
        }
    }

    void Roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDistance;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDistance, 1);
        agent.SetDestination(hit.position);
    }

    bool CanSeePlayer()
    {
        playerDir = gamemanager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(transform.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                agent.SetDestination(gamemanager.instance.player.transform.position);

                if (!HitByFlashlight())
                {
                    agent.speed = sprintSpeed;
                }

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }

                ArmRotate();
                agent.stoppingDistance = stoppingDistanceOG;
                return true;
            }
        }

        agent.stoppingDistance = 0;
        return false;
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void ArmRotate()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        armPivot1.rotation = Quaternion.Lerp(armPivot1.rotation, rot, Time.deltaTime * armRotateSpeed);
        armPivot2.rotation = Quaternion.Lerp(armPivot2.rotation, rot, Time.deltaTime * armRotateSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!canTakeDamage)
            return;

        canTakeDamage = false;

        HP -= damage;

        if (HP <= 0)
        {
            waveManager.instance.EnemyKilled();
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FlashRed());
        }

        StartCoroutine(DamageCooldown());
    }

    IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }

    bool HitByFlashlight()
    {
        RaycastHit hit;

        if (Physics.Raycast(
            gamemanager.instance.player.transform.position,
            gamemanager.instance.player.transform.forward,
            out hit,
            flashlightCheckDistance))
        {
            if (hit.collider.GetComponentInParent<enemyAI_1>() == this)
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = OGcolor;
    }
}