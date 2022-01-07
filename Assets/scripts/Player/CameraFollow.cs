using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CameraFollow : MonoBehaviour
{
    public Transform Player;
    private PlayerMovement movement;
    //public Transform PlayerPos;
    private Transform _transform;
    private PlayerInput input;
    public Vector3 upVector;
    public float MouseSensetivity = 0;
    public float MouseX = 0;
    public float MouseY = 0;
    float RotationX = 0;
    public float RotationY = 0;
    public float deltaSensetivity;
    public float wallRunAngle = 20;
    public float rotationSpeed = 10;
    private Camera camera;
    public Transform cameraTransform;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        camera = Camera.main;
        cameraTransform = camera.transform;
        _transform = transform;
        movement = Player.GetComponent<PlayerMovement>();
    }

    public void Update()
    {
        cameraTransform.position = _transform.position;
    }

    private void FixedUpdate()
    {
        upVector = movement.GravityDirection;
        deltaSensetivity = MouseSensetivity * Time.fixedDeltaTime;
        
        RotationY -= MouseY * deltaSensetivity;
        //RotationX += MouseX * deltaSensetivity;
        RotationY = Mathf.Clamp(RotationY, -90, 90);

        
        cameraTransform.rotation = _transform.rotation;
        
        Player.transform.Rotate(Vector3.up,MouseX * deltaSensetivity);
        
        //if (movement.OnWallSides[0])
        //{
        //    Quaternion angle = Quaternion.Euler(RotationY, 0, wallRunAngle);
        //    angle = Quaternion.Lerp(Quaternion.identity, angle,  Time.fixedDeltaTime * rotationSpeed);
        //    _transform.localRotation = angle;
        //}else
        //if (movement.OnWallSides[1])
        //{
        //    Quaternion angle = Quaternion.Euler(RotationY, 0, -wallRunAngle);
        //    angle = Quaternion.Lerp(Quaternion.identity, angle,  Time.fixedDeltaTime * rotationSpeed);
        //    _transform.localRotation = angle;
        //}
        //else
        {
            _transform.localRotation = Quaternion.Euler(RotationY,0,0);
        }
    }
}
