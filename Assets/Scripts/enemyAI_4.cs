using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class enemyAI_4 : MonoBehaviour, IDamage
{

    [Header("---- Unity Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Transform armPivot1;
    [SerializeField] Transform armPivot2;

    //Doesn't need sprint or roaming information 
    [Header("---- Enemy Settings ----")]
    [SerializeField] int HP;
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int armRotateSpeed;
    [SerializeField] float dragPauseTime;   // tracks to see how long they need to see player before dragging them to itself
    [SerializeField] float distanceFromPlayer;
    [SerializeField] float flashlightSlowMultiplier; // How much the enemy's speed is reduced when in the player's flashlight
    [SerializeField] float flashlightCheckDistance; // The distance at which the enemy checks if it's in the player's flashlight

    Color OGColor;

    bool testBool;

    float angleToPlayer;
    float seeingPlayerTimer;

    Vector3 playerDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>(); // Try to get the NavMeshAgent component if not assigned in the inspector

        if (model == null)
            model = GetComponentInChildren<Renderer>(); // Try to get the Renderer component from children if not assigned in the inspector

        OGColor = model.material.color;

    }

    // Update is called once per frame
    void Update()
    {
        testBool = CanSeePlayer();
    }

    void DragMechanic()
    {
        
    }

    bool CanSeePlayer()
    {
        playerDir = gamemanager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(transform.position, playerDir, Color.purple);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    //FaceTarget();
                }
                //ArmRotate();

                return true;
            }
        }

        return false;
    }

    public void TakeDamage(int damage)
    {
       
        HP -= damage;

        if (HP <= 0)
        {
            gamemanager.instance.EnemyKilled();
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
        model.material.color = OGColor;
    }
}
