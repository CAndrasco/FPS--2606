using UnityEngine;

public class ammoPickup : MonoBehaviour
{
    [SerializeField] AudioClip pickupSound;
    [SerializeField] float volume = 1f;

    bool pickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ammo pickup triggered");

        if (pickedUp) return;
        if (!other.CompareTag("Player")) return;

        playerController player = other.GetComponentInParent<playerController>();

        if (player == null)
        {
            Debug.LogWarning("No playerController found.");
            return;
        }

        pickedUp = true;

        // refill ammo
        player.RefillAllAmmo();

        //play pickup sound.
        AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);

        if (ammoSpawner.instance != null)
        {
            ammoSpawner.instance.AmmoPickedUp();
        }

        GetComponent<Collider>().enabled = false;
        Destroy(gameObject);
    }
}