using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IDamage
{
    [Header("---- Player Components ----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Header("---- Player Stats ----")]
    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int gravity;
    [SerializeField] int sprintSpeed = 10;
    [SerializeField] int jumpForce = 8;

    [Header("---- Flashlight ----")]
    [SerializeField] Light flashlight;

    [Header("---- Gun ----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    [Header("---- Ammo ----")]
    [SerializeField] int ammo = 0;
    [SerializeField] int ammoMax = 8;
    [SerializeField] TMP_Text ammoCountText;
    [SerializeField] TMP_Text ammoMaxText;

    int HPOriginal;
    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    void Start()
    {
        HPOriginal = HP;
        updatePlayerUI();
    }

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

        if (controller.isGrounded)
        {
            if (playerVel.y < 0)
            {
                playerVel.y = -2f;
            }

            if (Input.GetButtonDown("Jump"))
            {
                playerVel.y = jumpForce;
            }
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right +
                  Input.GetAxis("Vertical") * transform.forward;

        int currentSpeed = speed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }

        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);
    }

    void flashlightToggle()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlight.enabled = !flashlight.enabled;
        }
    }

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
        {
            ammo = 0;
        }

        updatePlayerUI();
    }
}