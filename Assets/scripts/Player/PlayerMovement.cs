using UnityEngine;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody) )]
public class PlayerMovement : MonoBehaviour
{
    public PlayerData playerData;

    [Header("Input")] 
    public float forwardInput;
    public float rightInput;
    public bool runInput;
    public bool crouchInput;
    public bool jumpInput;

    [Header("States")] 
    public bool OnWall = false;
    public bool isGrounded = false;
    public bool OnSlope = false;
    public bool isGrappeling = false;
    public bool isCrouching = false;
    public bool isSliding = false;
    public bool ShouldApplyGravity = true;
    public bool ShouldIncreaseGravity = true;
    public bool jumping = false;
    public float slopeAngle;
    
    [Header("Movement")]
    public int CurrnetJumpCount = 0;
    public float currentSpeed = 5f;
    public float acceliration = 10f;
    private float movementMultiplier = 10;
    [Header("Drag")]
    public float groundDrag = 6f;
    public float airDrag = 2f;
    public float grappelDrag = 0f;
    public float slideDrag = 0.2f;
    public float VelocityThreshold = 15f;
    public float Magnetude;
    
    private float currentCounterForce = 10f;

    [Header("Wall")]
    public float maxWallDistance = 1.2f;
    [SerializeField] public bool wallLeft = false;
    [SerializeField] public bool wallRight = false;
    
    [Header("Crouch & Slide")]
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale = Vector3.one;
    public float slideForce = 400;

    [Header("Camera")]
    public LayerMask WallLayer;
    [Range(0, 1)] public float maxAngle = 1;
    [Range(-1, 0)] public float minAngle = -1;
    private Vector3 lastWallNormal;

    [Header("Ground")]
    public LayerMask GroundLayer;
    [SerializeField] private float maxDistance = 1.2f;
    public float sphereRadius;
    public float lastFallTime;

    
    private RaycastHit GroundHit;
    private RaycastHit slopeHit;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private RaycastHit WallHit;
    
    [Header("Velocity and Gravity")]
    public Vector3 moveDirection;
    public Vector3 GravityDirection;
    public float currentGravityStrength = 10; 
    //Directions
    [HideInInspector] public Vector3 SlopeMovement;
    [HideInInspector] public Vector3 forwardWallVector;
    [HideInInspector] public Vector3 gravityUp;
    [HideInInspector] public Vector3 cameraForward;
    [HideInInspector] public Vector3 rightVector;
    [HideInInspector] public Vector3 forwardVector;
    
    public Transform orientation;
    public Transform GroundCheck;
    [HideInInspector] public Transform playerCam;
    [HideInInspector] public Rigidbody rigidbody;
    public CapsuleCollider _capsuleCollider;
    public bool DebugGizmos = false;
    
    private void Awake()
    {
        playerCam = Camera.main.transform;
        rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        sphereRadius = _capsuleCollider.radius * 0.9f;
        orientation = transform;
    }

    private void Start()
    {
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        
        currentSpeed = playerData.WalkSpeed;
        currentCounterForce = playerData.GroundCounterForce;
    }

    private void RotationTowardsGravityDirection()
    {
        Quaternion targetRoation = Quaternion.FromToRotation(orientation.up, -GravityDirection.normalized) * orientation.rotation;
        orientation.rotation = Quaternion.Slerp(orientation.rotation, targetRoation, playerData.RotationSpeed * Time.fixedDeltaTime);
    }

    private void CalculateMovement()
    {
        //Move Direction
        gravityUp = -GravityDirection.normalized;
        cameraForward = Vector3.Cross(gravityUp,playerCam.right);
        rightVector = Vector3.Cross(gravityUp, cameraForward);
        forwardVector = Vector3.Cross(gravityUp, rightVector);
        
        moveDirection = (rightVector * -rightInput + forwardVector * forwardInput).normalized;
    }



    private void Movement()
    {
        //Gravity
        if (ShouldApplyGravity)
        {
            if (!isGrounded)
            {
                if (ShouldIncreaseGravity)
                {
                    currentGravityStrength += Time.deltaTime * playerData.GravityIncrease;

                    currentGravityStrength =
                        Mathf.Clamp(currentGravityStrength, playerData.gravity, playerData.MaxGravity);
                }
                else
                {
                    currentGravityStrength = playerData.gravity;
                }
                rigidbody.AddForce(GravityDirection.normalized * currentGravityStrength, ForceMode.Force);    
            }
            else
            {
                rigidbody.AddForce(GravityDirection.normalized * playerData.gravity, ForceMode.Force);    
            }
        }

        //Move
        if (!isSliding)
        {
            if (isGrounded && !CheckSlope())
            {
                moveDirection = (moveDirection.normalized * currentSpeed * movementMultiplier);
            }
            else if (isGrounded && CheckSlope())
            {
                SlopeMovement = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
                moveDirection = (SlopeMovement.normalized * currentSpeed * movementMultiplier);
            }
            else if (!isGrounded && !OnWall)
            {
                moveDirection = moveDirection.normalized * currentSpeed * movementMultiplier * playerData.AirControll;
            }        
            else if (!isGrounded && OnWall)
            {
                moveDirection = (moveDirection.normalized * currentSpeed * movementMultiplier);
            }
        }

        rigidbody.AddForce(moveDirection);

        Magnetude = rigidbody.velocity.magnitude;
    }

