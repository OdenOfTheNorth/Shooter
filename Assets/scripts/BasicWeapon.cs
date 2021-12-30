using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeapon : MonoBehaviour
{/*
    public WeaponData[] _data;
    private float currentFireDelay = 1;
    private float TimeBetweenShots = 1;
    private int currentAmmoAmount;
    public int currentWeaponIndex = 0;

    public bool AimDownSights;
    public bool IsReloding = false;
    public bool relodingInput;
    public bool shootingInput;
    private bool CanShoot = true;

    private void Start()
    {
        ResetWeapon();
    }

    void ResetWeapon()
    {
        currentFireDelay = 1f;
        currentAmmoAmount = _data[currentWeaponIndex].ammoAmount;
    }

    private void Update()
    {
        FireDelay();

        if (relodingInput)
        {
            Reload();
        }
        
        if (CanShoot)
        {
            currentFireDelay += Time.deltaTime;    
            if (shootingInput)
            {
                print(" currentFireDelay = " + currentFireDelay + " TimeBetweenShots = " + TimeBetweenShots + " currentAmmoAmount = " + currentAmmoAmount );
                if (currentFireDelay > TimeBetweenShots && currentAmmoAmount > 0)
                {
                    Shoot();
                
                    currentFireDelay = 0;
                }
            }
        }
    }

    void FireDelay()
    {
        //print("FireDelay");
        TimeBetweenShots = 60f / _data[currentWeaponIndex].fireRatePerMinute;
    }

    public IEnumerator Reload()
    {
        IsReloding = true;
        
        yield return new WaitForSeconds(_data[currentWeaponIndex].reloadSpeed);
        
        currentAmmoAmount = _data[currentWeaponIndex].ammoAmount;
        IsReloding = false;
    }
    
    //public void Reload()
    //{
    //    currentAmmoAmount = _data[currentWeaponIndex].ammoAmount;
    //}

    public void Shoot()
    {
        print("Shoot");
        if (_data[currentWeaponIndex].useProjectiles)
        {
            GameObject bullet = Instantiate(_data[currentWeaponIndex].bullet, transform.position, Quaternion.identity);

            if (!bullet)
            {
                print("bullet is null");
                return;
            }
            Rigidbody rigidbody = bullet.GetComponent<Rigidbody>();
            
            if (!rigidbody)
            {
                print("bullet's rigidbody is null");
                return;
            }
            
            rigidbody.AddForce(transform.forward * _data[currentWeaponIndex].bulletSpeed, ForceMode.Impulse);
        }
        else
        {
            
        }

        currentAmmoAmount--;
    }

    public void ADS()
    {
        
    }

    public void ChangeWeapon()
    {
        if (_data.Length == 0) return;
        currentWeaponIndex = (currentWeaponIndex + 1) % _data.Length;	
        
        currentAmmoAmount = _data[currentWeaponIndex].ammoAmount;
    }*/
}
