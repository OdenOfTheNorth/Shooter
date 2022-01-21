using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float playerHeight = 2;
    
    [SerializeField] Transform orientation;
    
    [Header("Movement")] 
    public float moveSpeed = 5;
    [Range(0,1)] public float airMultiplier = 0.15f;
    private float movementMultiplier = 10;
    
    [Header("Sprinting")] 
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintSpeed = 7f;
    [SerializeField] float acceliration = 10f;
    
    
    [Header("Jump")] 
    public float jumpForce = 5;

    [Header("Gravity")]
    public bool ShouldApplyGravity = true;
    public Vector3 GravityDirection = Vector3.down;
    public float GravityStrength = 10f;
    public float RotationSpeed = 10f;
    
    [Header("Drag")] 
    public float groundDrag = 6;
    public float airDrag = 2;
    
    //[Header("Keys")] 
    [SerializeField] public bool jumpInput = false;
    [SerializeField] public bool sprintInput = false;
    
    public float horizontalMovement;
    public float verticalMovement;

    [Header("Ground")] 
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded = false;
    private float groundDistance = 0.4f;

    private Vector3 moveDirection;
    private Vector3 SlopeMoveDirection;
    
    private Rigidbody rg;

    private RaycastHit GroundHit;
    private RaycastHit SlopeHit;

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out SlopeHit, playerHeight / 2 + 0.5f))
        {
            if (SlopeHit.normal != -GravityDirection)
            {
                return true;
            }
        }
        
        return false;
    }

    void Start()
    {
        rg = GetComponent<Rigidbody>();
        rg.freezeRotation = true;
        rg.useGravity = false;
    }


    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
        
        print(isGrounded);
        
        MyInput();
        ControlDrag();
        ControlSpeed();

        if (jumpInput && isGrounded)
        {
            Jump();
        }

        SlopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, SlopeHit.normal);
    }

    private void MyInput()
    {
        //horizontalMovement = Input.GetAxisRaw("Horizontal");
        //verticalMovement = Input.GetAxisRaw("Vertical");
        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    void Jump()
    {
        Vector3 up = orientation.up;
        Vector3 cancelDownwardsMomentum = Vector3.Dot(rg.velocity, up) * up;

        rg.velocity -= cancelDownwardsMomentum;
        rg.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    
    void ControlSpeed()
    {
        if (sprintInput && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceliration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceliration * Time.deltaTime);
        }
    }
    
    void ControlDrag()
    {
        if (isGrounded)
        {
            rg.drag = groundDrag;
        }
        else
        {
            rg.drag = airDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotationTowardsGravityDirection();
    }
    
    void MovePlayer()
    {
        Drag();
        if (ShouldApplyGravity)
        {
            rg.AddForce(GravityDirection.normalized * GravityStrength, ForceMode.Force);    
        }
        
        if (isGrounded && !OnSlope())
        {
            rg.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);    
        }
        else if (isGrounded && OnSlope())
        {
            rg.AddForce(SlopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);    
        }
        else if (!isGrounded)
        {
            rg.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);    
        }
    }
    
    private void RotationTowardsGravityDirection()
    {
        Vector3 gravityUp = -GravityDirection.normalized;
        Quaternion targetRoation = Quaternion.FromToRotation(orientation.up, gravityUp) * orientation.rotation;
        orientation.rotation = Quaternion.Slerp(orientation.rotation, targetRoation, RotationSpeed * Time.fixedDeltaTime);
    }

    void Drag()
    {
        Vector3 Velocity = (rg.velocity) * (1 - 0.15f);
        
        rg.AddForce(Velocity, ForceMode.Acceleration);
    }
}
