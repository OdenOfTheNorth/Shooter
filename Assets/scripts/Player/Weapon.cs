using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using TMPro;

public class Weapon : MonoBehaviour
{
    [FormerlySerializedAs("GunProp")] 
    public GameObject WeaponHolder;
    public TextMeshProUGUI text;

    public static int weaponAmount = 2;
    public WeaponData[] WeaponDatas = new WeaponData[weaponAmount];
    public GameObject[] currentMesh = new GameObject[weaponAmount];
    [HideInInspector] public WeaponData currentWeapon;

    private AudioSource audio;
    public int currentWeaponIndex = 0;

    public Transform trans;
    public Transform camera;
    public Transform attackPoint;
    public RaycastHit Hit;
    public LayerMask LayerMask;
    
    public GameObject muzzleFlash;
    public GameObject bulletHoleGraphic;
    public string DamageTag;
    public bool debug = false;

    private void Start()
    {
        trans = transform;
        
        currentWeapon = WeaponDatas[currentWeaponIndex];
        for (int i = 0; i < WeaponDatas.Length; i++)
        {
            if (!WeaponDatas[i].mesh)
            {
                continue;
            }
            currentMesh[i] = Instantiate(WeaponDatas[i].mesh, trans.position, Quaternion.identity, WeaponHolder.transform);

            Transform weaponTrans = currentMesh[i].transform;
            
            weaponTrans.eulerAngles = camera.eulerAngles;
            weaponTrans.position = attackPoint.position;
            
            if (i != currentWeaponIndex)
            {
                currentMesh[i].gameObject.SetActive(false);
            }
        }

        currentWeapon.bulletsLeft = currentWeapon.magazineSize;
        
        camera = Camera.main.transform;
        
        audio = GetComponent<AudioSource>();
        
        if (text)
        {
            text.text = currentWeapon.bulletsLeft + " / " + currentWeapon.magazineSize;    
        }

        for (int i = 0; i < WeaponDatas.Length; i++)
        {
            WeaponDatas[i].readyToShoot = true;
            WeaponDatas[i].reloading = false;
        }
    }

    public void Shoot()
    {
        if (debug)
        {
            print("shoot"); 
        }

        //Spread
        float x = Random.Range(-currentWeapon.spread, currentWeapon.spread);
        float y = Random.Range(-currentWeapon.spread, currentWeapon.spread);

        //Calculate Direction with Spread
        Vector3 direction = camera.forward + new Vector3(x, y, 0);

        //audio.clip = currentWeapon.audioClip;
        //audio.Play();

        if (!currentWeapon.UseProjectile)
        {
            //RayCast
            if (Physics.Raycast(camera.position, direction, out Hit, currentWeapon.range))
            {
                Debug.Log(Hit.collider.name);

                if (Hit.collider != null)
                {
                    if (Hit.collider.gameObject == gameObject)
                    {
                        return;
                    }
                
                    HealthComponent healthComponent = Hit.collider.GetComponent<HealthComponent>();
                    if (healthComponent)
                    {
                        healthComponent.TakeDamage(currentWeapon.Damage, gameObject);
                    }   

                    Rigidbody rigidbody = Hit.collider.GetComponent<Rigidbody>();
                    if (rigidbody)
                    {
                        rigidbody.AddForce(direction * currentWeapon.KnockBack, ForceMode.Impulse);
                    }
                
                    GunBounce bounce = Hit.collider.GetComponent<GunBounce>();
                    if (bounce)
                    {
                        rigidbody = GetComponent<Rigidbody>();
                        rigidbody.AddForce(direction * -bounce.bounceForce, ForceMode.Impulse);
                    }
                
                    //Graphics
                    if (bulletHoleGraphic)
                    {
                        Instantiate(bulletHoleGraphic, Hit.point, Quaternion.identity);
                    }
                }
            }
        }
        else
        {
            //print("Projectile");
            Quaternion targetRotation = Quaternion.LookRotation(camera.forward);
            
            GameObject obj = Instantiate(currentWeapon.projectile,attackPoint.position, targetRotation);

            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            
            if (rigidbody)
            {
                rigidbody.AddForce(camera.forward * currentWeapon.projectileForce, ForceMode.Impulse);
            }
        }

        if (muzzleFlash)
        {
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);    
        }
        
