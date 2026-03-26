using UnityEngine;

public class gunSystem2 : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform shootPosition;
    [SerializeField] playerController player;
    [SerializeField] weaponRecoil recoil;

    [SerializeField] AudioSource shootAudio; //for the gunshots.

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

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        transform.localPosition = myGunStats.holdPosition;
        transform.localRotation = Quaternion.Euler(myGunStats.holdRotation);

        if (shootPosition != null)
        {
            shootPosition.localPosition = myGunStats.shootPositionOffset;
        }
    }

    void Update()
    {
        if (myGunStats == null)
            return;

        if (myGunStats.automatic)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
                Shoot();
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
                Shoot();
        }
    }

    void Shoot()
    {
        if (player.GetCurrentAmmo() <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }

        nextTimeToFire = Time.time + myGunStats.shootRate;

        player.UseAmmo(1);

        // plays audio
        if (shootAudio != null && myGunStats.shootSound.Length > 0)
        {
            int index = Random.Range(0, myGunStats.shootSound.Length);
            shootAudio.pitch = Random.Range(0.9f, 1.1f);
            shootAudio.PlayOneShot(myGunStats.shootSound[index], myGunStats.shootSoundVol);
        }

        int pelletCount = Mathf.Max(1, myGunStats.pelletsPerShot);

        for (int i = 0; i < pelletCount; i++)
        {
            Quaternion spreadRotation = shootPosition.rotation * Quaternion.Euler(
                Random.Range(-myGunStats.spread, myGunStats.spread),
                Random.Range(-myGunStats.spread, myGunStats.spread),
                0f
            );

            Instantiate(bulletPrefab, shootPosition.position, spreadRotation);
        }

        if (recoil != null)
        {
            recoil.Fire();
        }
    }
}