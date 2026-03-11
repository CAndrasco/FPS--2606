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
        int rand = Random.Range(0, spawnPoints.Length);

        Instantiate(ammoPrefab, spawnPoints[rand].position, Quaternion.identity);

        currentAmmo++;
    }

    public void AmmoPickedUp()
    {
        currentAmmo--;

        if(currentAmmo < maxAmmoOnMap)
        {
            SpawnAmmo();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
