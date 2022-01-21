using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Player;
    private PlayerMovement movement;
    private Transform _transform;
    private PlayerInput input;
    
    public float MouseSensetivity = 0;
    [HideInInspector] public float MouseX = 0;
    [HideInInspector] public float MouseY = 0;
    private float RotationX = 0;
    private float RotationY = 0;
    private float deltaSensetivity;
    [Header("Camera Effects")]
    [SerializeField] private float wallRunAngle = 20;
    [SerializeField] private float currentRunAngle = 0;
    [SerializeField] private float rotationSpeed = 10;
    private Camera cameraComp;
    public Transform cameraTransform;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        cameraComp = Camera.main;
        cameraTransform = cameraComp.transform;
        _transform = transform;
        movement = Player.GetComponent<PlayerMovement>();
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward);
        cameraTransform.transform.rotation = targetRotation;
        MouseY = 0;
    }

    public void Update()
    {
        deltaSensetivity = MouseSensetivity * Time.deltaTime;
        
        RotationY -= MouseY * deltaSensetivity;
        RotationY = Mathf.Clamp(RotationY, -90, 90);
        
        cameraTransform.position = _transform.position;
        cameraTransform.rotation = _transform.rotation;
        
        Player.transform.Rotate(Vector3.up,MouseX * deltaSensetivity);

        RotateCamera();
        /*
        if (movement.OnWall)
        {
            Quaternion angle = Quaternion.Euler(RotationY, 0, currentRunAngle);
            angle = Quaternion.Lerp(Quaternion.identity, angle, rotationSpeed * Time.deltaTime);//Time.fixedDeltaTime * rotationSpeed
            _transform.localRotation = angle;
        }else { }
        */
        _transform.localRotation = Quaternion.Euler(RotationY,0,currentRunAngle);
    }

    public void RotateCamera()
    {
        if (movement.wallRight)
        {
            currentRunAngle = Mathf.Lerp(currentRunAngle, wallRunAngle, rotationSpeed * Time.deltaTime);
        }
        else if (movement.wallLeft)
        {
            currentRunAngle = Mathf.Lerp(currentRunAngle, -wallRunAngle, rotationSpeed * Time.deltaTime);            
        }
        else
        {
            currentRunAngle = Mathf.Lerp(currentRunAngle, 0, rotationSpeed * Time.deltaTime);        
        }
    }

    private void FixedUpdate()
    {/*
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
        }*/
    }
}
