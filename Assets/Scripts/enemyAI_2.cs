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

    [Header("---- Audio ----")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] zombieSounds;
    [SerializeField] float soundRate = 3f;

    float soundTimer;

    [Header("---- Stats ----")]
    [SerializeField] int HP = 100;
    [SerializeField] int faceTargetSpeed = 6;
    [SerializeField] int armRotateSpeed = 6;
    [SerializeField] int sprintSpeed = 8;

    [Header("---- Attack ----")]
    [SerializeField] int attackDamage = 20;
    [SerializeField] float attackRate = 1f;
    [SerializeField] float attackRange = 2f;

    float attackTimer;
    Color OGcolor;

    Vector3 playerDir;

    enemyAI_4 boss;

    void Start()
    {
        OGcolor = model.material.color;

        if (anim == null)
            anim = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        Vector3 playerPos = gamemanager.instance.player.transform.position;
        playerDir = playerPos - transform.position;

        // Always chase player
        agent.speed = sprintSpeed;
        agent.SetDestination(playerPos);

        // face player
        FaceTarget();
        ArmRotate();

        // attack
        float dist = Vector3.Distance(transform.position, playerPos);
        if (dist <= attackRange)
            TryAttack();

        // animation based on movement
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

        float dist = Vector3.Distance(transform.position, gamemanager.instance.player.transform.position);

        if (soundTimer >= soundRate && dist < 25f)
        {
            PlayRandomSound();
            soundTimer = 0;
        }
    }

    void PlayRandomSound()
    {
        if (zombieSounds == null || zombieSounds.Length == 0 || audioSource == null) return;

        int rand = Random.Range(0, zombieSounds.Length);

        if (zombieSounds[rand] != null)
        {
            // loudness =5
            audioSource.PlayOneShot(zombieSounds[rand], 9f);
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

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = OGcolor;
    }
}