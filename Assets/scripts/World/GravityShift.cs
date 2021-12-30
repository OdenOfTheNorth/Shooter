using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityShift : MonoBehaviour
{
    public Transform directionObject;

    private void OnTriggerEnter(Collider other)
    {
        Vector3 dir = directionObject.position - transform.position;

        PlayerMovement movement = other.GetComponent<PlayerMovement>();

        if (movement != null)
        {
            movement.GravityDirection = dir.normalized;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(directionObject.position, transform.position);
    }
}
