using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI_2 : MonoBehaviour, IDamage
{
    [Header("---- Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform armPivot1;
    [SerializeField] Transform armPivot2;
    [SerializeField] Animator anim;

    [Header("---- Stats ----")]
    [SerializeField] int HP = 100;
    [SerializeField] int FOV = 120;
    [SerializeField] int faceTargetSpeed = 6;
    [SerializeField] int armRotateSpeed = 6;
    [SerializeField] int sprintSpeed = 8;

    [Header("---- Attack ----")]
    [SerializeField] int attackDamage = 20;
    [SerializeField] float attackRate = 1f;
    [SerializeField] float attackRange = 2f;

    float attackTimer;
    Color OGcolor;
    float OGSpeed;

    Vector3 playerDir;

    void Start()
    {
        anim.Play("ZombieAnimator");

        OGcolor = model.material.color;
        OGSpeed = agent.speed;

        // Auto assign animator if not set
        if (anim == null)
            anim = GetComponent<Animator>();
    }

    void Update()
    {
        Debug.Log("Animator: " + anim);

        attackTimer += Time.deltaTime;

        agent.speed = sprintSpeed;

        if (CanSeePlayer())
            agent.SetDestination(gamemanager.instance.player.transform.position);

        float dist = Vector3.Distance(transform.position, gamemanager.instance.player.transform.position);

        if (dist <= attackRange)
            TryAttack();

        // should fix animation..(matches enemyAI_1 behavior)
        if (agent != null && anim != null && agent.speed > 0)
        {
            float speed = agent.velocity.magnitude;
            anim.SetFloat("Speed", speed / agent.speed);
        }
    }

    void TryAttack()
    {
        if (attackTimer >= attackRate)
        {
            gamemanager.instance.playerScript.TakeDamage(attackDamage);
            attackTimer = 0;
        }
    }

    bool CanSeePlayer()
    {
        playerDir = gamemanager.instance.player.transform.position - transform.position;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player"))
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
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(playerDir),
            Time.deltaTime * faceTargetSpeed);
    }

    void ArmRotate()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        armPivot1.rotation = Quaternion.Lerp(armPivot1.rotation, rot, Time.deltaTime * armRotateSpeed);
        armPivot2.rotation = Quaternion.Lerp(armPivot2.rotation, rot, Time.deltaTime * armRotateSpeed);
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;

        if (HP <= 0)
        {
            waveManager.instance.enemyKilled();
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