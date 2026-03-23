using UnityEngine;

public class healPickup : MonoBehaviour
{
    [SerializeField] int healCount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent<playerController>();

            if (player != null )
            {
                player.addHeal(healCount);
                Destroy(gameObject);
            }
        }
    }
}
