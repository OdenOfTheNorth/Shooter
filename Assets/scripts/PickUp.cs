using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PickUp : Ability
{
    public float SpringForce = 3;
    public float SpringDamper = 3;
    public float PickUpRange = 3;
    public bool IsHoldingObject = false;
    public Rigidbody body;
    public string PickUpTag;
    private Transform camTrans;
    private RaycastHit[] hits;
    private RaycastHit hit;

    void Start()
    {
        camTrans = Camera.main.transform;
    }

    void Update()
    {
        if (abilityInput)
        {
            Raycast();
        }
    }

    private void FixedUpdate()
    {
        PickUpObject();
    }

    void Raycast()
    {
        print("Ray");
        
        if (IsHoldingObject)
        {
            IsHoldingObject = false;
            body.useGravity = true;
            body = null;
            return;
        }
        
        //Physics.RaycastAll()
        hits = Physics.RaycastAll(transform.position, camTrans.forward, PickUpRange);

        foreach (var tempHit in hits)
        {
            if (tempHit.collider != null)
            {
                if (tempHit.collider.isTrigger)
                {
                    continue;
                }
                
                print(tempHit.collider.name);
            
                if (tempHit.collider.CompareTag(PickUpTag))
                {
                    body = tempHit.collider.GetComponent<Rigidbody>();
                    IsHoldingObject = true;
                    hit = tempHit;
                    return;
                }
            }
        }
    }

    void PickUpObject()
    {
        if (body)
        {
            Vector3 desPos = transform.position + (camTrans.forward * PickUpRange);
            Vector3 dir = (body.position - desPos);
            //body.AddForce(dir.normalized * 10, ForceMode.Acceleration);
            BodyUprightForce.UprightForce(body, body.transform, 100, 10);
            
            Vector3 vel = body.velocity;
            Vector3 otherVel = Vector3.zero;
            
            float rayDirVel = Vector3.Dot(dir.normalized, vel);
            float otherDirVel = Vector3.Dot(dir.normalized, otherVel);

            float relVel = rayDirVel - otherDirVel;

            float x = -10;// hit.distance - PickUpRange;
            float springForce = (x * SpringForce) - (relVel * SpringDamper);
            body.useGravity = false;
            body.AddForce(dir.normalized * springForce);

            //BodyUprightForce.UprightForce(body, body.transform, 10, 10);

            //body.angularVelocity = Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        if (IsHoldingObject)
        {
            Gizmos.DrawLine(body.position, body.position + body.velocity.normalized);
        }
    }
}
