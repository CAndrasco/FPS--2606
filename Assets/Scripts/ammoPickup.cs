using UnityEngine;
//If you see this you're safe to work. 
public class ammoPickup : MonoBehaviour
{
    [SerializeField] int ammoAmount = 1;

    bool pickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;
        if (!other.CompareTag("Player")) return;

        gunSystem gun = other.GetComponentInChildren<gunSystem>();

        if (gun == null)
        {
            Debug.LogWarning("No gunSystem found on player.");
            return;
        }

        if (gun.IsAmmoFull())
            return;

        pickedUp = true;

        gun.AddAmmo(ammoAmount);

        ammoSpawner spawner = FindFirstObjectByType<ammoSpawner>();
        if (spawner != null)
        {
            spawner.AmmoPickedUp();
        }

        GetComponent<Collider>().enabled = false;
        Destroy(gameObject);

    }
}