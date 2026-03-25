using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [Header("---- Menus ----")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject exitDistance;

    [Header("---- UI ----")]
    [SerializeField] TMP_Text waveCounter;
    [SerializeField] TMP_Text zombieCounter;
    [SerializeField] TMP_Text exitDistanceText;

    public Image playerHPBar;
    public GameObject player;
    public playerController playerScript;
    public GameObject damagePlayerFlash;
    public Image bloodOverlay;

    public bool isPaused;

    float timeScaleOrig;

    void Awake()
    {
        instance = this;

        Time.timeScale = 1;
        timeScaleOrig = 1;

        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerScript = player.GetComponent<playerController>();
        }

        //find the menus and turn them off
        menuPause = GameObject.Find("Pause Menu");
        if (menuPause) menuPause.SetActive(false);
        menuLose = GameObject.Find("Lose Menu");
        if (menuLose) menuLose.SetActive(false);
        menuWin = GameObject.Find("Win Menu");
        if (menuWin) menuWin.SetActive(false);
        //find exit distance UI and Text then turn off (have to find child first)
        GameObject exitDist = GameObject.Find("Exit Distance Text");
        if (exitDist != null) exitDistanceText = exitDist.GetComponent<TMP_Text>();
        exitDistance = GameObject.Find("Exit Distance UI");
        if (exitDistance) exitDistance.SetActive(false);
        //wave and zombie counter find
        GameObject wCount = GameObject.Find("Wave Count Text");
        if (wCount != null) waveCounter = wCount.GetComponent<TMP_Text>();
        GameObject zCount = GameObject.Find("Zombie Count Text");
        if (zCount != null) zombieCounter = zCount.GetComponent<TMP_Text>();
        //find player HP
        GameObject playerHealth = GameObject.Find("Player HP Bar");
        if (playerHealth != null) playerHPBar = playerHealth.GetComponent<Image>();


    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }

        // Exit distance tracking
        if (waveManager.instance != null && waveManager.instance.exitDoor != null)
        {
            float dist = Vector3.Distance(
                player.transform.position,
                waveManager.instance.exitDoor.transform.position
            );

            exitDistanceText.text = dist.ToString("F0") + "m";
        }
    }

    // ---------------- UI UPDATE ----------------

    public void updateGameGoal(int wave, int enemies)
    {
        waveCounter.text = wave.ToString();
        zombieCounter.text = enemies.ToString();
    }

    public void showExitDistance()
    {
        exitDistance.SetActive(true);
    }

    // ---------------- PAUSE ----------------

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        menuActive.SetActive(false);
        menuActive = null;
    }

    // ---------------- GAME STATES ----------------

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void youWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }
}