using UnityEngine;

public class exitDoor : MonoBehaviour
{
    bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;

            gamemanager.instance.youWin(); // Call the youWin() method from the gamemanager instance to trigger the win condition when the player enters the exit door.

        }
    }
}
