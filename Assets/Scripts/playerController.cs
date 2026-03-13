using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IDamage
{
    [Header("---- Player Components ----")]
    [SerializeField] CharacterController controller; //Player Character Controller.
    [SerializeField] LayerMask ignoreLayer; //Layer mask for the raycast to detect objects.

    [Header("---- Player Stats ----")]
    [SerializeField] int HP; //Players Health.
    [SerializeField] int speed; //Players Movement Speed.
    [SerializeField] int gravity; //Players Gravity.

    [Header("---- Flashlight ----")]
    [SerializeField] Light flashlight; //Players flashlight.

    [Header("---- Gun ----")]
    [SerializeField] int shootDamage; //Damage of the gun.
    [SerializeField] int shootDist; //shoot distance.
    [SerializeField] float shootRate; //Shoot rate.

    [Header("---- Ammo ----")]

    [SerializeField] int ammo = 0; //Player current ammo.
    [SerializeField] int ammoMax = 8; // Max ammo = 8 for a standard pistol.
    [SerializeField] TMP_Text ammoCountText;
    [SerializeField] TMP_Text ammoMaxText;

    int HPOriginal; //Players original health.
    float shootTimer; //Timer for the shoot rate.

    Vector3 moveDir;
    Vector3 playerVel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOriginal = HP;
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (gamemanager.instance != null && gamemanager.instance.isPaused)
        {
            return;
        }

        movement();
        flashlightToggle();
    }

    void movement()
    {
        shootTimer += Time.deltaTime;

        if (controller.isGrounded && playerVel.y < 0)
        {
            playerVel.y = 0;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right +
            Input.GetAxis("Vertical") * transform.forward;

        controller.Move(moveDir * speed * Time.deltaTime);

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);

        //if (Input.GetButton("Fire1") && shootTimer >= shootRate && ammo > 0)
        //{
        //    shoot();
        //}

        //Shooting handled by gunSystem.
    }

    void flashlightToggle()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlight.enabled = !flashlight.enabled;
        }
    }

    // Shooting handled by gunSystem.

    //void shoot()
    //{
    //    if (ammo <= 0)
    //        return;

    //    shootTimer = 0;
    //    ammo--;
    //    updatePlayerUI();

    //    RaycastHit hit;

    //    if (Physics.Raycast(Camera.main.transform.position,
    //            Camera.main.transform.forward,
    //            out hit,
    //            shootDist,
    //            ~ignoreLayer))
        
          
    //}

    public void AddAmmo(int amount)
    {
        ammo += amount;

        if (ammo > ammoMax)
        {
            ammo = ammoMax;
        }

        updatePlayerUI();
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();

        if (HP <= 0)
        {
            gamemanager.instance.youLose();
        }
    }

    public void updatePlayerUI()
    {
        if (gamemanager.instance != null && gamemanager.instance.playerHPBar != null)
        {
            gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOriginal;
            ammoCountText.text = ammo.ToString("F0");
            ammoMaxText.text = ammoMax.ToString("F0");
        }
    }

    public bool IsAmmoFull()
    {
           return ammo >= ammoMax;
    }

    public int GetCurrentAmmo()
    {
        return ammo;
    }

    public void UseAmmo(int amount)
    {
        ammo -= amount;
        if (ammo < 0)
            ammo = 0;

        updatePlayerUI();
    }

}
