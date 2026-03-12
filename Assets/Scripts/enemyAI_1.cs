using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI_1 : MonoBehaviour, IDamage
{
    [Header("---- Unity Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] LayerMask ignoreLayer;
    //[SerializeField] Transform armHittingPos1;
    //[SerializeField] Transform armHittingPos2;
    [SerializeField] Transform armPivot1;
    [SerializeField] Transform armPivot2;

    [Header("---- Enemy Settings ----")]
    [SerializeField] int HP;
    //[SerializeField] GameObject Weapon;
    //[SerializeField] float damageRate;
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int armRotateSpeed;
    [SerializeField] int sprintSpeed;
    [SerializeField] int roamPauseTime;
    [SerializeField] int roamDistance;
    

    Color colorOrig;

    bool playerInRange;

    float roamTimer;
    float angleToPlayer;
    float stoppingDistanceOG;

    Vector3 playerDir;
    Vector3 startingPos;


    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>(); // Try to get the NavMeshAgent component if not assigned in the inspector

        if(model == null)
            model = GetComponentInChildren<Renderer>(); // Try to get the Renderer component from children if not assigned in the inspector

        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1);
        startingPos = transform.position;
        stoppingDistanceOG = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
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
        HP -= damage;
        if(HP <= 0)
        {
            //gamemanager.instance.updateGameGoal(-1);
            gamemanager.instance.EnemyKilled();
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    //just to check enemies are taking damage wont be needed in final build
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    void Die()
    {
        gamemanager.instance.EnemyKilled();
        Destroy(gameObject);
    }

}
