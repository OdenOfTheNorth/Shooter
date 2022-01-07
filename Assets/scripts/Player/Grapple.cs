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
            image.color = Color.green;      //canGrappleColor
            canGrappeling = true;
        }
        else
        {
            canGrappeling = false;
            image.color = Color.red;   //canNotGrappleColor
        }
    }

    public void Update()
    {
        CheckRaycast();

        if (abilityInput && !isGrappeling)
        {
            if (canGrappeling)
            {
                StartGrapple(hit);
            }
        }
        else if (abilityInput && isGrappeling)
        {
            EndGrapple();
        }

        if (isGrappeling)
        {
            rigidbody.AddForce(Camera.main.transform.forward * pullForce, ForceMode.Acceleration);
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple(RaycastHit hit)
    {
        print("start Grapple");
        isGrappeling = true;
        // add SpringJoint
        grapplePoint = hit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;
        // Distances
        float distance = Vector3.Distance(player.position, grapplePoint);
        joint.maxDistance = distance * 0.8f;
        joint.minDistance = distance * 0.2f;

        joint.spring = spring;
        joint.damper = damper;
        joint.massScale = massScale;

        lr.positionCount = 2;
    }
    void EndGrapple()
    {
        Destroy(joint);
        print("end Grapple");
        isGrappeling = false;
        lr.positionCount = 0;
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

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
