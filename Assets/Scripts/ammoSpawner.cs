using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ammoSpawner : MonoBehaviour
{
    public static ammoSpawner instance;

    [Header("---- Ammo ----")]
    [SerializeField] GameObject ammoPrefab;

    [Header("---- Spawn Settings ----")]
    [SerializeField] int maxAmmoOnMap = 10;
    [SerializeField] float spawnRadius = 25f;
    [SerializeField] float spawnDelayMin = 3f;
    [SerializeField] float spawnDelayMax = 7f;

    int currentAmmo;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        for (int i = 0; i < maxAmmoOnMap; i++)
        {
            SpawnAmmo();
        }
    }

    // ---------------- SPAWN ----------------

    void SpawnAmmo()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
            randomPos += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, spawnRadius, NavMesh.AllAreas))
            {
                // prevent roof spawns
                if (hit.position.y > transform.position.y + 2f)
                    continue;

                // prevent indoor spawns
                if (Physics.Raycast(hit.position + Vector3.up, Vector3.up, 10f))
                    continue;

                Instantiate(ammoPrefab, hit.position, Quaternion.identity);
                currentAmmo++;
                return;
            }
        }

        Debug.LogWarning("Failed to find valid ammo spawn");
    }

    // ---------------- PICKUP ----------------

    public void AmmoPickedUp()
    {
        currentAmmo--;

        if (currentAmmo < maxAmmoOnMap)
        {
            StartCoroutine(DelayedSpawn());
        }
    }

    IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(Random.Range(spawnDelayMin, spawnDelayMax));
        SpawnAmmo();
    }
}