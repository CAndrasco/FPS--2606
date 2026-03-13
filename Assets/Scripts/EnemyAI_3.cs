using UnityEngine;
using UnityEngine.AI;

public class EnemyAI_3 : MonoBehaviour
{
    [Header("---- Unity Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Transform armPivot1;
    [SerializeField] Transform armPivot2;

    [Header("---- Enemy Settings ----")]
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int armRotateSpeed;
    [SerializeField] float flashlightSlowMultiplier = 0.2f; // How much the enemy's speed is reduced when in the player's flashlight
    [SerializeField] float flashlightCheckDistance = 20f; // The distance at which the enemy checks if it's in the player's flashlight

    float OGspeed;
    float angleToPlayer;

    Vector3 playerDir;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>(); // Try to get the NavMeshAgent component if not assigned in the inspector

        if (model == null)
            model = GetComponentInChildren<Renderer>(); // Try to get the Renderer component from children if not assigned in the inspector

        OGspeed = agent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent == null)
            return; // If agent is still null, exit the Update method to avoid errors

        agent.SetDestination(gamemanager.instance.player.transform.position);
        playerDir = gamemanager.instance.player.transform.position - transform.position;

        FaceTarget();
        ArmRotate();

        if(HitByFlashlight())
        {
            agent.speed = OGspeed * flashlightSlowMultiplier;
        }
        else
        {
            agent.speed = OGspeed;
        }
    }

    bool HitByFlashlight() // This method checks if the enemy is currently being hit by the player's flashlight
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, flashlightCheckDistance))
        {
            if (hit.collider.GetComponentInParent<EnemyAI_3>() == this)
            {
                return true;
            }
        }
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
}
