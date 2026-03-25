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

    void Start()
    {
        // Safety check
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (Type == damageType.bullet)
        {
            // Moves bullet forward
            rb.linearVelocity = transform.forward * speed;

            // Makes bullet face direction it's moving
            transform.forward = rb.linearVelocity.normalized;

            // Prevent missed collision.
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            // No gravity for bullets
            rb.useGravity = false;

            // slight randomness for more natural feel. Not sure if I want to use or not.
            // rb.linearVelocity += Random.insideUnitSphere * 0.5f;

            // Destroy after time (failsafe)
            Destroy(gameObject, destroyTime);
        }

        if (Type == damageType.shockwave)
        {
            // Moves forward like a wave
            rb.linearVelocity = transform.forward * speed;

            // Destroy after time
            Destroy(gameObject, destroyTime);
        }
    }

    // Uses collision for reliable hits, not trigger like before.
    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;

        // Looks for anything that can take damage
        IDamage dmg = other.GetComponentInParent<IDamage>();

        if (dmg != null)
        {
            // Apply damage
            dmg.TakeDamage(damageAmount);

            // Only spawn blood on enemies.
            if (other.CompareTag("Enemy") && hitEffect != null && collision.contacts.Length > 0)
            {
                ContactPoint contact = collision.contacts[0];

                // Rotates effect so it sprays outward from hit surface
                Quaternion rot = Quaternion.LookRotation(contact.normal);

                Instantiate(hitEffect, contact.point, rot);
            }
        }

        // Destroy bullet/shockwave on impact.
        if (Type == damageType.bullet || Type == damageType.shockwave)
        {
            Destroy(gameObject);
        }
    }
}