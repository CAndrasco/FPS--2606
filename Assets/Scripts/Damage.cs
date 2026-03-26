using UnityEngine;

public class Damage : MonoBehaviour
{
    enum damageType { bullet, stationary, shockwave }

    [SerializeField] damageType Type;
    [SerializeField] Rigidbody rb;
    [SerializeField] int damageAmount = 10;
    [SerializeField] int speed = 200;
    [SerializeField] int destroyTime = 5;
    [SerializeField] ParticleSystem hitEffect;

    bool hasHit = false;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (Type == damageType.bullet)
        {
            rb.linearVelocity = transform.forward * speed;
            transform.forward = rb.linearVelocity.normalized;

            rb.useGravity = false;

            // destroy after time
            Destroy(gameObject, destroyTime);
        }

        if (Type == damageType.shockwave)
        {
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        IDamage dmg = other.GetComponentInParent<IDamage>();

        if (dmg != null)
        {
            hasHit = true;

            // apply damage
            dmg.TakeDamage(damageAmount);

            // spawn hit effect on enemies
            if (other.CompareTag("Enemy") && hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}