using UnityEngine;

public class gunSystem2 : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform shootPosition;
    [SerializeField] playerController player;
    [SerializeField] weaponRecoil recoil;

    gunStats myGunStats;
    float nextTimeToFire;

    void Awake()
    {
        if (player == null)
            player = FindFirstObjectByType<playerController>();

        if (recoil == null)
            recoil = GetComponentInChildren<weaponRecoil>();
    }

    public void SetGunStats(gunStats gun)
    {
        myGunStats = gun;
    }

    void Update()
    {
        if (myGunStats == null)
            return;

        if (myGunStats.automatic)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                Shoot();
            }
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

        if (player.GetCurrentAmmo() <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }

        nextTimeToFire = Time.time + myGunStats.shootRate;

        player.UseAmmo(1);

        int pelletCount = Mathf.Max(1, myGunStats.pelletsPerShot);

        for (int i = 0; i < pelletCount; i++)
        {
            Quaternion spreadRotation = shootPosition.rotation * Quaternion.Euler(
                Random.Range(-myGunStats.spread, myGunStats.spread),
                Random.Range(-myGunStats.spread, myGunStats.spread),
                0f
            );

            GameObject bullet = Instantiate(bulletPrefab, shootPosition.position, spreadRotation);

           
        }

        if (recoil != null)
        {
            recoil.Fire();
        }
    }
}