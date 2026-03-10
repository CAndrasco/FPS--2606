using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI_1 : MonoBehaviour, IDamage
{
    [Header("---- Unity Components ----")]
    [SerializeField] Renderer model;

    [Header("---- Enemy Settings ----")]
    [SerializeField] int HP;
    //[SerializeField] GameObject Weapon;
    //[SerializeField] float damageRate;
    [SerializeField] Transform armHittingPos1;
    [SerializeField] Transform armHittingPos2;
    [SerializeField] Transform armPivot1;
    [SerializeField] Transform armPivot2;

    Color colorOrig;

    float damageTimer;

    void Start()
    {
        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //void Shoot() -- Not needed at the moment
    //{
    //    damageTimer = 0;
    //    Instantiate(Weapon, armHittingPos1.position, armPivot1.rotation);
    //}

    public void TakeDamage(int damage)
    {
        HP -= damage;
        if(HP <= 0)
        {
            gamemanager.instance.updateGameGoal(-1);
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
