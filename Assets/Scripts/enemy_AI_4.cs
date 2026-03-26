using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.Burst.CompilerServices;

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

    [Header("---- Attack Stats ----")]
    [SerializeField] int chargeDistance; // if player is farther then this distance then the boss will charge
    [SerializeField] int spawnDistance; // if player is within this distance then will spawn zombies around boss
    [SerializeField] GameObject[] spawns;
    [SerializeField] int spawnAmount;
    [SerializeField] float spawnCoolDownTime;
    [SerializeField] GameObject shockwave;
    [SerializeField] float shockwaveRate;
    [SerializeField] Transform shockwavePos;

    float OGSpeed;
    float attackTimer;
    float chargeTimer;
    float playerDis;
    float shockwaveTimer;
    float spawnCoolDownTimer;

    int spawnCount;
    int enemiesAlive;

    Vector3 playerDir;
    Color OGcolor;

    bool isDead;
    bool hasSpawned;
    bool isCharging;

    void Start()
    {
        OGcolor = model.material.color;

        OGSpeed = agent.speed;

        if (agent != null)
            agent.speed = normalSpeed;
    }

    void Update()
    {
        if (isDead) return;

        attackTimer += Time.deltaTime;
        chargeTimer += Time.deltaTime;

        Vector3 playerPos = gamemanager.instance.player.transform.position;


        playerDir = playerPos - transform.position;
        FaceTarget();

        playerDis = Vector3.Distance(transform.position, playerPos);

        if (playerDis <= attackRange)
            TryAttack();

        shockwaveTimer += Time.deltaTime;

        if (!CanSeePlayer() && playerDis <= chargeDistance)
        {
            agent.SetDestination(playerPos);
        }


        if (playerDis <= spawnDistance && spawnCount < spawnAmount && !hasSpawned) // player is close to boss
        {
            Spawning();
        }
        else
        {
            spawnCoolDownTimer += Time.deltaTime;
        }

        if (spawnCoolDownTimer >= spawnCoolDownTime) // resets spawning so boss can spawn more
        {
            hasSpawned = false;
        }

        if (playerDis > spawnDistance && playerDis < chargeDistance && shockwaveTimer >= shockwaveRate) // player is in the middle ground
        {
            ShockWave();
        }


        if (!isCharging && playerDis >= chargeDistance) // player is far from the boss
        {
            chargeTimer += Time.deltaTime;

            if (chargeTimer >= chargeDelay)
            {
                Charge();
            }
        }
        if (!isCharging)
        {
            agent.speed = OGSpeed;
        }

    }

    void Charge()
    {
        isCharging = true;
        agent.speed = chargeSpeed;
        agent.SetDestination(gamemanager.instance.player.transform.position);
        StartCoroutine(chargeTiming());
        chargeTimer = 0;
    }

    IEnumerator chargeTiming()
    {
        yield return new WaitForSeconds(chargeDelay);
        isCharging = false;
    }
    void Spawning()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            if (SpawnEnemy(spawns[Random.Range(0, spawns.Length)]))
            {
                spawnCount++;
            }
        }

        hasSpawned = true;
        spawnCoolDownTimer = 0;
    }

    void ShockWave()
    {
        shockwaveTimer = 0;
        Instantiate(shockwave, shockwavePos.position, transform.rotation);
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
                return true;
            }
        }

        return false;
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

    bool SpawnEnemy(GameObject enemy)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 ranPos = Random.insideUnitSphere * 10f;
            ranPos += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(ranPos, out hit, 10f, NavMesh.AllAreas))
            {
                GameObject newEnemy = Instantiate(enemy, hit.position, Quaternion.identity);

                enemyAI_1 ai1 = newEnemy.GetComponent<enemyAI_1>();
                if (ai1 != null)
                {
                    ai1.SetBoss(this);
                }

                enemyAI_2 ai2 = newEnemy.GetComponent<enemyAI_2>();
                if (ai2 != null)
                {
                    ai2.SetBoss(this);
                }

                enemiesAlive++;
                return true;
            }
        }

        return false;
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
    }
}