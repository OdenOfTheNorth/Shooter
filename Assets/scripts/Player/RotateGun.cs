using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RotateGun : MonoBehaviour
{
    public Grapple Grapple;

    private Quaternion desiredRotaion;
    private float rotationSpeed = 10;
    private Transform Transform;
    
    void FixedUpdate()
    {
        Transform = transform;
        
        if (!Grapple.isGrappeling)
        {
            desiredRotaion = transform.parent.rotation;
        }
        else
        {
            desiredRotaion = Quaternion.LookRotation(Grapple.GetGrapplePoint() - Transform.position);
        }
        
        Transform.rotation = Quaternion.Lerp(Transform.rotation, desiredRotaion, Time.fixedDeltaTime * rotationSpeed);
    }
}
