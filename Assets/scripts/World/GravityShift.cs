using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityShift : MonoBehaviour
{
    public Transform directionObject;
    public Vector3 Direction;

    private void OnCollisionEnter(Collision other)
    {
        Vector3 dir;
        
        if (directionObject)
        {
            dir = directionObject.position - transform.position;    
        }
        else
        {
            dir = Direction;    
        }

        PlayerMovement movement = other.gameObject.GetComponent<PlayerMovement>();

        if (movement != null)
        {
            movement.GravityDirection = dir.normalized;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Vector3 dir;
        
        if (directionObject)
        {
            dir = directionObject.position - transform.position;    
        }
        else
        {
            dir = Direction;    
        }

        PlayerMovement movement = other.GetComponent<PlayerMovement>();

        if (movement != null)
        {
            movement.GravityDirection = dir.normalized;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        if (directionObject)
        {
            Gizmos.DrawLine(directionObject.position, transform.position);
        }
    }
}
