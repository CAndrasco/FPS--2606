using UnityEngine;

public class exitDoor : MonoBehaviour
{
    bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            gamemanager.instance.updateGameGoal(-1);
        }
    }
}
