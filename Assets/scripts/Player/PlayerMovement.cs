using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody) )]
public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    public float forwardInput;
    public float rightInput;
    public bool runInput;
    public bool crouchInput;
    public bool jumpInput;
    [Header("States")]
    public bool OnWall = false;
    [HideInInspector] public bool[] OnWallSides = new bool[2];
    //[HideInInspector] public bool OnLeftWall = false;
    public bool OnGround = false;
    public bool OnSlope = false;
    public bool ShouldApplyGravity = true;
    public bool jumping = false;

    public float angle;
    
    public PlayerData playerData;
    public int CurrnetJumpCount = 0;
    public float currentSpeed = 10f;
    public float currentCounterForce = 10f;
    public LayerMask GroundLayer;
    
    [Header("Wall")]
    public LayerMask WallLayer;
    [Range(0, 1)] public float maxAngle = 1;
    [Range(-1, 0)] public float minAngle = -1;
    private Vector3 lastWallNormal;
    
    public float sphereRadius;
    
    [HideInInspector] float lastTimeJumped = 0f;
    [HideInInspector] const float jumpGroundingPreventionTime = 0.2f;

    [Header("Ground")]
    [SerializeField] private float maxDistance = 1.2f;
    [HideInInspector] public float maxWallDistance = 1.2f;
    
    private Vector3 GroundMovement;
    private Vector3 SlopeMovement;
    
    private RaycastHit GroundHit;
    private RaycastHit slopeHit;
    private RaycastHit WallHit;
    [Header("Velocity and Gravity")]
    public Vector3 velocity;
    public Vector3 GravityDirection;
    public float maxVelocity = 10;
    [Range(0, 1)] public float maxVelocityThreshold = 0.95f;
    public float currentPercentige = 0;
    
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
        OnWall = CheckWall();    
        
        SetCurrentSpeed();

        Jump();
        
        //layerMask = 1 << GroundLayer;
        //layerMask = ~layerMask;
        //print(rigidbody.velocity.magnitude);
    }

    public void Jump()
    {
        if (CanJump())
        {
            jumping = true;
            CurrnetJumpCount++;

            Vector3 up = cachedtransform.up;
            Vector3 counterUpVelocity = Vector3.Dot(rigidbody.velocity,up) * up;

            //rigidbody.velocity += counterUpVelocity;
            
            Vector3 jumpStrength = GravityDirection.normalized * -playerData.jumpStrength;
            
            rigidbody.AddForce(jumpStrength + counterUpVelocity, ForceMode.Impulse);    
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
        Vector3 right =  Vector3.ProjectOnPlane(cachedtransform.right, GravityDirection);//cachedtransform.right;
        Vector3 forward =  Vector3.ProjectOnPlane(cachedtransform.forward, GravityDirection);//cachedtransform.forward;
        //Vector3 up =  Vector3.ProjectOnPlane(cachedtransform.up, GravityDirection);//cachedtransform.forward;
        //Vector3 counterUpForce = Vector3.Dot(Vel,up) * up;
        Vector3 counterForwardVelocity = Vector3.Dot(Vel,right) * right;
        Vector3 counterRightVelocity = Vector3.Dot(Vel,forward) * forward;
        Vector3 counterVelocity = counterForwardVelocity + counterRightVelocity;// rigidbody.velocity * -rigidbody.velocity.magnitude;

        /*float rigidBodyVelocity = (rigidbody.velocity + counterUpForce).magnitude;
        currentPercentige = Mathf.Abs(rigidBodyVelocity / maxVelocity);
        if (currentPercentige > maxVelocityThreshold)
        {
            maxVelocity += 5 * Time.deltaTime;
        }        
        if (currentPercentige < 1 - maxVelocityThreshold)
        {
            maxVelocity -= 5 * Time.deltaTime;
        }
        maxVelocity = Mathf.Clamp(maxVelocity, 0, maxVelocity);
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxVelocity);*/
        
        return counterVelocity * -currentCounterForce;//-currentPercentige * rigidBodyVelocity;
    }

    private void OnDrawGizmos()
    {
        
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
        //Physics.Raycast(pos + _capsuleCollider.center, GravityDirection, out GroundHit,_capsuleCollider.height + maxDistance,layerMask);
        
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
            return false;
        }
        
        return false;
    }
    
    private bool CheckSlope()
    {
        Vector3 pos = cachedtransform.position;
        Debug.DrawRay(pos + (GravityDirection.normalized * _capsuleCollider.height / 2), GravityDirection.normalized, Color.blue);
        
        RaycastHit slope;
        Physics.Raycast(pos + _capsuleCollider.center, GravityDirection, out slope,_capsuleCollider.height + maxDistance,GroundLayer);//layerMask

        if (slope.collider == null)
        {
            return false;
        }
        
        angle = Mathf.Abs(Vector3.Angle(GravityDirection, slope.normal));

        if (angle < 180)
        {
            Debug.DrawLine(slope.point ,slope.point + slope.normal, Color.green);
            
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

        //&print(dotAngle);
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
        return (OnGround || CurrnetJumpCount < playerData.MaxJumpCount || OnSlope || OnWall) && jumpInput;
    }
    
    private bool CheckWall()
    {
        Vector3 pos = cachedtransform.position;
        Vector3 right = cachedtransform.right;
        Vector3 forward = cachedtransform.forward;
        
        Vector3[] directions =
        {
            forward,
            right,
            (right + forward).normalized,
            (right - forward).normalized,
            -forward,
            -right,
            (-right + forward).normalized,
            (-right - forward).normalized
        };

        RaycastHit[] hit = new RaycastHit[directions.Length]; 
        
        for (int i = 0; i < directions.Length; i++)
        {
            var ray = new Ray(pos, directions[i]);
            Physics.Raycast(pos, directions[i], out hit[i], maxWallDistance, WallLayer);
            
            //Physics.Raycast(pos + _capsuleCollider.center, GravityDirection, out slope,_capsuleCollider.height + maxDistance,GroundLayer);//layerMask
            
            Debug.DrawRay(pos, directions[i] * maxWallDistance, Color.red);
           
            if (hit[i].collider != null)
            {
                Debug.DrawRay(hit[i].point, hit[i].normal * maxWallDistance, Color.green);
                //print("OnWall");

                int half = Mathf.RoundToInt(directions.Length / 2);

                if (half > i)
                {
                    OnWallSides[0] = true; 
                }
                else
                {
                    OnWallSides[1] = true; 
                }
                
                //OnWallSides[half] = true;
                StartWallRun(hit[i]);
                return true;
            }
            else
            {
                int half = Mathf.RoundToInt(directions.Length / 2);
                
                if (half > i)
                {
                    OnWallSides[0] = false; 
                }
                else
                {
                    OnWallSides[1] = false; 
                }
            }
        }
        return false;
    }
    
    public void StartWallRun(RaycastHit hit)
    {
        Vector3 WallNormal = hit.normal;
        
        if (WallNormal == lastWallNormal)
        {
            EndWallRun(Vector3.zero);
            print("SameWall");
            return;
        }
        
        if ((forwardInput == 0 || rightInput == 0) && OnGround)
        {
            EndWallRun(Vector3.zero);
            print("no input");
            return;
        }

        float dotAngle = Vector3.Dot(forwardVector, WallNormal);

        print(dotAngle);

        if (dotAngle < maxAngle && dotAngle > minAngle)
        {
            //OnWall = true;
            CurrnetJumpCount = 0;
            forwardWallVector = (Vector3.ProjectOnPlane(forwardVector, WallNormal).normalized * currentSpeed * 0.5f);// + hit.transform.up * playerData.gravity;
            
            Vector3 up = Vector3.ProjectOnPlane(cachedtransform.up, GravityDirection);//cachedtransform.forward;
            Vector3 antiGravity = Vector3.Dot(rigidbody.velocity,up) * up;
            Vector3 forces = (-WallNormal * playerData.gravity) + antiGravity + forwardWallVector + counterMovement(rigidbody.velocity);

            rigidbody.AddForce(forces, ForceMode.Acceleration);
            
            if (jumpInput)
            {
                Vector3 wallJump = (WallNormal * playerData.WallJumpStrength) + antiGravity - (GravityDirection.normalized * playerData.jumpStrength);// //+ ( * -playerData.jumpStrength);
                rigidbody.AddForce(wallJump, ForceMode.Impulse);//Jump();
                EndWallRun(WallNormal);
            }
        }
    }
    
    public void EndWallRun(Vector3 WallNormal)
    {
        rigidbody.AddForce(WallNormal * playerData.gravity, ForceMode.Acceleration);

        lastWallNormal = WallNormal;
        ShouldApplyGravity = true;
        OnWall = false;
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
