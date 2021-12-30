using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInput : MonoBehaviour
{
    public PlayerMovement movement;
    [HideInInspector] public CameraFollow cameraRotate;
    public GameObject CameraPos;
    public BasicWeapon weapon;
    public Ability[] abilitys;
    public Weapon WeaponSystem;
    public KeyCode[] keys;
    public bool debug = false;
    
    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        //weapon = GetComponent<BasicWeapon>();

        GameObject cameraObj = Camera.main.transform.gameObject;
        
        cameraRotate = CameraPos.GetComponent<CameraFollow>();
        weapon = cameraObj.GetComponent<BasicWeapon>();
    }

    void Update()
    {
        if (movement)
        {
            movement.forwardInput = Input.GetAxis("Vertical");
            movement.rightInput = Input.GetAxis("Horizontal");
            movement.runInput = Input.GetButton("Run");
            movement.crouchInput = Input.GetButton("Fire1");
            movement.jumpInput = Input.GetButtonDown("Jump");
        }

        if (cameraRotate)
        {
            cameraRotate.MouseX = Input.GetAxis("Mouse X");
            cameraRotate.MouseY = Input.GetAxis("Mouse Y");
        }

        //if (weapon)
        //{
        //    weapon.shootingInput = Input.GetButton("Fire1");
        //    weapon.AimDownSights = Input.GetButton("Fire2");
        //    weapon.relodingInput = Input.GetKeyDown(KeyCode.R);
        //}

        if (WeaponSystem)
        {
            if (WeaponSystem.WeaponDatas[WeaponSystem.currentWeaponIndex].allowButtonHold)
            {
                WeaponSystem.WeaponDatas[WeaponSystem.currentWeaponIndex].shooting = Input.GetKey(KeyCode.Mouse0);
            }
            else
            {
                WeaponSystem.WeaponDatas[WeaponSystem.currentWeaponIndex].shooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && WeaponSystem.WeaponDatas[WeaponSystem.currentWeaponIndex].bulletsLeft < 
                WeaponSystem.WeaponDatas[WeaponSystem.currentWeaponIndex].magazineSize && 
                !WeaponSystem.WeaponDatas[WeaponSystem.currentWeaponIndex].reloading) 
                WeaponSystem.Reload();

            //Shoot
            if (WeaponSystem.WeaponDatas[WeaponSystem.currentWeaponIndex].readyToShoot && WeaponSystem.WeaponDatas[WeaponSystem.currentWeaponIndex].shooting && !WeaponSystem.WeaponDatas[WeaponSystem.currentWeaponIndex].reloading && WeaponSystem.WeaponDatas[WeaponSystem.currentWeaponIndex].bulletsLeft > 0){
                WeaponSystem.WeaponDatas[WeaponSystem.currentWeaponIndex].bulletsShot = WeaponSystem.WeaponDatas[WeaponSystem.currentWeaponIndex].bulletsPerTap;
                WeaponSystem.Shoot();
            }
        }

        abilitys[0].abilityInput = Input.GetKeyDown(KeyCode.Q);
        abilitys[1].abilityInput = Input.GetKeyDown(KeyCode.E);
        
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
            if (keys.Length == abilitys.Length)
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
