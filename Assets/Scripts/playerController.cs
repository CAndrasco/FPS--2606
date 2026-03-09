using UnityEngine;

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
    [SerializeField] int ammoMax = 10; //Player max ammo.

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

        if(Input.GetButton("Fire1") && shootTimer >= shootRate && ammo > 0)
        {
            shoot();
        }
    }

    void flashlightToggle()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            flashlight.enabled = !flashlight.enabled;
        }
    }

    void shoot()
    {
        shootTimer = 0;
        ammo--;

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position,
                Camera.main.transform.forward,
                out hit,
                shootDist,
                ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.TakeDamage(shootDamage);
            }
        }
    }

    public void AddAmmo(int amount)
    {
        ammo += amount;

        if (ammo > ammoMax)
        {
            ammo = ammoMax;
        }
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
        //commented out until we get UI done. 
       // gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOriginal;
    }
    

}
