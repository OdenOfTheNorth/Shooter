using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    public GameObject WeaponHolder;
    public WeaponData[] WeaponDatas;
    
    public int currentWeaponIndex = 0;

    public Camera Camera;
    public Transform attackPoint;
    public RaycastHit Hit;
    public LayerMask Enemy;
    
    public GameObject muzzleFlash;
    public GameObject bulletHoleGraphic;

    private void Awake()
    {
        WeaponDatas[currentWeaponIndex].bulletsLeft = WeaponDatas[currentWeaponIndex].magazineSize;
        WeaponDatas[currentWeaponIndex].readyToShoot = true;
        Camera = Camera.main;
    }

    public void Shoot()
    {
        WeaponDatas[currentWeaponIndex].readyToShoot = false;

        //Spread
        float x = Random.Range(-WeaponDatas[currentWeaponIndex].spread, WeaponDatas[currentWeaponIndex].spread);
        float y = Random.Range(-WeaponDatas[currentWeaponIndex].spread, WeaponDatas[currentWeaponIndex].spread);

        //Calculate Direction with Spread
        Vector3 direction = Camera.transform.forward + new Vector3(x, y, 0);

        //RayCast
        if (Physics.Raycast(Camera.transform.position, direction, out Hit, WeaponDatas[currentWeaponIndex].range, Enemy))
        {
            Debug.Log(Hit.collider.name);

            if (Hit.collider.CompareTag("Enemy"))
                Hit.collider.GetComponent<HealthComponent>().TakeDamage(WeaponDatas[currentWeaponIndex].Damage);
        }

        //Graphics
        Instantiate(bulletHoleGraphic, Hit.point, Quaternion.Euler(0, 180, 0));
        Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        WeaponDatas[currentWeaponIndex].bulletsLeft--;
        WeaponDatas[currentWeaponIndex].bulletsShot--;

        Invoke("ResetShot", WeaponDatas[currentWeaponIndex].timeBetweenShooting);

        if(WeaponDatas[currentWeaponIndex].bulletsShot > 0 && WeaponDatas[currentWeaponIndex].bulletsLeft > 0)
            Invoke("Shoot", WeaponDatas[currentWeaponIndex].timeBetweenShots);
    }
    private void ResetShot()
    {
        WeaponDatas[currentWeaponIndex].readyToShoot = true;
    }
    public void Reload()
    {
        WeaponDatas[currentWeaponIndex].reloading = true;
        Invoke("ReloadFinished", WeaponDatas[currentWeaponIndex].reloadTime);
    }
    private void ReloadFinished()
    {
        WeaponDatas[currentWeaponIndex].bulletsLeft = WeaponDatas[currentWeaponIndex].magazineSize;
        WeaponDatas[currentWeaponIndex].reloading = false;
    }
    
    public void ChangeWeapon()
    {
        if (WeaponDatas.Length == 0) return;
        currentWeaponIndex = (currentWeaponIndex + 1) % WeaponDatas.Length;	
        
        WeaponDatas[currentWeaponIndex].bulletsLeft = WeaponDatas[currentWeaponIndex].magazineSize;
    }
}
