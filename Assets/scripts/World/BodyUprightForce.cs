using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyUprightForce : MonoBehaviour
{
    private Vector3 GravityDirection = Vector3.down;
    private Rigidbody rigidbody;
    private Transform trans;
    [Header("Spring like Physics")]
    public float spring = 100;
    public float Damper = 10;

    private void Start()
    {
        trans = transform;
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        UprightForce();
    }

    public void UprightForce()
    {
        Quaternion characterCurrent = trans.rotation;
        //Quaternion toGoal = transform.rotation;
        Quaternion toGoal = Quaternion.FromToRotation(trans.up, -GravityDirection);// * characterCurrent;

        Vector3 rotAxis;
        float rotDegrees;
        
        toGoal.ToAngleAxis(out rotDegrees, out rotAxis);

        float rotRadians = rotDegrees * Mathf.Deg2Rad;
        rigidbody.AddTorque((rotAxis * (rotRadians * spring)) - (rigidbody.angularVelocity * Damper));
    }
    
    public static void UprightForce(Rigidbody rb, Transform transform, float spring, float Damper)
    {
        Quaternion characterCurrent = transform.rotation;
        //Quaternion toGoal = transform.rotation;
        Quaternion toGoal = Quaternion.FromToRotation(transform.up, Vector3.up);// * characterCurrent;

        Vector3 rotAxis;
        float rotDegrees;
        
        toGoal.ToAngleAxis(out rotDegrees, out rotAxis);

        float rotRadians = rotDegrees * Mathf.Deg2Rad;
        rb.AddTorque((rotAxis * (rotRadians * spring)) - (rb.angularVelocity * Damper));
    }
}
