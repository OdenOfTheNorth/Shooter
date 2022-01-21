using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponData", menuName = "Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Audio")] 
    public GameObject audioClip;
    [Header("Graphics")] 
    public GameObject mesh;
    [Header("Gun stats")]
    public bool UseProjectile; 
    public GameObject projectile;
    public float projectileForce;
    
    public float Damage;
    public float timeBetweenShooting;
    public float spread;
    public float range;
    public float reloadTime;
    public float timeBetweenShots;

    public float EquipeSpeed = 3;

    public float KnockBack = 10;
    
    public int magazineSize;
    public int bulletsPerTap;
    public int bulletsLeft;
    public int bulletsShot;
    [Header("bools")]
    public bool allowButtonHold;
    public bool shooting;
    public bool readyToShoot;
    public bool reloading;
    
    /*public Mesh Mesh;
    public int fireRatePerMinute = 30;
    public int ammoAmount = 10;
    public float reloadSpeed = 3f;
    
    [Header("Bullets")] 
    public bool useProjectiles = false;
    public GameObject bullet;
    public float bulletSpeed = 100f;
    public float bulletDamage = 10;*/
}
