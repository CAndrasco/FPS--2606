using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI_3 : MonoBehaviour
{
    [Header("---- Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [Header("---- Audio ----")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] bossSounds;
    [SerializeField] float soundRate = 2f;

    float soundTimer;

    [Header("---- Stats ----")]
    [SerializeField] int HP = 300;
    [SerializeField] int FOV = 180;
    [SerializeField] int faceTargetSpeed = 5;

    [Header("---- Attack Stats ----")]
    [SerializeField] float chargeDelay = 2f;
    [SerializeField] float chargeSpeed = 15f;
    [SerializeField] int chargeDistance;
    [SerializeField] int spawnDistance;
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

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
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

        if (playerDis <= spawnDistance && spawnCount < spawnAmount && !hasSpawned)
        {
            Spawning();
        }
        else
        {
            spawnCoolDownTimer += Time.deltaTime;
        }

        if (spawnCoolDownTimer >= spawnCoolDownTime)
        {
            hasSpawned = false;
        }

        if (playerDis > spawnDistance && playerDis < chargeDistance && shockwaveTimer >= shockwaveRate)
        {
            ShockWave();
        }

        if (!isCharging && playerDis >= chargeDistance)
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

        HandleSound();
    }

    void HandleSound()
    {
        soundTimer += Time.deltaTime;

        if (soundTimer >= soundRate && playerDis < 25f)
        {
            PlayRandomSound();
            soundTimer = 0;
        }
    }

    void PlayRandomSound()
    {
        if (bossSounds.Length == 0 || audioSource == null) return;

        int rand = Random.Range(0, bossSounds.Length);
        audioSource.PlayOneShot(bossSounds[rand]);
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
}