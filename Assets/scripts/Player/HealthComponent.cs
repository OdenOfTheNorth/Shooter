using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public float HealthPoints = 10;

    private GameObject LastDamageDealer;

    public void TakeDamage(float damageAmount)
    {
        HealthPoints -= damageAmount;

        if (HealthPoints <= 0)
        {
            //do something
        }
        
        return;
    }
    
    public void TakeDamage(float damageAmount, GameObject damageDealer)
    {
        HealthPoints -= damageAmount;
        LastDamageDealer = damageDealer;
        if (HealthPoints <= 0)
        {
            //do something
        }
        
        return;
    }
}
