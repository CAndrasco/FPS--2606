using UnityEngine;
//If you see this you're safe to work. 
public class ammoPickup : MonoBehaviour
{
    [SerializeField] int ammoAmount = 8;

    bool pickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;
        if (!other.CompareTag("Player")) return;

        playerController player = other.GetComponent<playerController>(); // Get the playerController component from the player object.

        if (player == null) // If the playerController component is not found, log a warning and exit the method.
        {
            Debug.LogWarning("No playerController found.");
            return;
        }

        if (player.IsAmmoFull()) // If the player's ammo is already full, exit the method without picking up the ammo.
            return;

        pickedUp = true;

        player.AddAmmo(ammoAmount); // Add the specified amount of ammo to the player's ammo count.

        ammoSpawner spawner = FindFirstObjectByType<ammoSpawner>();
        if (spawner != null)
        {
            spawner.AmmoPickedUp();
        }

        GetComponent<Collider>().enabled = false;
        Destroy(gameObject);

    }
}