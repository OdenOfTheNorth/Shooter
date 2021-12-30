using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody) )]
public class PlayerMovement : MonoBehaviour
{
    public float forwardInput;
    public float rightInput;
    public bool runInput;
    public bool crouchInput;
    public bool jumpInput;
    public bool OnWall = false;
    public bool OnGround = false;
    public bool OnSlope = false;
    public bool ShouldApplyGravity = true;
    public bool jumping = false;

    public float angle;
    
    public PlayerData playerData;
    public int CurrnetJumpCount = 0;
    //public float RotationSpeed = 10f;
    public float currentSpeed = 10f;
    public float currentCounterForce = 10f;
    public LayerMask GroundLayer;
    public LayerMask WallLayer;

    public float sphereRadius;
    
    [HideInInspector] float lastTimeJumped = 0f;
    [HideInInspector] const float jumpGroundingPreventionTime = 0.2f;

    [SerializeField] private float maxDistance = 1.2f;
    [HideInInspector] public float maxWallDistance = 1.2f;
    
    private Vector3 SphereLoc;
    private Vector3 GroundMovement;
    private Vector3 SlopeMovement;
    private RaycastHit slopeHit;
    private RaycastHit WallHit;
    
    public Vector3 velocity;
    public Vector3 GravityDirection;
    [HideInInspector] public Vector3 forwardWallVector;
    [HideInInspector] public Vector3 gravityUp;
    [HideInInspector] public Vector3 cameraForward;
    [HideInInspector] public Vector3 rightVector;
    [HideInInspector] public Vector3 forwardVector;
    
    public Transform cachedtransform;
    [HideInInspector] public Transform cameraTransform;
    [HideInInspector] public Rigidbody rigidbody;
    
