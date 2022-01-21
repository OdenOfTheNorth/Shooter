using UnityEngine;

public class TestInputPlayer : MonoBehaviour
{
    public Player player;
    public PlayerLook playerLook;
    public WallRun wallRun;
    public Weapon WeaponSystem;
    [Header("Keys")] 
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    public Ability[] abilitys;
    public KeyCode[] keys;
    public KeyCode[] WeaponKeys;
    public bool debug = false;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerLook = GetComponent<PlayerLook>();
        wallRun = GetComponent<WallRun>();
        WeaponSystem = GetComponent<Weapon>();
    }

    void Update()
    {
        if (player)
        {
            player.horizontalMovement = Input.GetAxisRaw("Horizontal");
            player.verticalMovement = Input.GetAxisRaw("Vertical");
            player.jumpInput = Input.GetKeyDown(jumpKey);
            player.sprintInput = Input.GetKey(sprintKey);
        }

        if (playerLook)
        {
            playerLook.mouseX = Input.GetAxisRaw("Mouse X");
            playerLook.mouseY = Input.GetAxisRaw("Mouse Y");
        }

        if (wallRun)
        {
            wallRun.jumpInput = Input.GetKeyDown(jumpKey);
        }

        if (WeaponSystem)
        {
            if (WeaponSystem.currentWeapon.allowButtonHold)
            {
                WeaponSystem.currentWeapon.shooting = Input.GetKey(KeyCode.Mouse0);
            }
            else
            {
                WeaponSystem.currentWeapon.shooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && WeaponSystem.currentWeapon.bulletsLeft <
                WeaponSystem.currentWeapon.magazineSize && !WeaponSystem.currentWeapon.reloading)
            {
                WeaponSystem.Reload();  
            }


            //Shoot
            if (WeaponSystem.currentWeapon.readyToShoot && WeaponSystem.currentWeapon.shooting && 
                !WeaponSystem.currentWeapon.reloading && WeaponSystem.currentWeapon.bulletsLeft > 0)
            {
                WeaponSystem.currentWeapon.bulletsShot = WeaponSystem.currentWeapon.bulletsPerTap;
                WeaponSystem.Shoot();
            }
            /*
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
            }*/
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
