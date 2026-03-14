using System.Collections;
using UnityEngine;

public class ammoSpawner : MonoBehaviour
{
    [SerializeField] GameObject ammoPrefab;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] int maxAmmoOnMap = 5;

    int currentAmmo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < maxAmmoOnMap; i++)
        {
            SpawnAmmo();
        }
    }

    public void SpawnAmmo()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int rand = Random.Range(0,spawnPoints.Length);

            Transform spawn = spawnPoints[rand];

            if(spawn.childCount == 0)
            {
                Instantiate(ammoPrefab, spawn.position, Quaternion.identity, spawn);
                currentAmmo++;
                return;
            }
        }
    }

    public void AmmoPickedUp()
    {
        currentAmmo--;

        if(currentAmmo < maxAmmoOnMap)
        {
            StartCoroutine(DelayedSpawn());
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(Random.Range(3f,7f)); //Spawner waits 3-7 seconds.
        SpawnAmmo();
    }
}
