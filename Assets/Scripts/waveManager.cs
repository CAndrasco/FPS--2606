using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class waveManager : MonoBehaviour
{
    public static waveManager instance;

    [Header("---- Enemy Prefabs ----")]
    [SerializeField] GameObject weakEnemyPrefab;
    [SerializeField] GameObject strongEnemyPrefab;
    [SerializeField] GameObject bossEnemyPrefab;

    [Header("---- Spawn Settings ----")]
    [SerializeField] int spawnRate = 2;
    [SerializeField] int spawnDist = 25;

    [Header("---- Exit ----")]
    [SerializeField] GameObject exitDoorPrefab;
    [SerializeField] Transform[] exitSpawnPoints;

    public GameObject exitDoor;

    int currentWave;
    public int enemiesAlive;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        startFirstWave();
    }

    // start wave 1
    public void startFirstWave()
    {
        currentWave = 1;
        StartCoroutine(SpawnWave(weakEnemyPrefab, 8));
    }

    IEnumerator SpawnWave(GameObject enemyPrefab, int amount)
    {
        // start at 0 instead of amount because spawns can fail
        enemiesAlive = 0;
        int spawned = 0;

        gamemanager.instance.updateGameGoal(currentWave, enemiesAlive);

        // keep trying until we actually spawn all enemies
        while (spawned < amount)
        {
            if (Spawn(enemyPrefab))
            {
                spawned++;
                enemiesAlive++;

                gamemanager.instance.updateGameGoal(currentWave, enemiesAlive);
            }

            yield return new WaitForSeconds(spawnRate);
        }
    }

    // tries to spawn enemy, returns true if it worked
    public bool Spawn(GameObject enemy)
    {
        // try multiple positions
        for (int i = 0; i < 15; i++)
        {
            Vector3 ranPos = Random.insideUnitSphere * spawnDist;
            ranPos += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(ranPos, out hit, spawnDist, NavMesh.AllAreas))
            {
                // skip rooftops
                if (hit.position.y > transform.position.y + 2f)
                    continue;

                // skip indoor ceilings
                if (Physics.Raycast(hit.position + Vector3.up * 1f, Vector3.up, 10f))
                    continue;

                Instantiate(enemy, hit.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
                return true;
            }
        }

        Debug.LogWarning("Failed to find valid spawn position");
        return false;
    }

    public void enemyKilled()
    {
        enemiesAlive--;

        gamemanager.instance.updateGameGoal(currentWave, enemiesAlive);

        if (enemiesAlive <= 0)
        {
            if (currentWave == 1)
            {
                currentWave = 2;
                StartCoroutine(SpawnWave(strongEnemyPrefab, 8));
            }
            else if (currentWave == 2)
            {
                currentWave = 3;
                StartCoroutine(SpawnWave(bossEnemyPrefab, 1));
                spawnExitDoor();
            }
        }
    }

    void spawnExitDoor()
    {
        if (exitSpawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, exitSpawnPoints.Length);
        Transform spawnPoint = exitSpawnPoints[randomIndex];

        exitDoor = Instantiate(exitDoorPrefab, spawnPoint.position, spawnPoint.rotation);

        gamemanager.instance.showExitDistance();
    }
}