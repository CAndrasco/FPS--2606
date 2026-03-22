using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI_4 : MonoBehaviour, IDamage
{
    [Header("---- Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [Header("---- Stats ----")]
    [SerializeField] int HP = 500;
    [SerializeField] int faceTargetSpeed = 6;

    [Header("---- Movement ----")]
    [SerializeField] float normalSpeed = 5f;
    [SerializeField] float chargeSpeed = 12f;
    [SerializeField] float chargeDelay = 3f;

    [Header("---- Attack ----")]
    [SerializeField] int attackDamage = 30;
    [SerializeField] float attackRate = 1f;
    [SerializeField] float attackRange = 2.5f;

    float attackTimer;
    float chargeTimer;

    Vector3 playerDir;
    Color OGcolor;

    bool isDead;

    void Start()
    {
        OGcolor = model.material.color;

        if (agent != null)
            agent.speed = normalSpeed;
    }

    void Update()
    {
        if (isDead) return;

        attackTimer += Time.deltaTime;
        chargeTimer += Time.deltaTime;

        Vector3 playerPos = gamemanager.instance.player.transform.position;

        // Always chase player
        agent.SetDestination(playerPos);

        playerDir = playerPos - transform.position;
        FaceTarget();

        float dist = Vector3.Distance(transform.position, playerPos);

        if (dist <= attackRange)
            TryAttack();

        // Charge burst
        if (chargeTimer >= chargeDelay)
        {
            StartCoroutine(Charge());
            chargeTimer = 0;
        }
    }

    IEnumerator Charge()
    {
        agent.speed = chargeSpeed;

        yield return new WaitForSeconds(1.5f);

        agent.speed = normalSpeed;
    }

    void TryAttack()
    {
        if (attackTimer >= attackRate)
        {
            gamemanager.instance.playerScript.TakeDamage(attackDamage);
            attackTimer = 0;
        }
    }

    void FaceTarget()
    {
        if (playerDir == Vector3.zero) return;

        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            rot,
            Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        HP -= damage;

        if (HP <= 0)
        {
            isDead = true;

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