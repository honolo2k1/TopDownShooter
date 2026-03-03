using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using static Enums;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsAlly;
    [Space]

    private const float REFERENCE_BULLET_SPEED = 20F;
    private Player player;

    [SerializeField] private List<Weapon_Data> defaultWeaponData;
    [SerializeField] public Weapon currentWeapon;
    private bool weaponReady;
    private bool isShooting;

    [Header("Bullet Details")]
    [SerializeField] private float bulletImpactForce;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletSpeed;


    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int maxSlots = 2;
    [SerializeField] public List<Weapon> weaponSlots;

    [SerializeField] private GameObject weaponPickupPrefab;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
    }
    private void Update()
    {
        if (isShooting)
            Shoot();
    }
    private IEnumerator BurstFire()
    {
        SetWeaponReady(false);
        for (int i = 1; i <= currentWeapon.BulletPerShot; i++)
        {
            FireSingleBullet();

            yield return new WaitForSeconds(currentWeapon.BurstFireDelay);

            if (i >= currentWeapon.BulletPerShot)
                SetWeaponReady(true);
        }
    }
    private void Shoot()
    {
        if (WeaponReady() == false) return;
        if (currentWeapon.CanShoot() == false) return;
        player.WeaponVisuals.PlayFireAnimation();
        if (currentWeapon.ShootType == ShootType.Single) isShooting = false;

        if (currentWeapon.BurstActived() == true)
        {
            StartCoroutine(BurstFire());
            return;
        };

        FireSingleBullet();

        TriggerEnemyDodge();
    }

    private void FireSingleBullet()
    {
        currentWeapon.BulletsInMagazine--;
        UpdateWeaponUI();

        player.WeaponVisuals.CurrentWeaponModel().FireSFX.Play();

        GameObject newBullet = ObjectPool.Instance.GetObject(bullet, GunPoint());

        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        bulletScript.BulletSetup(whatIsAlly,currentWeapon.BulletDamage, currentWeapon.GunDistance, bulletImpactForce);

        Vector3 bulletsDirection = currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.linearVelocity = bulletsDirection * bulletSpeed;
    }

    private void Reload()
    {
        SetWeaponReady(false);
        player.WeaponVisuals.PlayReloadAnimation();

        player.WeaponVisuals.CurrentWeaponModel().ReloadSfx.Play();
    }

    #region Slots Manager 

    public void SetDefaultWeapon(List<Weapon_Data> weaponData)
    {
        defaultWeaponData = new List<Weapon_Data>(weaponData);
        weaponSlots.Clear();

        foreach (Weapon_Data item in defaultWeaponData) 
        { 
            PickUpWeapon(new Weapon(item));
        }

        EquipWeapon(0);
    }


    private void EquipWeapon(int index)
    {
        if (index > weaponSlots.Count)
            return;

        SetWeaponReady(false);
        currentWeapon = weaponSlots[index];
        player.WeaponVisuals.PlayWeaponEquipAnimation();

        CameraManager.Instance.ChangeCameraDistance(currentWeapon.CameraDistance);

        UpdateWeaponUI();
    }
    public void PickUpWeapon(Weapon newWeapon)
    {

        if (WeaponInSlots(newWeapon.WeaponType) != null)
        {
            WeaponInSlots(newWeapon.WeaponType).TotalReverseAmmo += newWeapon.BulletsInMagazine;
            return;
        }

        if (weaponSlots.Count >= maxSlots && newWeapon.WeaponType != currentWeapon.WeaponType)
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);

            player.WeaponVisuals.SwitchOffWeaponModels();

            weaponSlots[weaponIndex] = newWeapon;
            CreateWeaponOnTheGround();

            EquipWeapon(weaponIndex);
            return;
        }

        weaponSlots.Add(newWeapon);
        player.WeaponVisuals.SwitchOnBackupWeaponModel();

        UpdateWeaponUI();
    }
    private void DropWeapon()
    {
        if (HasOnlyOneWeapon())
            return;

        CreateWeaponOnTheGround();

        if (weaponSlots.Count <= 1) { return; }
        weaponSlots.Remove(currentWeapon);

        EquipWeapon(0);
    }

    private void CreateWeaponOnTheGround()
    {
        GameObject droppedWeapon = ObjectPool.Instance.GetObject(weaponPickupPrefab, transform);
        droppedWeapon.GetComponent<Pickup_Weapon>()?.SetupPickupWeapon(currentWeapon, transform);
    }



    public void SetWeaponReady(bool ready) 
    {
        weaponReady = ready;
        
        if (ready)
            player.Sound.WeaponReady.Play();
    } 
    public bool WeaponReady() => weaponReady;

    #endregion

    public void UpdateWeaponUI()
    {
        UI.Instance.InGameUI.UpdateWeaponUI(weaponSlots, currentWeapon);
    }

    public Vector3 BulletDirection()
    {
        Transform aim = player.Aim.Aim();

        //if (player.aim.Target() != null)
        //{
        //    weaponHolder.LookAt(aim);
        //    GunPoint().LookAt(aim);
        //}

        Vector3 direction = (aim.position - GunPoint().position).normalized;

        //if (!player.aim.CanAimPrecisly())
        //{
        //    direction.y = 0;
        //}
        return direction;
    }

    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;
    public Weapon WeaponInSlots(WeaponType weaponType)
    {
        foreach (var weapon in weaponSlots)
        {
            if (weapon.WeaponType == weaponType)
            {
                return weapon;
            }
        }
        return null;
    }
    public Weapon CurrentWeapon() => currentWeapon;
    public Transform GunPoint() => player.WeaponVisuals.CurrentWeaponModel().gunPoint;

    private void TriggerEnemyDodge()
    {
        Vector3 rayOrigin = GunPoint().position;
        Vector3 rayDirection = BulletDirection();

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, Mathf.Infinity))
        {
            Enemy_Melee enemy_Melee = hit.collider.gameObject.GetComponentInParent<Enemy_Melee>();

            if (enemy_Melee != null)
            {
                enemy_Melee.ActiveDodgeRoll();
            }
        }
    }

    #region Input Events
    private void AssignInputEvents()
    {
        PlayerControls controls = player.Controls;
        controls.Character.Fire.performed += context => isShooting = true;
        controls.Character.Fire.canceled += context => isShooting = false;

        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
        controls.Character.EquipSlot3.performed += context => EquipWeapon(2);
        controls.Character.EquipSlot4.performed += context => EquipWeapon(3);
        controls.Character.EquipSlot5.performed += context => EquipWeapon(4);

        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };

        controls.Character.ToogleWeaponMode.performed += context => currentWeapon.ToogleBurst();
    }

    #endregion
}
