using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class WallRun : MonoBehaviour
{
    public Transform orientation;
    
    [Header("Wall Running")]
    [SerializeField] private float wallDistance = 0.5f;
    [SerializeField] private float minimumJumpHeight = 1.5f;
    [SerializeField] private float wallRunGravity;
    [SerializeField] private float wallRunJumpForce;
    [SerializeField] private float wallRunSpeed = 40f;
    [SerializeField] private bool wallLeft = false;
    [SerializeField] private bool wallRight = false;

    [SerializeField] private LayerMask WallLayer;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerMovement player;

    public bool jumpInput;

    private RaycastHit LeftHit;
    private RaycastHit RightHit;
    
    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, -Vector3.down, minimumJumpHeight);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<PlayerMovement>();
        orientation = transform;
    }

    public void CheckWall()
    {
        Physics.Raycast(transform.position, -orientation.right, out LeftHit, wallDistance );
        if (LeftHit.collider != null)
        {
            wallLeft = true;
        }
        else
        {
            wallLeft = false;
        }
        
        Physics.Raycast(transform.position, orientation.right, out RightHit, wallDistance );
        if (RightHit.collider != null)
        {
            wallRight = true;
        }        
        else
        {
            wallRight = false;
        }
    }

    private void Update()
    {
        CheckWall();

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
                StopWallRun();
            }
        }
        else
        {
            StopWallRun();
        }
    }

    void StartWallRun()
    {
        player.ShouldApplyGravity = false;
        
        rb.AddForce(player.GravityDirection * wallRunGravity, ForceMode.Force);
        /*
        if (wallLeft)
        {
            WallHit = LeftHit;
        }
        
        if (wallRight)
        {
            WallHit = RightHit;
        }
        */
        //Vector3 forward = Vector3.ProjectOnPlane(orientation.forward, WallHit.normal);
        //rb.AddForce(forward.normalized * wallRunSpeed, ForceMode.Force);

        if (!rb)
        {
            print("NO BODY");   
        }
        
        if (jumpInput)
        {
            if (wallLeft)
            {
                print("wallLeft");
                Vector3 wallRunJumpDirection = transform.up + LeftHit.normal;
                //Vector3 up = orientation.up;
                //Vector3 cancelDownwardsMomentum = Vector3.Dot(rb.velocity, up) * up;
                //rb.velocity -= cancelDownwardsMomentum;
                
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
            
            if (wallRight)
            {
                print("wallRight");
                Vector3 wallRunJumpDirection = transform.up + RightHit.normal;
                //Vector3 up = orientation.up;
                //Vector3 cancelDownwardsMomentum = Vector3.Dot(rb.velocity, up) * up;
                //rb.velocity -= cancelDownwardsMomentum;
                
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
        }
    }
    
    void StopWallRun()
    {
        player.ShouldApplyGravity = true;
    }
}


/*
    [Range(0, 1)] public float maxAngle = 1;
    [Range(-1, 0)] public float minAngle = -1;

    private Vector3 WallNormal;
    private Vector3 lastWallNormal;
    public LayerMask wallLayer;
    public int layer;

    public void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void Update()
    {
        layer = wallLayer.value;
        
        if (playerMovement.OnWall)
        {
            Vector3 antiGravity = playerMovement.GravityDirection * -playerMovement.playerData.gravity;
            Vector3 forcePlayerIntoWall = WallNormal * -playerMovement.playerData.gravity;
            Vector3 forwardWallVector = playerMovement.forwardWallVector;

            //Vector3 forces = antiGravity + forcePlayerIntoWall + forwardWallVector + playerMovement.counterMovement(playerMovement.rigidbody.velocity);
            
            //playerMovement.rigidbody.AddForce( forces, ForceMode.Force);
            
            if (playerMovement.jumpInput)
            {
                Vector3 wallJump = (WallNormal * playerMovement.playerData.WallJumpStrength) - (playerMovement.GravityDirection * playerMovement.playerData.jumpStrength);//+ ( * -playerData.jumpStrength);
                playerMovement.rigidbody.AddForce(wallJump, ForceMode.Impulse);
                EndWallRun();
            }
        }
        //CheckWall();
    }
    
    public bool CheckWall()
    {
        Vector3 pos = playerMovement.orientation.position;
        Vector3 right = playerMovement.orientation.right;
        Vector3 forward = playerMovement.orientation.forward;
        
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
            Physics.Raycast(pos, directions[i], out hit[i], playerMovement.maxWallDistance, playerMovement.layerMask);
            Debug.DrawRay(pos, directions[i] * playerMovement.maxWallDistance, Color.red);
           
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
    
    public void OnCollisionEnter(Collision other)
    {
        StartWallRun(other.contacts[0].normal); 
        
        if (wallLayer != other.gameObject.layer)
        {
            return;
        }

        if ((wallLayer.value & 1<<other.gameObject.layer) != 0)
        {
             
        }
        
        if (other.collider.gameObject.layer == layer)
        {
              
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (playerMovement.OnWall)
        {
            EndWallRun();
        }
    }

    public void StartWallRun(Vector3 WallNormal)
    {
        if (WallNormal == lastWallNormal)
        {
            return;
        }
        
        if ((playerMovement.forwardInput == 0 || playerMovement.rightInput == 0) && playerMovement.isGrounded)
        {
            return;
        }

        float dotAngle = Vector3.Dot(playerMovement.forwardVector, WallNormal);

        print(dotAngle);

        if (dotAngle < maxAngle && dotAngle > minAngle)
        {
            lastWallNormal = WallNormal;
            playerMovement.OnWall = true;
            playerMovement.forwardWallVector = Vector3.ProjectOnPlane(playerMovement.forwardVector, WallNormal).normalized * playerMovement.currentSpeed;
        }
    }
    
    public void EndWallRun()
    {   
        playerMovement.ShouldApplyGravity = true;
        playerMovement.OnWall = false;
    }

    void ResetJump()
    {
        playerMovement.CurrnetJumpCount = 1;
    }*/