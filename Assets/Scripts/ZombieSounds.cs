using UnityEngine;

public class ZombieSounds : MonoBehaviour
{
    [Header("---- Audio ----")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] zombieSounds;

    [Header("---- Timing ----")]
    [SerializeField] Vector2 soundCooldownRange = new Vector2(2f, 5f);

    float soundTimer;
    float currentCooldown;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        currentCooldown = Random.Range(soundCooldownRange.x, soundCooldownRange.y);
    }

    void Update()
    {
        soundTimer += Time.deltaTime;

        if (soundTimer >= currentCooldown && !audioSource.isPlaying)
        {
            PlayZombieSound();

            soundTimer = 0f;
            currentCooldown = Random.Range(soundCooldownRange.x, soundCooldownRange.y);
        }
    }

    void PlayZombieSound()
    {
        if (zombieSounds.Length == 0)
            return;

        int index = Random.Range(0, zombieSounds.Length);

        //makes the audio louder and more varied.
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.volume = Random.Range(2.8f, 5.5f);

        audioSource.PlayOneShot(zombieSounds[index]);
    }
}