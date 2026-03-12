using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    enum damageType { bullet, stationary, DOT}
    [SerializeField] damageType Type;
    [SerializeField] Rigidbody rb;
    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] ParticleSystem hitEffect;

    bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(Type == damageType.bullet)
        {
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        IDamage dmg = other.GetComponent<IDamage>();

        if(dmg != null && Type != damageType.DOT)
        {
            dmg.TakeDamage(damageAmount);
        }

        if(Type == damageType.bullet)
        {
            if(hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.isTrigger) return;
        IDamage dmg = other.GetComponentInParent<IDamage>();

        if(dmg != null && Type == damageType.DOT && !isDamaging)
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
