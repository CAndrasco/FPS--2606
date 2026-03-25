using UnityEngine;

public class healPickup : MonoBehaviour
{
    [SerializeField] int healCount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IPickup pickup = other.GetComponent<IPickup>();

            if (pickup != null )
            {
                pickup.addHeal(healCount);
                Destroy(gameObject);
            }
        }
    }
}
