using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject gunModel;
    public Sprite gunIcon;

    [Range(1, 10)] public int shootDamage;
    [Range(1, 1000)] public int shootDist;
    [Range(0.1f, 2f)] public float shootRate;

    public int ammoCur;
    [Range(1, 50)] public int ammoMax;

    [Header("Fire Mode")]
    public bool automatic = false;
    [Range(1, 20)] public int pelletsPerShot = 1;
    [Range(0f, 10f)] public float spread = 0f;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    [Range(0, 1)] public float shootSoundVol;
}