using UnityEngine;

public class gunSystem : MonoBehaviour
{
    //Gun stats
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsShot;
    public GameObject bulletPrefab;

    //bools 
    bool shooting, readyToShoot, reloading;
    bool triggerReleased = true;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    //Graphics
    public GameObject muzzleFlash, bulletHoleGraphic;

    public float camShakeMagnitude, camShakeDuration;

    // 🔊 Gun sound
    public AudioSource gunAudio;
    public AudioClip gunShotSound;

    playerController player;



    private void Awake()
    {
        player = GetComponentInParent<playerController>();

        if (fpsCam == null)
            fpsCam = Camera.main;

        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        if (Input.GetMouseButtonUp(0))
            triggerReleased = true;

        if (triggerReleased && Input.GetMouseButtonDown(0) && readyToShoot && player.GetCurrentAmmo() > 0)
        {
            triggerReleased = false;
            Shoot();
        }
    }

    private void Shoot()
    {
        Debug.Log("Shoot called on: " + gameObject.name + " frame: " + Time.frameCount);

        readyToShoot = false;

        Vector3 targetPoint;

        // Aim from center of camera
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out rayHit, range))
        {
            targetPoint = rayHit.point;
        }
        else
        {
            targetPoint = fpsCam.transform.position + fpsCam.transform.forward * range;
        }

        // Direction from gun to target
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        // Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        // Spawn bullet
        GameObject currentBullet = Instantiate(bulletPrefab, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        // Spawn muzzle flash
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackPoint.position, attackPoint.rotation);
        }

        // Gunshot sound.
        if (gunAudio != null && gunShotSound != null)
        {
            gunAudio.pitch = Random.Range(0.9f, 1.1f);
            gunAudio.PlayOneShot(gunShotSound);
        }

        player.UseAmmo(1);

        Invoke("ResetShot", timeBetweenShooting);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        reloading = false;
    }
}