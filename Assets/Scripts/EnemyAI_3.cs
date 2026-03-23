using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI_3 : MonoBehaviour //IDamage
{
    [Header("---- Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [Header("---- Stats ----")]
    [SerializeField] int HP = 300;
    [SerializeField] int FOV = 180;
    [SerializeField] int faceTargetSpeed = 5;

    [Header("---- Attack Stats ----")]
    [SerializeField] float chargeDelay = 2f;
    [SerializeField] float chargeSpeed = 15f;
    [SerializeField] int chargeDistance; // if player is farther then this distance then the boss will charge
    [SerializeField] int spawnDistance; // if player is within this distance then will spawn zombies around boss
    [SerializeField] GameObject[] spawns;
    [SerializeField] int spawnAmount;
    [SerializeField] float spawnCoolDownTime;
    [SerializeField] GameObject shockwave;
    [SerializeField] float shockwaveRate;
    [SerializeField] Transform shockwavePos;
    [SerializeField] int attackDamage = 30;
    [SerializeField] float attackRate = 1f;
    [SerializeField] float attackRange = 2.5f;

    float OGSpeed;
    float attackTimer;
    float chargeTimer;
    float playerDis;
    float spawnCoolDownTimer;
    float shockwaveTimer;

    int spawnCount;

    Vector3 playerDir;
    Color OGcolor;

    bool isDead;
    bool hasSpawned;
    bool isCharging;

    void Start()
    {
        OGcolor = model.material.color;
        OGSpeed = agent.speed;
    }

    void Update()
    {
        Vector3 playerPos = gamemanager.instance.player.transform.position;
        playerDis = Vector3.Distance(transform.position, playerPos);

        attackTimer += Time.deltaTime;
        playerDir = playerPos - transform.position;
        if (playerDis <= attackRange)
            TryAttack();

        FaceTarget();

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
        if(!isCharging)
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
            waveManager.instance.Spawn(spawns[Random.Range(0, spawns.Length)]);
            spawnCount++;
            waveManager.instance.enemiesAlive++;
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

    void FaceTarget()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(playerDir), Time.deltaTime * faceTargetSpeed);
    }

    void TryAttack()
    {
        if (attackTimer >= attackRate)
        {
            gamemanager.instance.playerScript.TakeDamage(attackDamage);
            attackTimer = 0;
        }
    }

    //public void TakeDamage(int damage)
    //{
    //    if (isDead) return;

    //    HP -= damage;

    //    if (HP <= 0)
    //    {
    //        isDead = true;
    //        waveManager.instance.enemyKilled();
    //        Destroy(gameObject);
    //    }
    //    else
    //    {
    //        StartCoroutine(FlashRed());
    //    }
    //}

    //IEnumerator FlashRed()
    //{
    //    model.material.color = Color.red;
    //    yield return new WaitForSeconds(0.1f);
    //    model.material.color = OGcolor;
    //}
}