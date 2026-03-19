using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject exitDistance;

    [SerializeField] TMP_Text waveCounter;
    [SerializeField] TMP_Text zombieCounter;
    [SerializeField] TMP_Text exitDistanceText;

    public Image playerHPBar;
    public GameObject player;
    public playerController playerScript;

    public bool isPaused;

    float timeScaleOrig;

    float gameExitDistance;
    
    void Awake()
    {
        instance = this;

        Time.timeScale = 1;
        timeScaleOrig = 1;

        player = GameObject.FindWithTag("Player");

        if(player != null)
        {
            playerScript = player.GetComponent<playerController>();
        }
        
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //now starts waves through different manager
        waveManager.instance.StartFirstWave();
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

        // Real time distance tracking
        // Only runs if exit door exists (wave 3)
        if (waveManager.instance != null && waveManager.instance.exitDoor != null && waveManager.instance.exitDoor.activeInHierarchy)
        {
            //You can replace Camera.main with player.transform as an alternate way. I was just trying to get it to work... - T
            float actualDistance = Vector3.Distance(Camera.main.transform.position, waveManager.instance.exitDoor.transform.position);

            exitDistanceText.text = actualDistance.ToString("F0") + "m";

            //The trigger script on exit door handles the win. - T

        }
    }

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

    public void updateGameGoal(int wave, int enemies)
    {
        waveCounter.text = wave.ToString();
        zombieCounter.text = enemies.ToString();
    }

    public void showExitDistance()
    {
        exitDistance.SetActive(true);
    }

    public void youLose()
    {
        statePause();

        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void youWin() // Called by exitDoor trigger when player enters the exit door
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

}