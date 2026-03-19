using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEditor;
using Unity.Hierarchy;

public class enemyAI_3 : MonoBehaviour, IDamage
{
    [Header("---- Unity Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Transform armPivot1;
    [SerializeField] Transform armPivot2;

    [Header("---- Enemy Settings ----")]
    [SerializeField] int HP;
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int armRotateSpeed;
    [SerializeField] int sprintSpeed;
    [SerializeField] int roamPauseTime;
    [SerializeField] int roamDistance;
    [SerializeField] float flashlightSlowMultiplier = 0.2f; // How much the enemy's speed is reduced when in the player's flashlight
    [SerializeField] float flashlightCheckDistance = 20f; // The distance at which the enemy checks if it's in the player's flashlight

    [Header("---- Enemy Attack Settings ----")]
    [SerializeField] float waitChargeTimer;
    [SerializeField] float chargingSprintSpeed;
    [SerializeField] float chargingDistance;

    Color OGColor;

    bool isDead = false;
    bool hasSeenPlayer = false;

    float chargingTimer;
    float angleToPlayer;
    float OGSpeed;

    Vector3 playerDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>(); // Try to get the NavMeshAgent component if not assigned in the inspector

        if (model == null)
            model = GetComponentInChildren<Renderer>(); // Try to get the Renderer component from children if not assigned in the inspector

        OGColor = model.material.color;
        OGSpeed = agent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        //agent.SetDestination(gamemanager.instance.player.transform.position);

        if(CanSeePlayer())
        {
            chargingTimer += Time.deltaTime;
            if (chargingTimer >= waitChargeTimer)
            {
                ChargingPlayer(); 
            }
        }
        else
        {
            agent.speed = OGSpeed;
        }
        if (agent == null)
            return; // If agent is still null, exit the Update method to avoid errors

        //if (HitByFlashlight())
        //{
        //    agent.speed = OGSpeed * flashlightSlowMultiplier;
        //}
        //else if (!HitByFlashlight() && CanSeePlayer())
        //{
        //    agent.speed = sprintSpeed;
        //}
        //else
        //{
        //    agent.speed = OGSpeed;
        //}

    }

    bool HitByFlashlight() // This method checks if the enemy is currently being hit by the player's flashlight
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, flashlightCheckDistance))
        {
            if (hit.collider.GetComponentInParent<enemyAI_3>() == this)
            {
                return true;
            }
        }
        return false;
    }

    void ChargingPlayer() // charges player's location from before timer starts
    {
        chargingTimer = 0;
        playerDir = gamemanager.instance.player.transform.position;
        agent.speed = chargingSprintSpeed;
        agent.SetDestination(playerDir);
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
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }
                ArmRotate();
                hasSeenPlayer = true;
                return true;
            }
        }

        return false;
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = OGColor;
    }

    IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);
    }

}
