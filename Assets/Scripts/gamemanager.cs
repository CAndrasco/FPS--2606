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

    [SerializeField] GameObject[] wave1Enemies;
    [SerializeField] GameObject[] wave2Enemies;
    [SerializeField] GameObject bossEnemy;
    [SerializeField] GameObject exitDoor;

    [SerializeField] TMP_Text waveCounter;
    [SerializeField] TMP_Text zombieCounter;

    public Image playerHPBar;
    public GameObject player;
    public playerController playerScript;

    public bool isPaused;

    float timeScaleOrig;

    int gameGoalCount;
    public int enemiesAlive;
    int currentWave;


    void Awake()
    {
        instance = this;

        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
    }

    void Start()
    {
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

    // TO DO: NEED TO UPDATE FOR GAME GOAL TO BE RELATED TO EXIT
    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;

        if (gameGoalCount <= 0)
        {
            statePause();

            menuActive = menuWin;
            menuActive.SetActive(true);
        }
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
        waveCounter.text = currentWave.ToString("F0");

        enemiesAlive = wave1Enemies.Length;
        zombieCounter.text = enemiesAlive.ToString("F0");

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
        waveCounter.text = currentWave.ToString("F0");

        enemiesAlive = wave1Enemies.Length;
        zombieCounter.text = enemiesAlive.ToString("F0");


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
        waveCounter.text = currentWave.ToString("F0");

        enemiesAlive = 1;
        zombieCounter.text = enemiesAlive.ToString("F0");

        bossEnemy.SetActive(true);
        exitDoor.SetActive(true);
    }

    public void EnemyKilled()
    {
        enemiesAlive--;
        zombieCounter.text = enemiesAlive.ToString("F0");

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
            else if (currentWave == 3)
            {
                exitDoor.SetActive(true);
            }
        }
    }

}