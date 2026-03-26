using UnityEngine;
using UnityEngine.Rendering;

public interface IPickup
{
    public void getGunStats(gunStats gun);
    public void addHeal(int amount);
}

