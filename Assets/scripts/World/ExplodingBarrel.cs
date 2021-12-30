using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExplodingBarrel : MonoBehaviour
{
    private SphereCollider Collider;


    private List<Rigidbody> bodies;
    
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody triggerBody = other.gameObject.GetComponent<Rigidbody>();

        if (!triggerBody)
        {
            return;
        }
        
        foreach (Rigidbody body in bodies)
        {
            if (body != triggerBody)
            {
                continue;
            }
            else
            {
                
            }
        }
            
        bodies.Add(triggerBody);
    }
}
