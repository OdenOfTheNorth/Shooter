using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Grapple : Ability
{
    public Image image;
    private LineRenderer lr;
    public LayerMask grappleble;
    
    public float spring = 4.5f;
    public float damper = 7f;
    public float massScale = 4.5f;
    
    private Rigidbody rigidbody;
    private SpringJoint joint;
    private ConfigurableJoint rigidBodyJoint;
    private RaycastHit hit; 
    
    private Vector3 grapplePoint;
    public float maxDistance;
    public float pullForce = 10f;
    public Transform gun, camera, player;

    public bool isGrappeling = false;
    private bool canGrappeling = false;
    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>();

        if (!lr)
        {
            lr = gameObject.AddComponent<LineRenderer>();
        }
        lr.positionCount = 0;

    }

    void CheckRaycast()
    {
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, grappleble))
        {
            print(hit.transform.name);
            canGrappeling = true;
            if (image)
            {
                image.color = Color.green;      //canGrappleColor
            }
        }
        else
        {
            canGrappeling = false;
            if (image)
            {
                image.color = Color.red;      //canGrappleColor
            }
        }
    }

    public void Update()
    {
        CheckRaycast();

        if (abilityInput && !isGrappeling)
        {
            if (canGrappeling)
            {
                StartGrapple();
            }
        }
        else if (abilityInput && isGrappeling)
        {
            EndGrapple();
        }

        if (isGrappeling)
        {
            if (Input.GetKeyDown(cancel))
            {
                EndGrapple();
            }
            rigidbody.AddForce((grapplePoint - transform.position).normalized * pullForce, ForceMode.Acceleration);
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()//RaycastHit hit
    {

        RaycastHit outHit;
        
        if (Physics.Raycast(camera.position, camera.forward,out outHit, maxDistance, grappleble))
        {
            print("start Grapple");
            isGrappeling = true;
            // add SpringJoint

            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
        
            grapplePoint = outHit.point;
            joint.connectedAnchor = grapplePoint;

            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            playerMovement.ShouldIncreaseGravity = false;
            playerMovement.isGrappeling = true;
            
            //playerMovement.ResetDownwardsVel();
        
            // Distances
            float distance = Vector3.Distance(player.position, grapplePoint);
            joint.maxDistance = distance * 0.8f;
            joint.minDistance = distance * 0.2f;

            joint.spring = spring;
            joint.damper = damper;
            joint.massScale = massScale;

            lr.positionCount = 2;
        }
        


    }
    void EndGrapple()
    {
        Destroy(joint);
        Destroy(rigidBodyJoint);
        print("end Grapple");
        isGrappeling = false;
        lr.positionCount = 0;

        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        playerMovement.isGrappeling = false;
        playerMovement.ShouldIncreaseGravity = true;
        playerMovement.currentGravityStrength = playerMovement.playerData.gravity;
        //playerMovement.ResetDownwardsVel();
    }

    void DrawRope()
    {
        if (!isGrappeling)
        {
            return;
        }
        lr.SetPosition(0,gun.position);
        lr.SetPosition(1,grapplePoint);
    }

    private void OnDrawGizmos()
    {
        if (!isGrappeling)
        {
            return;
        }
        
        Gizmos.DrawWireSphere(gun.position, 1.2f);
        Gizmos.DrawWireSphere(joint.anchor, 1.2f);
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
