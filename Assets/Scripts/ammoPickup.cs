using UnityEngine;
//If you see this you're safe to work. 
public class ammoPickup : MonoBehaviour
{
    [SerializeField] int ammoAmount = 1;

    bool pickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;

        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent<playerController>();

            if (player != null ) 
            {
                if (player.IsAmmoFull())
                    return;

                pickedUp = true;

                player.AddAmmo(ammoAmount);

                FindFirstObjectByType<ammoSpawner>().AmmoPickedUp();

                GetComponent<Collider>().enabled = false;

                Destroy(gameObject);

            }
        }
    }
}