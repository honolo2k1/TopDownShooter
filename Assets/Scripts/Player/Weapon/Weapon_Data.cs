using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;


[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon System/Weapon Data")]
public class Weapon_Data : ScriptableObject
{
    public string WeaponName;

    [Header("Bullet Info")]
    public int BulletDamage;

    [Header("Regular Shot")]
    public ShootType ShootType;
    public int BulletPerShot = 1;
    public float FireRate = 1;

    [Header("Burst Shot")]
    public bool BurstAvalible;
    public bool BurstActive;
    public int BurstBulletsPerShot;
    public float BurstFireRate;
    public float BurstFireDelay = 0.1f;

    [Header("Magazine Details")]
    public int BulletsInMagazine;
    public int MagazineCapacity;
    public int TotalReverseAmmo;

    [Header("Weapon Spread")]
    public float BaseSpread;
    public float MaxSpread;
    public float SpreadIncreaseRate = 0.15f;

    [Header("Weapon Generics")]
    public WeaponType WeaponType;
    [Range(1, 3)]
    public float ReloadSpeed = 1;
    [Range(1, 3)]
    public float EquipmentSpeed = 1;
    [Range(4, 25)]
    public float GunDistance = 4;
    [Range(4, 12)]
    public float CameraDistance = 6;

    [Header("UI Elements")]
    public Sprite WeaponIcon;
    public string WeaponInfo;
}
