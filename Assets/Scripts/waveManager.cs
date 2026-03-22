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

    public void StartFirstWave()
    {
        StartWave1();
    }

    void StartWave1()
    {

        currentWave = 1;
        enemiesAlive = wave1Enemies.Length;

        ActivateEnemies(wave1Enemies);

        gamemanager.instance.updateGameGoal(currentWave, enemiesAlive);
    }

    void startWave2()
    {
        currentWave = 2;
        enemiesAlive = wave2Enemies.Length;

        ActivateEnemies(wave2Enemies);

        gamemanager.instance.updateGameGoal(currentWave, enemiesAlive);
    }

    void startFinalWave()
    {
        currentWave = 3;
        enemiesAlive = bossEnemies.Length;

        ActivateEnemies(bossEnemies);

        gamemanager.instance.updateGameGoal(currentWave, enemiesAlive);
    }

    void ActivateEnemies(GameObject[] enemies)
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

    public void EnemyKilled()
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
