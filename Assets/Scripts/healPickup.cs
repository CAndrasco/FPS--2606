using UnityEngine;

public class healPickup : MonoBehaviour
{
    [SerializeField] int healCount;
    [SerializeField] float rotateSpeed;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

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
