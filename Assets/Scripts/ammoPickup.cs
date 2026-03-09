using UnityEngine;

public class ammoPickup : MonoBehaviour
{
    [SerializeField] int ammoAmount = 1;

    void OnTriggerEnter(Collider other)
    {
        playerController player = other.GetComponent<playerController>();

        if (player != null)
        {
            player.AddAmmo(ammoAmount);
            Destroy(gameObject);
        }
    }
}