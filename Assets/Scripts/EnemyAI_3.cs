using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI_3 : MonoBehaviour
{
    [Header("---- Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    [Header("---- Audio ----")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] bossSounds;
    [SerializeField] float soundRate = 2f;

    float soundTimer;

    [Header("---- Stats ----")]
    [SerializeField] int HP = 300;
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

    bool isCharging;
    bool hasSpawned;

    void Start()
    {
        OGcolor = model.material.color;
        OGSpeed = agent.speed;

        // make sure audio source exists
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // grab animator (handles child models too)
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Debug.Log("Animator object: " + anim.gameObject.name);
        Debug.Log("Current state: " + anim.GetCurrentAnimatorStateInfo(0).shortNameHash);
        Debug.Log("Speed param: " + anim.GetFloat("Speed"));

        Vector3 playerPos = gamemanager.instance.player.transform.position;
        playerDir = playerPos - transform.position;
        playerDis = Vector3.Distance(transform.position, playerPos);

        attackTimer += Time.deltaTime;

        // always move toward player unless charging
        if (!isCharging)
        {
            agent.speed = OGSpeed;
            agent.SetDestination(playerPos);
        }

        FaceTarget();

        // attack if close
        if (playerDis <= attackRange)
            TryAttack();

        // spawn enemies if player is close
        if (playerDis <= spawnDistance && spawnCount < spawnAmount && !hasSpawned)
        {
            Spawning();
        }
        else
        {
            spawnCoolDownTimer += Time.deltaTime;
        }

        if (spawnCoolDownTimer >= spawnCoolDownTime)
            hasSpawned = false;

        // shockwave if mid distance
        shockwaveTimer += Time.deltaTime;
        if (playerDis > spawnDistance && playerDis < chargeDistance && shockwaveTimer >= shockwaveRate)
        {
            ShockWave();
        }

        // charge if far away
        if (!isCharging && playerDis >= chargeDistance)
        {
            chargeTimer += Time.deltaTime;

            if (chargeTimer >= chargeDelay)
            {
                Charge();
            }
        }

        // animate try number 5?
        if (agent != null && anim != null)
        {
            float speed = agent.velocity.magnitude;
            anim.SetFloat("Speed", speed);
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
        if (bossSounds == null || bossSounds.Length == 0 || audioSource == null) return;

        int rand = Random.Range(0, bossSounds.Length);

        if (bossSounds[rand] != null)
            audioSource.PlayOneShot(bossSounds[rand], 5f);
    }

    void Charge()
    {
        isCharging = true;
        agent.speed = chargeSpeed;
        agent.SetDestination(gamemanager.instance.player.transform.position);

        chargeTimer = 0;
        StartCoroutine(chargeTiming());
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
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(playerDir),
            Time.deltaTime * faceTargetSpeed);
    }
}