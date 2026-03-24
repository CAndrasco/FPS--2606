using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    enum damageType { bullet, stationary, DOT, shockwave }

    [SerializeField] damageType Type;
    [SerializeField] Rigidbody rb;
    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] ParticleSystem hitEffect;

    bool isDamaging;
    bool hasHit = false;

    void Start()
    {
        if (Type == damageType.bullet)
        {
            // Shoot bullet forward
            rb.linearVelocity = transform.forward * speed;

            // Prevent tunneling through objects
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            // Destroy bullet after time
            Destroy(gameObject, destroyTime);
        }
        if(Type == damageType.shockwave)
        {
            // shoots forward
            rb.linearVelocity = transform.forward * speed;

            //I want it to tunnel through object so no collision detection
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        IDamage dmg = other.GetComponentInParent<IDamage>();

        if (dmg != null && Type != damageType.DOT && !hasHit)
        {
            hasHit = true;
            dmg.TakeDamage(damageAmount);
        }

        if (Type == damageType.bullet)
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
        if (Type == damageType.shockwave)
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) return;

        IDamage dmg = other.GetComponentInParent<IDamage>();

        if (dmg != null && Type == damageType.DOT && !isDamaging)
        {
            StartCoroutine(DamageOther(dmg));
        }
    }

    IEnumerator DamageOther(IDamage dmg)
    {
        isDamaging = true;
        dmg.TakeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}