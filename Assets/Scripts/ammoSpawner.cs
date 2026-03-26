using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ammoSpawner : MonoBehaviour
{
    public static ammoSpawner instance;

    [Header("---- Ammo ----")]
    [SerializeField] GameObject ammoPrefab;

    [Header("---- Spawn Settings ----")]
    [SerializeField] int maxAmmoOnMap = 5;
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

    // ---------------- spawn ammo ----------------

    void SpawnAmmo()
    {
        int groundMask = LayerMask.GetMask("Environment");

        for (int i = 0; i < 20; i++)
        {
            Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
            randomPos += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, spawnRadius, NavMesh.AllAreas))
            {
                RaycastHit groundHit;

                // raycast down only hitting Environment layer
                if (Physics.Raycast(hit.position + Vector3.up * 2f, Vector3.down, out groundHit, 10f, groundMask))
                {
                    // tag is ground.
                    if (!groundHit.collider.CompareTag("Ground"))
                        continue;

                    // get correct height from collider
                    float heightOffset = 0.5f;
                    Collider col = ammoPrefab.GetComponentInChildren<Collider>();

                    if (col != null)
                    {
                        heightOffset = col.bounds.extents.y;
                    }

                    // final spawn position heightoffset is important....
                    Vector3 spawnPos = groundHit.point + Vector3.up * (heightOffset + 0.5f);

                    Instantiate(ammoPrefab, spawnPos, Quaternion.identity);
                    currentAmmo++;
                    return;
                }
            }
        }

        Debug.LogWarning("Failed to find valid ammo spawn");
    }

    // ---------------- pickup ----------------

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