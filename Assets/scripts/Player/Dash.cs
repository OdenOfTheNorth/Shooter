using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Ability
{
    public bool canDash = true;
    public float DashForce = 100f;
    public float DashDuration = 1.2f;

    private Rigidbody rigidbody;
    private Vector3 savedVelocity;

    private float lastDashTime = 0;
    private float DashCooldown = 5;
    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    
    public void Update()
    {
        if (abilityInput && canDash)
        {
            float time = Time.time;
            if (time - lastDashTime > DashCooldown)
            {
                lastDashTime = time;
                StartCoroutine(Cast());
            }
        }
    }

    public IEnumerator Cast()
    {
        print("start Dash");
        
        savedVelocity = rigidbody.velocity;
        
        rigidbody.AddForce(Camera.main.transform.forward * DashForce, ForceMode.Impulse);

        yield return new WaitForSeconds(DashDuration);
        
        rigidbody.velocity = savedVelocity;
    }
}