    private CapsuleCollider _capsuleCollider;
    [HideInInspector] public int layerMask;
    void Start()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();
        sphereRadius = _capsuleCollider.radius * 0.9f;
        rigidbody = GetComponent<Rigidbody>();

        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        
        currentSpeed = playerData.WalkSpeed;
        cameraTransform = Camera.main.transform;
        cachedtransform = transform;
        currentCounterForce = playerData.GroundCounterForce;
    }

    private void RotationTowardsGravityDirection()
    {
        Vector3 gravityUp = -GravityDirection.normalized;
        Quaternion targetRoation = Quaternion.FromToRotation(cachedtransform.up, gravityUp) * cachedtransform.rotation;
        cachedtransform.rotation = Quaternion.Slerp(cachedtransform.rotation, targetRoation, playerData.RotationSpeed * Time.fixedDeltaTime);
    }

    private void CalculateMovement()
    {
        //Move Direction
        gravityUp = -GravityDirection.normalized;
        cameraForward = Vector3.Cross(gravityUp,cameraTransform.right);
        rightVector = Vector3.Cross(gravityUp, cameraForward);
        forwardVector = Vector3.Cross(gravityUp, rightVector);
        GroundMovement = (rightVector * -rightInput + forwardVector * forwardInput).normalized;
    }

    private void Movement()
    {
        float deltaTime = Time.deltaTime;
        
        if (ShouldApplyGravity)
        {
            rigidbody.AddForce(GravityDirection.normalized * playerData.gravity, ForceMode.Acceleration);    
        }
        if (OnGround && !OnSlope)
        {
            velocity = (GroundMovement * currentSpeed);// + counterMovement(rigidbody.velocity);
            Vector3 deltaToMove = velocity * deltaTime;
            //rigidbody.AddForce(velocity, ForceMode.Acceleration);
        }
        else if (OnGround && OnSlope)
        {
            SlopeMovement = Vector3.ProjectOnPlane(GroundMovement, GravityDirection);
            velocity = (SlopeMovement * currentSpeed);// + counterMovement(rigidbody.velocity);
            Vector3 deltaToMove = velocity * deltaTime;
            //rigidbody.AddForce(velocity , ForceMode.Acceleration);
        }
        else if (!OnGround)
        {
            velocity = (GroundMovement * (currentSpeed * playerData.AirControll));// + counterMovement(rigidbody.velocity);
            //rigidbody.AddForce(velocity , ForceMode.Acceleration);  
        }

        Vector3 counterForce = (OnGround ? counterMovement(rigidbody.velocity) : counterMovement(rigidbody.velocity));
        
        rigidbody.AddForce(velocity + counterMovement(rigidbody.velocity), ForceMode.Acceleration);//

    }

    private void Update()
    {
        OnGround = CheckGrounded();
        OnSlope = (OnGround == true ? CheckSlope() : false);
        //OnWall = CheckWall();    
        
        SetCurrentSpeed();

        Jump();
        
        layerMask = 1 << GroundLayer;
        layerMask = ~layerMask;
        //print(rigidbody.velocity.magnitude);
    }

    public void Jump()
    {
        if (CanJump())
        {
            jumping = true;
            CurrnetJumpCount++;
            rigidbody.AddForce(GravityDirection.normalized * -playerData.jumpStrength, ForceMode.Impulse);    
        }
    }

    private void FixedUpdate()
    {
        CalculateMovement();
        Movement();
        RotationTowardsGravityDirection();
    }

    public Vector3 counterMovement(Vector3 Vel)
    {
        Vector3 right = Vector3.ProjectOnPlane(cachedtransform.right, GravityDirection);
        Vector3 forward = Vector3.ProjectOnPlane(cachedtransform.forward, GravityDirection);
        
        Vector3 counterForwardVelocity = Vector3.Dot(Vel,right) * right;
        Vector3 counterRightVelocity = Vector3.Dot(Vel,forward) * forward;
        Vector3 counterVelocity = counterForwardVelocity + counterRightVelocity;// rigidbody.velocity * -rigidbody.velocity.magnitude;
        
        /*
        float MaxCounterForce = 10 * 10;
        float currentPercentingmagnetude = (currentCounterForce * currentCounterForce) / Vel.sqrMagnitude;
        if (currentPercentingmagnetude > 0.9)        {                    }        
        float deltaCounterForce = (-currentCounterForce + Vel.magnitude);
        deltaCounterForce = Mathf.Clamp(deltaCounterForce, -currentCounterForce, -3);
        */
            
        return counterVelocity * -currentCounterForce;
    }

    private void OnDrawGizmos()
    {
        if (_capsuleCollider != null || cachedtransform != null)
        {
            //Gizmos.color = Color.red;
            //Gizmos.DrawSphere(SphereLoc, _capsuleCollider.radius);
            //Gizmos.color = Color.green;
            //Gizmos.DrawSphere(cachedtransform.position + (GravityDirection * maxDistance), _capsuleCollider.radius);
        }
    }

    public void Landed()
    {
        jumping = false;
        CurrnetJumpCount = 0;
        currentCounterForce = playerData.GroundCounterForce;
    }

    private bool CheckGrounded()
    {
        Vector3 pos = cachedtransform.position + (GravityDirection * maxDistance);
       
        //Physics.Raycast(pos, directions[i], out hit[i], maxWallDistance, layerMask);RaycastHit spahereHit;

        if (Physics.CheckSphere(pos, sphereRadius, GroundLayer))//
        {
            if (!OnGround)
            {
                Landed();
            }

            //print("Sphere Hit");
            return true;
        }
        else
        {
            currentCounterForce = playerData.AirCounterForce;
            //print("Sphere not Hit");
            //OnGround = false;
            return false;
        }
        
        return false;
    }
    
    private bool CheckSlope()
    {
        Vector3 pos = cachedtransform.position;
        Debug.DrawRay(pos + _capsuleCollider.center, GravityDirection * maxDistance, Color.red);
        
        RaycastHit slope;
        Physics.Raycast(pos + _capsuleCollider.center, GravityDirection, out slope,_capsuleCollider.height + maxDistance,layerMask);

        if (slope.collider == null)
        {
            return false;
        }
        
        angle = Mathf.Abs(Vector3.Angle(GravityDirection, slope.normal));

        if (angle < 180)
        {
            if (OnGround && !jumping)
            {
                rigidbody.AddForce(slope.normal * -playerData.gravity);    
            }

            slopeHit = slope;
            return true;
        }
        else
        {
            slopeHit = slope;
            return false;
        }
        
        if (slope.collider != null && slope.normal != GravityDirection)
        {
            //currentCounterForce = playerData.SlideCounterForce;
            slopeHit = slope;
            return true;
        }
        else if (slope.collider == null || slope.normal == GravityDirection)
        {
            slopeHit = slope;
            return false;
        }

        return false;
    }

    void SetCurrentSpeed()
    {
        if (runInput)
        {
            currentSpeed = playerData.RunSpeed;
        }
        else// if (crouchInput)
        {
            currentSpeed = playerData.WalkSpeed;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        float dotAngle = Vector3.Dot(forwardVector, other.contacts[0].normal);

        print(dotAngle);
    }

    void LockOnWall(Vector3 Wallnormal)
    {
        transform.position = cachedtransform.position + (Wallnormal * (_capsuleCollider.radius * 0.5f));
    }

    void LockOnGround(Vector3 groundLevel)
    {
        transform.position = groundLevel + (GravityDirection * (-_capsuleCollider.height * 0.5f));
    }

    bool CanJump()
    {
        //print(CurrnetJumpCount + " " + playerData.MaxJumpCount );
        return (OnGround || CurrnetJumpCount < playerData.MaxJumpCount || OnSlope) && jumpInput;
    }
    
    /*
    public void EndWallRun()
    {   //CurrnetJumpCount++;
        ShouldApplyGravity = true;
        //print("NotOnWall");
    }
    
    private bool CheckWall()
    {
        Vector3 pos = cachedtransform.position;
        Vector3 right = cachedtransform.right;
        Vector3 forward = cachedtransform.forward;
        
        Vector3[] directions =
        {
            //forward,
            right,
            //(right + forward).normalized,
            //(right - forward).normalized,
            //-forward,
            -right,
            //(-right + forward).normalized,
            //(-right - forward).normalized
        };

        RaycastHit[] hit = new RaycastHit[directions.Length]; 
        
        //int layerMask = 1 << WallLayer;
        //layerMask = ~layerMask;
        
        for (int i = 0; i < directions.Length; i++)
        {
            Physics.Raycast(pos, directions[i], out hit[i], maxWallDistance, layerMask);
            Debug.DrawRay(pos, directions[i] * maxWallDistance, Color.red);
           
            if (hit[i].collider != null)
            {
                //print("OnWall");
                
                StartWallRun(hit[i].normal);
                return true;
            }
        }
        EndWallRun();
        return false;
    }

    public void StartWallRun(Vector3 WallNormal)
    {
        if ((forwardInput == 0 || rightInput == 0) && OnGround)
        {
            return;
        }

        //float angle = Vector3.Dot(forwardVector, WallNormal);
        //if (angle < 0.0f) { return; }
        
        currentCounterForce = playerData.GroundCounterForce;

        CurrnetJumpCount = 0;
        //
        if (ShouldApplyGravity)
        {
            ShouldApplyGravity = false;
            rigidbody.AddForce(GravityDirection.normalized * -(playerData.gravity), ForceMode.Acceleration);
        }

        print("OnWall");
        
        if (!OnWall)
        {
            forwardWallVector = Vector3.ProjectOnPlane(forwardVector, WallNormal).normalized * currentSpeed;
        }
        
        Debug.DrawRay(cachedtransform.position,forwardWallVector);
        
        rigidbody.AddForce(-WallNormal, ForceMode.Force);
        rigidbody.AddForce(forwardWallVector + counterMovement(rigidbody.velocity), ForceMode.Force);
        
        if (jumpInput)
        {
            Vector3 wallJump = (WallNormal * playerData.WallJumpStrength) - (GravityDirection * playerData.jumpStrength);//+ ( * -playerData.jumpStrength);
            rigidbody.AddForce(wallJump, ForceMode.Impulse);
            EndWallRun();
        }
    }*/
}
