using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Ability
{
    public Camera camera;
    public float Range = 1.2f;
    public float Damage = 3f;
    public float KnockBack = 45f;
    
    public float coolDown = 1f;
    public float lastMeleeTime = 1f;

    private void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        if (abilityInput && Time.time - lastMeleeTime > coolDown)
        {
            MeleeAttack();
        }
    }

    public void MeleeAttack()
    {
        Vector3 pos = transform.position;
        Physics.Raycast(pos, camera.transform.forward, out RaycastHit hit, Range);
        
        if (hit.collider != null)
        {
            Rigidbody triggerBody = hit.transform.GetComponent<Rigidbody>();
            if (triggerBody)
            {
                
                
                triggerBody.AddForceAtPosition( camera.transform.forward * KnockBack, hit.point,ForceMode.Impulse);    
            }
            
            HealthComponent Health =  hit.transform.GetComponent<HealthComponent>();
            if (Health)
            {
                Health.TakeDamage(Damage);
            }
        }
    }
}
