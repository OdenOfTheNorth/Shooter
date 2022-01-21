using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInput : MonoBehaviour
{
    public PlayerMovement movement;
    [HideInInspector] public CameraFollow cameraRotate;
    public GameObject CameraPos;
    //public BasicWeapon weapon;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode ShootingKey = KeyCode.Mouse0;
    public KeyCode[] WeaponKeys;
    public Ability[] abilitys;
    public KeyCode[] keys;
    public Weapon WeaponSystem;
    public bool debug = false;
    
    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        //weapon = GetComponent<BasicWeapon>();

        GameObject cameraObj = Camera.main.transform.gameObject;
        
        cameraRotate = CameraPos.GetComponent<CameraFollow>();
        //weapon = cameraObj.GetComponent<BasicWeapon>();
    }

    void Update()
    {
        if (movement)
        {
            movement.forwardInput = Input.GetAxisRaw("Vertical");
            movement.rightInput = Input.GetAxisRaw("Horizontal");
            //movement.crouchInput = Input.GetKey(crouchKey);
            if (Input.GetKeyDown(crouchKey) && movement.CanCrouch())
            {
                movement.StartCrouch();
            }
            if (Input.GetKeyUp(crouchKey) && movement.CanCrouch())
            {
                movement.StopCrouch();
            }

            movement.jumpInput = Input.GetKeyDown(jumpKey);
            movement.runInput = Input.GetKey(sprintKey);
        }

        if (cameraRotate)
        {
            cameraRotate.MouseX = Input.GetAxis("Mouse X");
            cameraRotate.MouseY = Input.GetAxis("Mouse Y");
        }
        
        if (WeaponSystem)
        {
            //int index = WeaponSystem.currentWeaponIndex;
            
            if (WeaponSystem.currentWeapon.allowButtonHold)
            {
                WeaponSystem.currentWeapon.shooting = Input.GetKey(ShootingKey);
            }
            else
            {
                WeaponSystem.currentWeapon.shooting = Input.GetKeyDown(ShootingKey);
            }

            if (Input.GetKeyDown(KeyCode.R) && WeaponSystem.currentWeapon.bulletsLeft < 
                WeaponSystem.currentWeapon.magazineSize && 
                !WeaponSystem.currentWeapon.reloading) 
                WeaponSystem.Reload();

            //Shoot
            if (WeaponSystem.currentWeapon.readyToShoot && WeaponSystem.currentWeapon.shooting && 
                !WeaponSystem.currentWeapon.reloading && WeaponSystem.currentWeapon.bulletsLeft > 0){
                WeaponSystem.currentWeapon.bulletsShot = WeaponSystem.currentWeapon.bulletsPerTap;
                WeaponSystem.Shoot();
            }
        }
        
        for (int i = 0; i < WeaponKeys.Length; i++)
        {
            if (WeaponKeys[i] == KeyCode.None)
            {
                if (debug)
                {
                    print("keys " + i + " dose not exist");    
                }
                
                continue;
            }
            if (WeaponSystem.WeaponDatas[i] == null)
            {
                if (debug)
                {
                    print("WeaponSystem " + i + " dose not exist");
                }

                continue;
            }
            if (WeaponKeys.Length != WeaponSystem.WeaponDatas.Length)
            {
                if (debug)
                {
                    print("keys and abilitys langths are difrect"); 
                }

                continue;
            }

            if (Input.GetKeyDown(WeaponKeys[i]))
            {
                WeaponSystem.ChangeWeapon(i);
            }
        }

        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i] == KeyCode.None)
            {
                if (debug)
                {
                    print("keys " + i + " dose not exist");    
                }
                
                continue;
            }
            if (abilitys[i] == null)
            {
                if (debug)
                {
                    print("ability " + i + " dose not exist");
                }

                continue;
            }
            if (keys.Length != abilitys.Length)
            {
                if (debug)
                {
                    print("keys and abilitys langths are difrect"); 
                }

                continue;
            }

            abilitys[i].abilityInput = Input.GetKeyDown(keys[i]);
        }
        
    }
}
