using UnityEngine;

public class gunSystem : MonoBehaviour
{
    //Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsShot;
    public GameObject bulletPrefab;

    //bools 
    bool shooting, readyToShoot, reloading;
    bool triggerReleased = true; // This variable tracks whether the trigger has been released since the last shot, preventing continuous shooting when the mouse button is held down.


    //Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;


    //Graphics
    public GameObject muzzleFlash, bulletHoleGraphic;
  
    public float camShakeMagnitude, camShakeDuration;

    playerController player; // Reference to the playerController script.


    private void Awake()
    {
        player = FindFirstObjectByType<playerController>(); // Find the playerController script in the scene and assign it to the player variable.

        readyToShoot = true;

        if (fpsCam == null)
        {
            fpsCam = Camera.main;
        }
    }
    private void Update()
    {
        MyInput();
        //UpdateAmmoUI();
    }

    private void MyInput()
    {
        if (Input.GetMouseButtonUp(0))
            triggerReleased = true;

        if (triggerReleased && Input.GetMouseButtonDown(0) && readyToShoot && !reloading && player.GetCurrentAmmo() > 0)
        {
            triggerReleased = false;
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && player.GetCurrentAmmo() < magazineSize && !reloading)
        {
            Reload();
        }

        
    }

    private void Shoot()
    {
        Debug.Log("Shoot called on: " + gameObject.name + " frame: " + Time.frameCount); // Log the name of the gun that is shooting for debugging purposes.

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

        player.UseAmmo(1); // Tell the playerController script that ammo has been used.

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


