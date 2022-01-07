using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float jumpForce = 100;
    public Vector3 jumpDir = Vector3.up;
    
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody body = other.gameObject.GetComponent<Rigidbody>();

        Vector3 up = other.gameObject.transform.up;//cachedtransform.forward;
        Vector3 antiGravity = Vector3.Dot(body.velocity,up) * up;
        
        if (body)
        {
            //body.velocity = 
            body.AddForce(-antiGravity + jumpDir * jumpForce, ForceMode.Impulse);
        }
    }
}
