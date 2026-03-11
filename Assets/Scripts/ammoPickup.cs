using UnityEngine;
//If you see this you're safe to work. 
public class ammoPickup : MonoBehaviour
{
    [SerializeField] int ammoAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent<playerController>();

            if (player != null ) 
            {
                player.AddAmmo(ammoAmount);

                FindFirstObjectByType<ammoSpawner>().AmmoPickedUp();

                Destroy(gameObject);

            }
        }
    }
}