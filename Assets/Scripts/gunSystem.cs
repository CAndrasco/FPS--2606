using UnityEngine;

public class gunSystem : MonoBehaviour
{
    [Header("Gun Data")]
    [SerializeField] gunStats stats;

    [Header("Projectile Settings")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform attackPoint;
    [SerializeField] Camera fpsCam;
    [SerializeField] GameObject muzzleFlash;

    [Header("Spread")]
    [SerializeField] float spread = 0.02f;

    bool readyToShoot = true;
    bool reloading = false;
    bool triggerReleased = true;

    int currentAmmo;

    playerController player;
    AudioSource audioSource;
    RaycastHit rayHit;

    private void Awake()
    {
        player = GetComponentInParent<playerController>();
        audioSource = GetComponent<AudioSource>();

        if (fpsCam == null)
            fpsCam = GetComponentInParent<Camera>();

        if (fpsCam == null)
            fpsCam = Camera.main;

        if (stats != null)
            currentAmmo = stats.ammoMax;
    }

    private void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        if (Input.GetMouseButtonUp(0))
            triggerReleased = true;

        if (triggerReleased && Input.GetMouseButtonDown(0) && readyToShoot && !reloading && currentAmmo > 0)
        {
            triggerReleased = false;
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < stats.ammoMax && !reloading)
        {
            Reload();
        }
    }

    private void Shoot()
    {
        if (stats == null || bulletPrefab == null || attackPoint == null || fpsCam == null)
        {
            Debug.LogError("gunSystem missing references on " + gameObject.name);
            return;
        }

        readyToShoot = false;

        Vector3 targetPoint;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out rayHit, stats.shootDist))
        {
            targetPoint = rayHit.point;
        }
        else
        {
            targetPoint = fpsCam.transform.position + fpsCam.transform.forward * stats.shootDist;
        }

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0f);

        GameObject currentBullet = Instantiate(bulletPrefab, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        if (muzzleFlash != null)
        {
            GameObject flash = Instantiate(muzzleFlash, attackPoint.position, attackPoint.rotation);
            Destroy(flash, 2f);
        }

        PlayShootSound();

        currentAmmo--;

        Invoke(nameof(ResetShot), stats.shootRate);
    }

    void PlayShootSound()
    {
        if (audioSource != null && stats.shootSound != null && stats.shootSound.Length > 0)
        {
            int rand = Random.Range(0, stats.shootSound.Length);
            audioSource.PlayOneShot(stats.shootSound[rand], stats.shootSoundVol);
        }
    }

    void ResetShot()
    {
        readyToShoot = true;
    }

    void Reload()
    {
        reloading = true;
        Invoke(nameof(ReloadFinished), 1f);
    }

    void ReloadFinished()
    {
        currentAmmo = stats.ammoMax;
        reloading = false;
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;

        if (currentAmmo > stats.ammoMax)
            currentAmmo = stats.ammoMax;
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMaxAmmo()
    {
        return stats != null ? stats.ammoMax : 0;
    }
}