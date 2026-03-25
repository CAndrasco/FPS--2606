using UnityEngine;

public class playerGunHolder : MonoBehaviour
{
    [SerializeField] Transform gunAnchor;

    [Header("Gun Prefabs")]
    [SerializeField] GameObject pistol;
    [SerializeField] GameObject shotGun;
    [SerializeField] GameObject machineGun;

    GameObject currentGun;

    void Start()
    {
     
    }

    public void EquipPistol()
    {
        EquipGun(pistol);
    }

    public void EquipShotGun()
    {
        EquipGun(shotGun);
    }

    public void EquipMachineGun()
    {
        EquipGun(machineGun);
    }

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

      
    }

   
}