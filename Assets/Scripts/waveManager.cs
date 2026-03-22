using JetBrains.Annotations;
using UnityEngine;

public class waveManager : MonoBehaviour
{
    public static waveManager instance;

    [SerializeField] GameObject[] wave1Enemies;
    [SerializeField] GameObject[] wave2Enemies;
    [SerializeField] GameObject[] bossEnemies;

    [SerializeField] GameObject exitDoorPrefab;
    [SerializeField] Transform[] exitSpawnPoints;

    public GameObject exitDoor;

    public int enemiesAlive;
    int currentWave;


    void Awake()
    {
        instance = this;
    }

    public void startFirstWave()
    {
        startWave1();
    }

    void startWave1()
    {

        currentWave = 1;
        enemiesAlive = wave1Enemies.Length;

        activateEnemies(wave1Enemies);

        gamemanager.instance.updateGameGoal(currentWave, enemiesAlive);
    }

    void startWave2()
    {
        currentWave = 2;
        enemiesAlive = wave2Enemies.Length;

        activateEnemies(wave2Enemies);

        gamemanager.instance.updateGameGoal(currentWave, enemiesAlive);
    }

    void startFinalWave()
    {
        currentWave = 3;
        enemiesAlive = bossEnemies.Length;

        activateEnemies(bossEnemies);

        spawnExitDoor();

        gamemanager.instance.updateGameGoal(currentWave, enemiesAlive);
    }

    void activateEnemies(GameObject[] enemies)
    {
        for (int i = 0;
            i < enemies.Length;
            i++)
        {
            enemies[i].SetActive(true);
        }
    }
    
    void spawnExitDoor()
    {
        int randomIndex = Random.Range(0, exitSpawnPoints.Length);

        Transform spawnPoint = exitSpawnPoints[randomIndex];

        spawnPoint.parent.gameObject.SetActive(false);

        exitDoor = Instantiate(exitDoorPrefab, spawnPoint.position, spawnPoint.rotation);

        gamemanager.instance.showExitDistance();
    }

    public void enemyKilled()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0)
        {
            if (currentWave == 1)
                startWave2();
            else if (currentWave == 2)
                startFinalWave();
        }
        gamemanager.instance.updateGameGoal(currentWave, enemiesAlive);
    }


}
