using UnityEngine;

public class playerGunHolder : MonoBehaviour
{
    [SerializeField] Transform gunAnchor;
    GameObject currentGun;

    public void EquipGun(GameObject equippedGunPrefab)
    {
        if (gunAnchor == null)
        {
            Debug.LogError("Gun Anchor is not assigned on " + gameObject.name);
            return;
        }

        if (equippedGunPrefab == null)
        {
            Debug.LogError("Equipped gun prefab is null.");
            return;
        }

        if (currentGun != null)
        {
            Destroy(currentGun);
        }

        currentGun = Instantiate(equippedGunPrefab, gunAnchor);

        currentGun.transform.localPosition = Vector3.zero;
        currentGun.transform.localRotation = Quaternion.identity;

        DisableGunPhysics(currentGun);
    }

    void DisableGunPhysics(GameObject gun)
    {
        Rigidbody[] rigidbodies = gun.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Collider[] colliders = gun.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
    }
}