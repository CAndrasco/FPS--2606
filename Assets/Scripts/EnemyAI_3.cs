using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI_3 : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP = 300;
    [SerializeField] int FOV = 180;
    [SerializeField] int faceTargetSpeed = 5;

    [SerializeField] float chargeDelay = 2f;
    [SerializeField] float chargeSpeed = 15f;

    float chargeTimer;
    Vector3 playerDir;
    Color OGcolor;

    bool isDead;

    void Start()
    {
        OGcolor = model.material.color;
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            chargeTimer += Time.deltaTime;

            if (chargeTimer >= chargeDelay)
            {
                agent.speed = chargeSpeed;
                agent.SetDestination(gamemanager.instance.player.transform.position);
                chargeTimer = 0;
            }
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