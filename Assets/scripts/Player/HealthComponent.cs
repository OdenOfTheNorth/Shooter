using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public float HealthPoints = 10;

    private GameObject LastDamageDealer;

    public Death dead;

    private void Start()
    {
        dead = GetComponent<Death>();
    }

    public void TakeDamage(float damageAmount)
    {
        HealthPoints -= damageAmount;

        print(HealthPoints);
        
        if (HealthPoints <= 0)
        {
            dead.Call();
            //do something
        }
        
        return;
    }
    
    public void TakeDamage(float damageAmount, GameObject damageDealer)
    {
        HealthPoints -= damageAmount;
        LastDamageDealer = damageDealer;
        
        print(HealthPoints);
        
        if (HealthPoints <= 0)
        {
            dead.Call();
            //do something
        }
        
        return;
    }
}