    private void Update()
    {
        if (playerData == null)
        {
            print("playerData is null");
            return;
        }
        
        if (!_capsuleCollider)
        {
            return;
        }
        
        isGrounded = CheckGrounded();
        OnSlope = CheckSlope();
        OnWall = CheckWall();

        CalculateMovement();
        SetCurrentSpeed();
        RigidBodyDrag();

        if (CanWallRun())
        {
            if (wallLeft)
            {
                StartWallRun();
            }
            else if (wallRight)
            {
                StartWallRun();
            }
            else
            {
                EndWallRun(Vector3.zero);
            }
        }
        else
        {
            EndWallRun(Vector3.zero);
        }
        
        Jump();
    }
    
    private void FixedUpdate()
    {
        if (playerData == null)
        {
            print("playerData is null");
            return;
        }
        
        Movement();
        RotationTowardsGravityDirection();
    }
    public bool CanCrouch()
    {
        return (isGrounded || OnSlope);
    }

    public void StartCrouch() 
    {
        orientation.localScale = crouchScale;

        Vector3 NewPos = orientation.position + (GravityDirection.normalized * 0.5f);

        //isCrouching = true;
        
        orientation.position = NewPos;// new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
    
        if (rigidbody.velocity.sqrMagnitude < (VelocityThreshold * VelocityThreshold) && isCrouching)
        {
            isCrouching = false;
            isSliding = true;
            rigidbody.AddForce(orientation.forward * slideForce, ForceMode.Impulse);
        }
        else
        {
            isCrouching = true;
            isSliding = false;
        }
    }
    
    public void StopCrouch() {
        orientation.localScale = playerScale;
        Vector3 NewPos = orientation.position + (GravityDirection.normalized * -0.5f);
        orientation.position = NewPos;// new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        isCrouching = false;
    }

    void RigidBodyDrag()
    {
        if (isGrounded && !isSliding)
        {
            rigidbody.drag = groundDrag;
        }
        else if (!isGrounded && !isGrappeling)
        {
            rigidbody.drag = airDrag;
        }
        else
        {
            rigidbody.drag = grappelDrag;
        }
    }

    bool CanJump()
    {
        return (isGrounded || CurrnetJumpCount < playerData.MaxJumpCount || OnSlope) && jumpInput;
    }
    
    public void Jump()
    {
        if (OnWall)
        {
            return;
        }
        
        if (CanJump())
        {
            jumping = true;
            
            if (!isGrounded)
            {
                CurrnetJumpCount++;
                ResetDownwardsVel();
                rigidbody.AddForce(moveDirection * currentSpeed, ForceMode.Impulse);  
            }

            Vector3 jumpStrength = GravityDirection.normalized * -playerData.jumpStrength;
            rigidbody.AddForce(jumpStrength, ForceMode.Impulse);    
        }
    }
    
    public void ResetDownwardsVel()
    {
        Vector3 up = orientation.up;
        Vector3 counterUpVelocity = Vector3.Dot(rigidbody.velocity,up) * up;
        rigidbody.velocity -= counterUpVelocity;
        currentGravityStrength = playerData.gravity;
    }
    
    private void OnDrawGizmos()
    {
        if (DebugGizmos)
        {
            Vector3 pos = orientation.position + (GravityDirection.normalized * (_capsuleCollider.height / 2));
            Gizmos.DrawWireSphere(pos, sphereRadius);   
        }
    }

    public void Landed()
    {
        if (jumping)
        {
            jumping = false;
        }
        
        CurrnetJumpCount = 0;
        currentCounterForce = playerData.GroundCounterForce;
        currentGravityStrength = playerData.gravity;
    }

    private bool CheckGrounded()
    {
        //Vector3 pos = orientation.position + (GravityDirection.normalized * (_capsuleCollider.height / 2));

        bool ground = Physics.CheckSphere(GroundCheck.position, sphereRadius, GroundLayer);
        
        if (ground)//
        {
            if (!isGrounded)
            {
                Landed();
            }

            lastFallTime = 0;
            return true;
        }
        else
        {
            lastFallTime += Time.deltaTime;
            currentCounterForce = playerData.AirCounterForce;
            return false;
        }
        
        return false;
    }
    
