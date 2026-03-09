using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI_1 : MonoBehaviour, IDamage
{
    [Header("---- Unity Components ----")]
    [SerializeField] Renderer model;

    [Header("---- Enemy Settings ----")]
    [SerializeField] int HP;


    Color colorOrig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        if(HP <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    //just to check enemies are taking damage wont be needed in final build
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
