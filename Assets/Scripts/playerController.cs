using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class playerController : MonoBehaviour, IDamage, IPickup
{
    [Header("---- Player Components ----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;



    [Header("---- Player Stats ----")]
    [SerializeField] int HP;
    [Range(1, 10)][SerializeField] int speed;
    [Range(2, 6)][SerializeField] int sprintMod;
    [SerializeField] int gravity;
    [SerializeField] int jumpForce = 8;

    [Header("---- Flashlight ----")]
    [SerializeField] Light flashlight;

    [Header("---- Gun ----")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] gunSystem2 gunSystem;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    [Header("---- Ammo ----")]
    [SerializeField] int ammo = 0;
    [SerializeField] int ammoMax = 8;
    [SerializeField] TMP_Text ammoCountText;
    [SerializeField] TMP_Text ammoMaxText;

    [Header("---- Healing ----")]
    [SerializeField] int healAmount;
    [SerializeField] int maxHeals;

    [Header("----Audio----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audJump;
    [SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audStep;
    [SerializeField] float audStepVol;

    int HPOriginal;
    int currentHeals;
    int gunListPos;

    float shootTimer;

    bool isPlayingStep;
    bool isSprinting;

    Vector3 moveDir;
    Vector3 playerVel;

    void Start()
    {
        HPOriginal = HP;
        updatePlayerUI();
        gamemanager.instance.updateMedUI(currentHeals, maxHeals);
    }

    void Update()
    {
        if (gamemanager.instance != null && gamemanager.instance.isPaused)
            return;

        if (Input.GetButtonDown("Heal"))
        {
            useHeal();
        }

        movement();
        sprint();
        flashlightToggle();
    }

    void movement()
    {
        shootTimer += Time.deltaTime;

        jump();

        moveDir = Input.GetAxis("Horizontal") * transform.right +
                  Input.GetAxis("Vertical") * transform.forward;

        int currentSpeed = isSprinting ? speed * sprintMod : speed;

        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);

        if (moveDir.normalized.magnitude > 0.3f && !isPlayingStep)
            StartCoroutine(playStep());

        selectGun();
    }

    IEnumerator playStep()
    {
        isPlayingStep = true;

        if (audStep.Length > 0 && aud != null)
        {
            aud.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVol);
        }

        yield return new WaitForSeconds(isSprinting ? 0.3f : 0.5f);

        isPlayingStep = false;
    }

    void sprint()
    {
        isSprinting = Input.GetButton("Sprint");
    }

    void jump()
    {
        if (controller.isGrounded)
        {
            if (playerVel.y < 0)
                playerVel.y = -2f;

            if (Input.GetButtonDown("Jump"))
            {
                playerVel.y = jumpForce;

                if (audJump.Length > 0 && aud != null)
                {
                    aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
                }
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
            ammo = ammoMax;

        updatePlayerUI();
    }

    // new method for ammo, (ammo pickup uses this)
    public void RefillAllAmmo()
    {
        ammo = ammoMax;
        updatePlayerUI();
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        if (gamemanager.instance.bloodOverlay != null &&
            !gamemanager.instance.bloodOverlay.gameObject.activeSelf)
        {
            gamemanager.instance.bloodOverlay.gameObject.SetActive(true);
        }

        updatePlayerUI();

        if (audHurt.Length > 0 && aud != null)
        {
            aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        }

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

            if (gamemanager.instance.bloodOverlay != null)
            {
                Color c = gamemanager.instance.bloodOverlay.color;
                c.a = 1f - hpRatio;
                gamemanager.instance.bloodOverlay.color = c;
            }           
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
    public int GetAmmoMax()
    {
        return ammoMax;
    }

    public void UseAmmo(int amount)
    {
        ammo -= amount;

        if (ammo < 0)
            ammo = 0;

        updatePlayerUI();
    }

    void useHeal()
    {
        if (currentHeals <= 0 || HP >= HPOriginal)
            return;

        currentHeals--;
        HP += healAmount;

        if (HP > HPOriginal)
        {
            HP = HPOriginal;
        }

        updatePlayerUI();
        gamemanager.instance.updateMedUI(currentHeals, maxHeals);
    }

    public void addHeal(int amount)
    {
        if (currentHeals >= maxHeals)
            return;

        currentHeals += amount;

        if (currentHeals > maxHeals)
        {
            currentHeals = maxHeals;
        }
        gamemanager.instance.updateMedUI(currentHeals, maxHeals);
    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;
        changeGun();
      
    }

    void changeGun()
    {

        shootDamage = gunList[gunListPos].shootDamage;
        shootDist = gunList[gunListPos].shootDist;
        shootRate = gunList[gunListPos].shootRate;
        //ammo = gunList[gunListPos].ammoCur;
        //ammoMax = gunList[gunListPos].ammoMax;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        gunSystem.SetGunStats(gunList[gunListPos]);
        gamemanager.instance.updateGunUI(gunList[gunListPos]);

    }

    void selectGun()
    { 
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count -1)
        {
            gunListPos++;
            changeGun();
           
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            changeGun();
        }
    }
}