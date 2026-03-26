using UnityEngine;

public class gunPickup : MonoBehaviour
{
    [SerializeField] gunStats gun;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pik = other.GetComponent<IPickup>();

        if (pik != null)
        {
            // reset ammo when picked up so player gets full ammo
            gun.ammoCur = gun.ammoMax;

            // add gun to player's inventory
            pik.getGunStats(gun);

            // remove pickup from world
            Destroy(gameObject);
        }
    }
}