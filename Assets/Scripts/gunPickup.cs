using UnityEngine;

public class gunPickup : MonoBehaviour
{
    [Header("The gun prefab that gets equipped")]
    [SerializeField] GameObject gunPrefabToEquip;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerGunHolder gunHolder = other.GetComponent<playerGunHolder>();

        if (gunHolder != null)
        {
            gunHolder.EquipGun(gunPrefabToEquip);
            Destroy(gameObject);
        }
    }
}
