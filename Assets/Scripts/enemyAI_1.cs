using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI_1 : MonoBehaviour, IDamage
{
    [Header("---- Unity Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform armPivot1;
    [SerializeField] Transform armPivot2;
    [SerializeField] Animator anim;

    [Header("---- Enemy Settings ----")]
    [SerializeField] int HP = 50;
    [SerializeField] int FOV = 90;
    [SerializeField] int faceTargetSpeed = 5;
    [SerializeField] int armRotateSpeed = 5;
    [SerializeField] int sprintSpeed = 6;
    [SerializeField] int roamPauseTime = 2;
    [SerializeField] int roamDistance = 10;
    [SerializeField] float flashlightSlowMultiplier = 0.2f;
    [SerializeField] float flashlightCheckDistance = 20f;

    [Header("---- Attack Settings ----")]
    [SerializeField] int attackDamage = 10;
    [SerializeField] float attackRate = 1.5f;
    [SerializeField] float attackRange = 1.5f;

    float attackTimer;
    float roamTimer;
    float angleToPlayer;
    float OGSpeed;

    Vector3 playerDir;
    Vector3 startingPos;

    bool playerInRange;
    bool canTakeDamage = true;

    Color OGcolor;

    enemyAI_4 boss;

    void Start()
    {
        OGcolor = model.material.color;
        OGSpeed = agent.speed;
        startingPos = transform.position;
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        float speed = agent.velocity.magnitude;
        if (anim != null)
            anim.SetFloat("Speed", speed / agent.speed);

        if (HitByFlashlight())
            agent.speed = OGSpeed * flashlightSlowMultiplier;
        else
            agent.speed = OGSpeed;

        if (!playerInRange)
            CheckRoam();

        float dist = Vector3.Distance(transform.position, gamemanager.instance.player.transform.position);

        if (dist <= attackRange)
            TryAttack();

        if (playerInRange)
            CanSeePlayer();
    }

    void TryAttack()
    {
        if (attackTimer >= attackRate)
        {
            gamemanager.instance.playerScript.TakeDamage(attackDamage);
            attackTimer = 0;
        }
    }

    void CheckRoam()
    {
        if (agent.remainingDistance < 1f)
        {
            roamTimer += Time.deltaTime;
            if (roamTimer >= roamPauseTime)
                Roam();
        }
    }

    void Roam()
    {
        roamTimer = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDistance + startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDistance, 1);

        agent.SetDestination(hit.position);
    }

    bool CanSeePlayer()
    {
        playerDir = gamemanager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                agent.SetDestination(gamemanager.instance.player.transform.position);

                if (!HitByFlashlight())
                    agent.speed = sprintSpeed;

                FaceTarget();
                ArmRotate();
                return true;
            }
        }

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
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    //Replaced old takedmg with new one that works better..
    public void TakeDamage(int damage)
    {
        HP -= damage;

        if (HP <= 0)
        {
            if (boss != null)
            {
                boss.OnEnemyKilled();
            }
            else if (waveManager.instance != null)
            {
                waveManager.instance.enemyKilled();
            }
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    public void SetBoss(enemyAI_4 bossRef)
    {
        boss = bossRef;
    }

    IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(0.05f);
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
            return hit.collider.GetComponentInParent<enemyAI_1>() == this;
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