        currentWeapon.readyToShoot = false;
        currentWeapon.bulletsLeft--;
        currentWeapon.bulletsShot--;

        if (text)
        {
            text.text = currentWeapon.bulletsLeft + " / " + currentWeapon.magazineSize;    
        }
        
        //Debug.Break();

        Invoke("ResetShot", currentWeapon.timeBetweenShooting);

        if(currentWeapon.bulletsShot > 0 && currentWeapon.bulletsLeft > 0)
            Invoke("Shoot", currentWeapon.timeBetweenShots);
    }
    private void ResetShot()
    {
        if (text)
        {
            text.text = currentWeapon.bulletsLeft + " / " + currentWeapon.magazineSize;    
        }
        currentWeapon.readyToShoot = true;
    }
    public void Reload()
    {
        //print("Reload");
        currentWeapon.reloading = true;
        if (text)
        {
            text.text = currentWeapon.bulletsLeft + " / " + currentWeapon.magazineSize;    
        }
        Invoke("ReloadFinished", currentWeapon.reloadTime);
    }
    private void ReloadFinished()
    {
        currentWeapon.bulletsLeft = currentWeapon.magazineSize;
        currentWeapon.reloading = false;
        if (text)
        {
            text.text = currentWeapon.bulletsLeft + " / " + currentWeapon.magazineSize;    
        }
    }
    
    public void ChangeWeapon()
    {
        /*
        if (WeaponDatas.Length == 0) return;
        
        if (currentWeapon.reloading)
        {
            return;
        }
        
        currentMesh[currentWeaponIndex].gameObject.SetActive(false);
        currentWeaponIndex = weaponIndex;
        currentMesh[currentWeaponIndex].gameObject.SetActive(true);
        
        Destroy(currentMesh);
        
        currentWeaponIndex = (currentWeaponIndex + 1) % WeaponDatas.Length;
        currentWeapon.bulletsLeft = currentWeapon.magazineSize;
        currentWeapon.readyToShoot = true;
        
        currentMesh = Instantiate(currentWeapon.mesh, transform.position, Quaternion.identity, WeaponHolder.transform);
        //currentMesh.transform.eulerAngles = Vector3.zero;
        currentMesh.transform.localPosition = Vector3.zero;
        if (text)
        {
            text.text = currentWeapon.bulletsLeft + " / " + currentWeapon.magazineSize;    
        }*/
    }
    
    public void ChangeWeapon(int weaponIndex)
    {
        if (currentWeapon.reloading)
        {
            return;
        }
        
        if (WeaponDatas.Length == 0) return;
        
        if (currentMesh[currentWeaponIndex].gameObject != null)
        {
            currentMesh[currentWeaponIndex].gameObject.SetActive(false);
        }

        currentWeaponIndex = weaponIndex;
        
        if (currentMesh[currentWeaponIndex].gameObject != null)
        {
            currentMesh[currentWeaponIndex].gameObject.SetActive(true);
        }
        
        currentWeapon = WeaponDatas[currentWeaponIndex];
        
        //currentWeapon.bulletsLeft = currentWeapon.magazineSize;
        //currentWeapon.readyToShoot = true;
        currentWeapon.reloading = false;
        ResetShot();
        //currentMesh = Instantiate(currentWeapon.mesh, transform.position, Quaternion.identity, WeaponHolder.transform);
        //currentMesh.transform.eulerAngles = camera.eulerAngles;
        //currentMesh.transform.localPosition = Vector3.zero;
        //if (text)
        //{
        //    text.text = currentWeapon.bulletsLeft + " / " + currentWeapon.magazineSize;    
        //}
    }
    
    public void Update()
    {
        //currentMesh.transform.localPosition = Vector3.zero;
        //if (text)
        //{
        //    text.text = currentWeapon.bulletsLeft + " / " + currentWeapon.magazineSize;    
        //}
    }
}
