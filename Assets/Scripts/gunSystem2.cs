using UnityEngine;

public class gunSystem2 : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform shootPosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab is missing on " + gameObject.name);
            return;
        }

        if (shootPosition == null)
        {
            Debug.LogError("Shoot Position is missing on " + gameObject.name);
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, shootPosition.position, shootPosition.rotation);
        Debug.Log("Spawned bullet: " + bullet.name);
    }
}
