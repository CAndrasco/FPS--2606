using UnityEngine;
using TMPro;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine.AI;

public class playerController : MonoBehaviour, IDamage
{
    [Header("---- Player Components ----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Header("---- Player Stats ----")]
    [SerializeField] int HP;
    [Range(1,10)] [SerializeField] int speed;
    [Range(2, 6)] [SerializeField] int sprintMod;
    [SerializeField] int gravity;
    //[SerializeField] int sprintSpeed = 10;
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

    [Header("----Audio----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audJump;
    [SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audStep;
    [SerializeField] float audStepVol;


    int HPOriginal;
    float shootTimer;
    bool isPlayingStep;
    bool isSprinting;

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
        sprint();
        flashlightToggle();
    }

    void movement()
    {
        shootTimer += Time.deltaTime;

        //OLD JUMP CODE. CHANGED TO ITS OWN FUNCTION - ERIC
        //if (controller.isGrounded)
        //{
        //    if (playerVel.y < 0)
        //    {
        //        playerVel.y = -2f;
        //    }

        //    if (Input.GetButtonDown("Jump"))
        //    {
        //        playerVel.y = jumpForce;
        //    }
        //}
        jump();

        moveDir = Input.GetAxis("Horizontal") * transform.right +
                  Input.GetAxis("Vertical") * transform.forward;

       
        int currentSpeed = speed;

        //OLD SPRINT CODE. CHANGED TO ITS OWN FUNCTION - ERIC
        //if (Input.GetKey(KeyCode.LeftShift))
        //{
        //    currentSpeed = sprintSpeed;

        //}


        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);

        if(moveDir.normalized.magnitude > 0.3f && !isPlayingStep)
        StartCoroutine(playStep());
    }
    IEnumerator playStep()
    {
        isPlayingStep = true;
        aud.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVol);
        if (isSprinting)
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
        isPlayingStep = false;
    }

    void sprint()
    {
        if(Input.GetButtonDown ("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if(Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }
    void jump()
    {
        if (controller.isGrounded)
        {
            if (playerVel.y < 0)
            {
                playerVel.y = -2f;
            }

            if (Input.GetButtonDown("Jump"))
            {
                playerVel.y = jumpForce;
                aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
            }
        }
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
        //wake up blood overlay
        if (gamemanager.instance.bloodOverlay != null & !gamemanager.instance.bloodOverlay.gameObject.activeSelf)
        {
            gamemanager.instance.bloodOverlay.gameObject.SetActive(true);
        }
        updatePlayerUI();
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            gamemanager.instance.youLose();
        }
    }
    IEnumerator flashDamage()
    {
        gamemanager.instance.damagePlayerFlash.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        gamemanager.instance.damagePlayerFlash.SetActive(false);
    }

    public void updatePlayerUI()
    {
        if (gamemanager.instance != null && gamemanager.instance.playerHPBar != null)
        {
            float hpRatio = (float)HP / HPOriginal;
            gamemanager.instance.playerHPBar.fillAmount = hpRatio;

            //handle blood overlay
            if(gamemanager.instance.bloodOverlay != null)
            {
                //get current color
                Color c = gamemanager.instance.bloodOverlay.color;
                //flip ratio
                c.a = 1f - hpRatio;
                //apply it back
                gamemanager.instance.bloodOverlay.color = c;
            }


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