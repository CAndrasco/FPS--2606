using UnityEngine;

public class ammoGlowPulse : MonoBehaviour
{
    [SerializeField] Renderer[] rends;
    [SerializeField] float pulseSpeed = 2f;
    [SerializeField] float minIntensity = 0.5f;
    [SerializeField] float maxIntensity = 1.5f;
    [SerializeField] Color emissionColor = Color.yellow;

    Material[] mats;

    void Start()
    {
        mats = new Material[rends.Length];

        for (int i = 0; i < rends.Length; i++)
        {
            mats[i] = rends[i].material;
        }
    }

    void Update()
    {
        float emission = Mathf.Lerp(minIntensity, maxIntensity,
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);

        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].SetColor("_EmissionColor", emissionColor * emission);
        }
    }
}