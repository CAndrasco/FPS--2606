using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject exitDistance;

    [SerializeField] GameObject[] wave1Enemies;
    [SerializeField] GameObject[] wave2Enemies;
    [SerializeField] GameObject bossEnemy;
    [SerializeField] GameObject exitDoor;
    [SerializeField] Transform[] exitSpawnPoints;

    [SerializeField] TMP_Text waveCounter;
    [SerializeField] TMP_Text zombieCounter;
    [SerializeField] TMP_Text exitDistanceText;

    public Image playerHPBar;
    public GameObject player;
    public playerController playerScript;

    public bool isPaused;

    float timeScaleOrig;

    float gameExitDistance;
    public int enemiesAlive;
    int currentWave;

    


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

        startWave1();
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
        if (exitDoor != null && exitDoor.activeInHierarchy)
        {
            //You can replace Camera.main with player.transform as an alternate way. I was just trying to get it to work... - T
            float actualDistance = Vector3.Distance(Camera.main.transform.position, exitDoor.transform.position);
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

    
    public void updateGameGoal()
    {
        waveCounter.text = currentWave.ToString("F0");
        zombieCounter.text = enemiesAlive.ToString("F0");
        
    }

    public void youLose()
    {
        statePause();

        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    void startWave1()
    {
        currentWave = 1;        
        enemiesAlive = wave1Enemies.Length;
        updateGameGoal();
        


        for (int i = 0;
            i < wave1Enemies.Length;
            i++)
        {
            wave1Enemies[i].SetActive(true);
        }
    }

    void startWave2()
    {
        currentWave = 2;
        enemiesAlive = wave2Enemies.Length;
        updateGameGoal();
        


        for (int i = 0;
            i < wave2Enemies.Length;
            i++)
        {
            wave2Enemies[i].SetActive(true);
        }
    }

    void startFinalWave()
    {
        currentWave = 3;    
        enemiesAlive = 1;

        updateGameGoal();
        

        bossEnemy.SetActive(true);

        int randomIndex = Random.Range(0, exitSpawnPoints.Length);

        exitDoor.transform.position = exitSpawnPoints[randomIndex].position;
        exitDoor.transform.rotation = exitSpawnPoints[randomIndex].rotation;

        exitDoor.SetActive(true);
        exitDistance.SetActive(true);
        
    }

    public void EnemyKilled()
    {
        enemiesAlive--;
        

        if (enemiesAlive <= 0)
        {
            if (currentWave == 1)
            {
                startWave2();
            }
            else if (currentWave == 2)
            {
                startFinalWave();
            }
        }
        else
        {
            updateGameGoal();
        }
            //else if (currentWave == 3)
            //{
            //    exitDoor.SetActive(true);
            //}
        
    }

    public void youWin() // Called by exitDoor trigger when player enters the exit door
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

}