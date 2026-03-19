using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI_2 : MonoBehaviour, IDamage
{
    [Header("---- Unity Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform armPivot1;
    [SerializeField] Transform armPivot2;

    [Header("---- Enemy Settings ----")]
    [SerializeField] int HP;
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int armRotateSpeed;
    [SerializeField] int sprintSpeed;
    [SerializeField] float flashlightSlowMultiplier = 0.2f;
    [SerializeField] float flashlightCheckDistance = 20f;

    [Header("---- Attack Settings ----")]
    [SerializeField] int attackDamage = 20;
    [SerializeField] float attackRate = 1.0f;
    [SerializeField] float attackRange = 2f;

    float attackTimer;

    Color OGcolor;

    bool isDead = false;

    float angleToPlayer;
    float OGSpeed;

    Vector3 playerDir;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (model == null)
            model = GetComponentInChildren<Renderer>();

        OGcolor = model.material.color;
        OGSpeed = agent.speed;
    }

    void Update()
    {
        if (agent == null) return;

        attackTimer += Time.deltaTime;

        if (HitByFlashlight())
        {
            agent.speed = OGSpeed * flashlightSlowMultiplier;
        }
        else
        {
            agent.speed = sprintSpeed;
        }

        if (CanSeePlayer())
        {
            agent.SetDestination(gamemanager.instance.player.transform.position);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, gamemanager.instance.player.transform.position);

        if (distanceToPlayer <= attackRange)
        {
            TryAttack();
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

    bool HitByFlashlight()
    {
        RaycastHit hit;

        if (Physics.Raycast(
            gamemanager.instance.player.transform.position,
            gamemanager.instance.player.transform.forward,
            out hit,
            flashlightCheckDistance))
        {
            if (hit.collider.GetComponentInParent<enemyAI_2>() == this)
            {
                return true;
            }
        }

        return false;
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

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        HP -= damage;

        if (HP <= 0)
        {
            isDead = true;
            waveManager.instance.EnemyKilled();
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = OGcolor;
    }
}