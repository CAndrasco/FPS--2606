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

    // ---------------- WAVE START ----------------

    public void startFirstWave()
    {
        currentWave = 1;
        StartCoroutine(SpawnWave(weakEnemyPrefab, 8));
    }

    IEnumerator SpawnWave(GameObject enemyPrefab, int amount)
    {
        enemiesAlive = amount;

        gamemanager.instance.updateGameGoal(currentWave, enemiesAlive);

        for (int i = 0; i < amount; i++)
        {
            Spawn(enemyPrefab);
            yield return new WaitForSeconds(spawnRate);
        }
    }

    // ---------------- SPAWN ----------------

    void Spawn(GameObject enemy)
    {
        for (int i = 0; i < 10; i++) // try multiple positions
        {
            Vector3 ranPos = Random.insideUnitSphere * spawnDist;
            ranPos += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(ranPos, out hit, spawnDist, NavMesh.AllAreas))
            {
                // this will prevent roof spawns
                if (hit.position.y > transform.position.y + 2f)
                    continue;

                // prevents indoor spawns (ceiling check)
                if (Physics.Raycast(hit.position + Vector3.up * 1f, Vector3.up, 10f))
                    continue;

                Instantiate(enemy, hit.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
                return;
            }
        }

        Debug.LogWarning("Failed to find valid spawn position");
    }

    // ---------------- ENEMY KILLED ----------------

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

    // ---------------- EXIT ----------------

    void spawnExitDoor()
    {
        if (exitSpawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, exitSpawnPoints.Length);
        Transform spawnPoint = exitSpawnPoints[randomIndex];

        exitDoor = Instantiate(exitDoorPrefab, spawnPoint.position, spawnPoint.rotation);

        gamemanager.instance.showExitDistance();
    }
}