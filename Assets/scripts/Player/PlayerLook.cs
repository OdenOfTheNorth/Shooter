using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField]private float sensX = 10f;
    [SerializeField]private float sensY = 10f;

    [SerializeField] Transform cam;
    [SerializeField] Transform orientation;
    
    public float mouseX;
    public float mouseY;
    
    private float multiplier = 0.01f;
    
    private float xRotation;
    private float yRotation;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        MyInput();
        
        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation ,0);
        orientation.rotation = Quaternion.Euler(0, yRotation,0);
    }

    void MyInput()
    {
        //mouseX = Input.GetAxisRaw("Mouse X");
        //mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90, 90);
    }
}
