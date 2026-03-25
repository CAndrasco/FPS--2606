using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class gunSystem2 : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform shootPosition;
    [SerializeField] playerController player; // drag player into Inspector
    [SerializeField] weaponRecoil recoil;
    gunStats myGunStats;

    public void SetGunStats(gunStats gun)
    {
        myGunStats = gun;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab is missing on " + gameObject.name);
            return;
        }
        if (shootPosition == null)
        {
            Debug.LogError("Shoot Position is missing on " + gameObject.name);
            return;
        }
        if (myGunStats == null)
        {
            Debug.LogError("GunStats is missing on " + gameObject.name);
            return;
        }
        if (player == null)
        {
            Debug.LogError("playerController reference is missing on " + gameObject.name);
            return;
        }
        recoil.Fire();
        // Ask playerController if there's ammo
        if (player.GetCurrentAmmo() <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }
       
        // Deduct ammo through playerController so UI stays in sync
        player.UseAmmo(1);

        GameObject bullet = Instantiate(bulletPrefab, shootPosition.position, shootPosition.rotation);
        Debug.Log("Spawned bullet: " + bullet.name);
    }
}