    private bool CheckSlope()
    {
        Vector3 pos = orientation.position;
       
        if (DebugGizmos)
        {
            Debug.DrawRay(pos + (GravityDirection.normalized * _capsuleCollider.height / 2), GravityDirection.normalized, Color.blue);
        }

        RaycastHit slope;
        Physics.Raycast(pos + _capsuleCollider.center, GravityDirection, out slope,_capsuleCollider.height + maxDistance,GroundLayer);//layerMask

        if (slope.collider == null)
        {
            return false;
        }

        if (CheckGrounded())
        {
            ShouldIncreaseGravity = true;
        }
        
        slopeAngle = Mathf.Abs(Vector3.Angle(GravityDirection, slope.normal));

        if (slopeAngle < 180)
        {
            if (DebugGizmos)
            {
                Debug.DrawLine(slope.point ,slope.point + slope.normal, Color.green);
            }

            slopeHit = slope;
            return true;
        }
        else
        {
            slopeHit = slope;
            return false;
        }
    }

    private bool CheckWall()
    {
        Vector3 pos = orientation.position;
        Vector3 right = orientation.right;
        Vector3 forward = orientation.forward;
        
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

        //RaycastHit[] hit = new RaycastHit[directions.Length]; 
        
        for (int i = 0; i < directions.Length; i++)
        {
            Physics.Raycast(pos, directions[i], out WallHit, maxWallDistance, WallLayer);

            if (DebugGizmos)
            {
                Debug.DrawRay(pos, directions[i] * maxWallDistance, Color.red);
            }

            if (WallHit.collider != null)
            {
                if (DebugGizmos)
                {
                    Debug.DrawRay(WallHit.point, WallHit.normal * maxWallDistance, Color.green);    
                }

                int half = Mathf.RoundToInt(directions.Length / 2);

                if (half < i)
                {
                    wallLeft = true;
                    leftWallHit = WallHit;
                }
                else
                {
                    wallRight = true; 
                    rightWallHit = WallHit;
                }

                //StartWallRun();
                return true;
            }
            else
            {
                int half = Mathf.RoundToInt(directions.Length / 2);
                
                if (half > i)
                {
                    wallLeft = false; 
                }
                else
                {
                    wallRight = false; 
                }
            }
        }
        return false;
    }

    private bool CanWallRun()
    {
        if (!isGrounded)
        {
            return true;
        }
        
        if ((forwardInput == 0 && rightInput == 0))
        {
            EndWallRun(Vector3.zero);
            //print("no input");
            return false;
        }

        return true;
    }
    
    public void StartWallRun()//RaycastHit hit
    {
        ShouldApplyGravity = false;
        
        rigidbody.AddForce(GravityDirection.normalized, ForceMode.Force);

        float dotAngle = Vector3.Dot(forwardVector, WallHit.normal);
        //print(dotAngle);
        
        if (dotAngle < maxAngle && dotAngle > minAngle)
        {
            Vector3 forward = Vector3.ProjectOnPlane(orientation.forward, WallHit.normal);

            moveDirection = forward.normalized;
        
            if (jumpInput)
            {
                print("Jump");
            
                if (wallLeft)
                {
                    print("Jump wallLeft");
                    Vector3 wallRunJumpDirection = 
                        (orientation.up * playerData.WallJumpUpStrength) + 
                        (leftWallHit.normal * playerData.WallJumpDirStrength) +
                        (moveDirection.normalized * 10);
                    rigidbody.AddForce(wallRunJumpDirection, ForceMode.Impulse);
                }
            
                if (wallRight)
                {
                    print("Jump wallRight");
                    Vector3 wallRunJumpDirection = 
                        (orientation.up * playerData.WallJumpUpStrength) + 
                        (rightWallHit.normal * playerData.WallJumpDirStrength) +
                        (moveDirection.normalized * 10);
                    rigidbody.AddForce(wallRunJumpDirection, ForceMode.Impulse);
                }
            }

            CurrnetJumpCount = 0;
        }
    }
    
    public void EndWallRun(Vector3 WallNormal)
    {
        lastWallNormal = WallNormal;
        ShouldApplyGravity = true;
        OnWall = false;
    }
    
    void SetCurrentSpeed()
    {
        if (runInput && isGrounded)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, playerData.RunSpeed, acceliration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, playerData.WalkSpeed, acceliration * Time.deltaTime);
        }
    }
}
