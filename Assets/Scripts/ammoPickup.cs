using UnityEngine;

public class ammoPickup : MonoBehaviour
{
    bool pickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;
        if (!other.CompareTag("Player")) return;

        playerController player = other.GetComponent<playerController>();

        if (player == null)
        {
            Debug.LogWarning("No playerController found.");
            return;
        }

        pickedUp = true;

        // added full ammo refills.L
        player.RefillAllAmmo();

        if (ammoSpawner.instance != null)
        {
            ammoSpawner.instance.AmmoPickedUp();
        }

        GetComponent<Collider>().enabled = false;
        Destroy(gameObject);
    }
}