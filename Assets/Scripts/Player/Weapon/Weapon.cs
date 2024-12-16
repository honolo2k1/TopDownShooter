using UnityEngine;
using static Enums;

[System.Serializable]

public class Weapon
{
    public WeaponType WeaponType;

    public int BulletDamage;

    #region Regular Mode Variables
    public ShootType ShootType;
    public int BulletPerShot { get; private set; }
    private float defaultFireRate;
    public float FireRate { get; private set; }
    private float lastShootTime;
    #endregion

    #region Burst Mode Variables
    private bool burstAvalible;
    private bool burstActive;
    private int burstBulletsPerShot;
    private float burstFireRate;
    public float BurstFireDelay { get; private set; }
    #endregion

    [Header("Magazine Details")]
    private int magazineCapacity;
    public int TotalReverseAmmo;
    public int BulletsInMagazine;

    #region Weapon Generic Info
    public float EquipmentSpeed { get; private set; }
    public float ReloadSpeed { get; private set; }
    public float GunDistance { get; private set; }
    public float CameraDistance { get; private set; }
    #endregion

    #region Weapon Spread Info
    private float baseSpread = 1f;
    private float maximumSpread = 3f;
    private float currentSpread = 2f;

    private float spreadIncreaseRate = 0.15f;

    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1f;
    #endregion

    public Weapon_Data WeaponData { get; private set; }

    public Weapon(Weapon_Data weaponData)
    {
        BulletDamage = weaponData.BulletDamage;

        BulletsInMagazine = weaponData.BulletsInMagazine;
        magazineCapacity = weaponData.MagazineCapacity;
        TotalReverseAmmo = weaponData.TotalReverseAmmo;

        burstActive = weaponData.BurstActive;
        burstAvalible = weaponData.BurstAvalible;
        burstBulletsPerShot = weaponData.BurstBulletsPerShot;
        burstFireRate = weaponData.BurstFireRate;
        BurstFireDelay = weaponData.BurstFireDelay;

        BulletPerShot = weaponData.BulletPerShot;
        FireRate = weaponData.FireRate;
        WeaponType = weaponData.WeaponType;
        ShootType = weaponData.ShootType;

        baseSpread = weaponData.BaseSpread;
        maximumSpread = weaponData.MaxSpread;
        spreadIncreaseRate = weaponData.SpreadIncreaseRate;

        EquipmentSpeed = weaponData.EquipmentSpeed;
        ReloadSpeed = weaponData.ReloadSpeed;
        GunDistance = weaponData.GunDistance;
        CameraDistance = weaponData.CameraDistance;

        defaultFireRate = FireRate;

        this.WeaponData = weaponData;
    }


    #region Spread Methods
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-currentSpread, currentSpread);

        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue / 2, randomizedValue);

        return spreadRotation * originalDirection;
    }
    private void UpdateSpread()
    {
        if (Time.time > lastSpreadUpdateTime + spreadCooldown)
        {
            currentSpread = baseSpread;
        }
        InceaseSpread();
        lastSpreadUpdateTime = Time.time;

    }
    private void InceaseSpread()
    {
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }
    #endregion

    #region Burst Methods
    public bool BurstActived()
    {
        if (WeaponType == WeaponType.Shotgun)
        {
            BurstFireDelay = 0;
            return true;
        }

        return burstActive;
    }

    public void ToogleBurst()
    {
        if (burstAvalible == false) return;
        burstActive = !burstActive;

        if (burstActive)
        {
            BulletPerShot = burstBulletsPerShot;
            FireRate = burstFireRate;
        }
        else
        {
            BulletPerShot = 1;
            FireRate = defaultFireRate;
        }
    }
    #endregion
    public bool CanShoot() => HaveEnoughBullet() && ReadyToFire();

    private bool ReadyToFire()
    {
        if (Time.time > lastShootTime + 1 / FireRate)
        {
            lastShootTime = Time.time;
            return true;
        }
        return false;
    }
    #region Reload Methods
    public bool CanReload()
    {
        if (BulletsInMagazine == magazineCapacity) { return false; }

        if (TotalReverseAmmo > 0)
        {
            return true;
        }
        return false;
    }
    public void RefillBullets()
    {
        int bulletsToReload = magazineCapacity;
        if (bulletsToReload > TotalReverseAmmo)
            bulletsToReload = TotalReverseAmmo;

        TotalReverseAmmo -= bulletsToReload;
        BulletsInMagazine = bulletsToReload;

        if (TotalReverseAmmo < 0)
            TotalReverseAmmo = 0;
    }
    private bool HaveEnoughBullet() => BulletsInMagazine > 0;

    #endregion
}
