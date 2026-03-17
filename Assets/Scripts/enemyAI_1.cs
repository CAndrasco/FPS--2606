using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.GameCenter;

public class enemyAI_1 : MonoBehaviour, IDamage
{
    [Header("---- Unity Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Transform armPivot1;
    [SerializeField] Transform armPivot2;
    [SerializeField] LayerMask flashlightCheckIgnore;

    [Header("---- Enemy Settings ----")]
    [SerializeField] int HP;
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int armRotateSpeed;
    [SerializeField] int sprintSpeed;
    [SerializeField] int roamPauseTime;
    [SerializeField] int roamDistance;
    [SerializeField] float flashlightSlowMultiplier; // How much the enemy's speed is reduced when in the player's flashlight
    [SerializeField] float flashlightCheckDistance; // The distance at which the enemy checks if it's in the player's flashlight
    //[SerializeField] int cameraFOV;

    Color OGcolor;

    bool playerInRange;
    //bool isSlowed = false;
    //bool isDead = false;

    float roamTimer;
    float angleToPlayer;
    float stoppingDistanceOG;
    float OGSpeed;

    Vector3 playerDir;
    Vector3 startingPos;


    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>(); // Try to get the NavMeshAgent component if not assigned in the inspector

        if(model == null)
            model = GetComponentInChildren<Renderer>(); // Try to get the Renderer component from children if not assigned in the inspector

        OGcolor = model.material.color;
        OGSpeed = agent.speed;
        startingPos = transform.position;
        stoppingDistanceOG = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (HitByFlashlight()) // Check if the enemy is currently being hit by the player's flashlight
        {
            agent.speed = OGSpeed * flashlightSlowMultiplier; // Reduce speed when hit by flashlight
        }
        else
        {
            agent.speed = OGSpeed; // Reset to normal speed when not hit by flashlight
        }

        if (agent == null) 
            return; // If agent is still null, exit the Update method to avoid errors

        if (agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;
        }
        if(playerInRange && !CanSeePlayer())
        {
            CheckRoam();
        }
        else if(!playerInRange)
        {
            CheckRoam();
        }
    }

    void CheckRoam()
    {
        if(agent.remainingDistance < 0.01f && roamTimer >= roamPauseTime)
        {
            Roam();
        }
    }

    void Roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDistance;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDistance, 1);
        agent.SetDestination(hit.position);
    }

    bool CanSeePlayer()
    {
        playerDir = gamemanager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(transform.position, playerDir);

        RaycastHit hit;
        if(Physics.Raycast(transform.position,playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                agent.SetDestination(gamemanager.instance.player.transform.position);
                if (!HitByFlashlight())
                {
                    agent.speed = sprintSpeed; 
                }
                if(agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }
                ArmRotate();
                agent.stoppingDistance = stoppingDistanceOG;
                return true;
            }
        }
        agent.stoppingDistance = 0;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    public void TakeDamage(int damage)
    {
        //if (isDead) return;

        HP -= damage;

        if (HP <= 0)
        {
            //isDead = true;
            gamemanager.instance.EnemyKilled();
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    bool HitByFlashlight() // This method checks if the enemy is currently being hit by the player's flashlight
    {
        RaycastHit hit;
        //Vector3 center = transform.position;

        //Collider[] hitCollider = Physics.OverlapSphere(center, flashlightCheckDistance);
        //playerDir = gamemanager.instance.player.transform.position;
        //angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        //Debug.DrawRay(playerDir, gamemanager.instance.player.transform.position, Color.purple);

        //for (int i = 0; i < hitCollider.Length; i++)
        //{
        //    if (hitCollider[i].CompareTag("Player"))
        //    {
        //        isSlowed = true;
        //    }
        //    else
        //    {
        //        isSlowed = false;
        //    }
        //}

        if (Physics.Raycast(gamemanager.instance.player.transform.position, gamemanager.instance.player.transform.forward, out hit,
            flashlightCheckDistance))
        {
            //return true;
            if (hit.collider.GetComponentInParent<enemyAI_1>() == this)
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = OGcolor;
    }

}
