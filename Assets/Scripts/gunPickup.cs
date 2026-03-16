using UnityEngine;

public class gunPickup : MonoBehaviour
{
    public enum GunType
    {
        Pistol,
        ShotGun,
        MachineGun
    }

    [SerializeField] GunType gunType;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerGunHolder gunHolder = other.GetComponentInParent<playerGunHolder>();

        if (gunHolder == null)
        {
            Debug.LogError("playerGunHolder not found on player!");
            return;
        }

        switch (gunType)
        {
            case GunType.Pistol:
                gunHolder.EquipPistol();
                break;

            case GunType.ShotGun:
                gunHolder.EquipShotGun();
                break;

            case GunType.MachineGun:
                gunHolder.EquipMachineGun();
                break;
        }

        Destroy(gameObject);
    }
}
