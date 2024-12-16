using UnityEngine;
using static Enums;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy Data/Range Weapon Data")]
public class Enemy_RangeWeaponData : ScriptableObject
{
    [Header("Weapon Details")]
    public Enemy_RangeWeaponType WeaponType;
    public float FireRate = 1;

    public int MinBulletPerAttack = 1;
    public int MaxBulletPerAttack = 1;

    public float MinWeaponCooldown = 2;
    public float MaxWeaponCooldown = 3;

    [Header("Bullet Details")]
    public int BulletDamage;
    [Space]
    public float BulletSpeed = 20;
    public float WeaponSpread = 0.1f;

    public int GetBulletsPerAttack() => Random.Range(MinBulletPerAttack, MaxBulletPerAttack + 1);
    public float GetWeaponCooldown() => Random.Range(MinWeaponCooldown, MaxWeaponCooldown);

    public Vector3 ApplyWeaponSpread(Vector3 originalDirection)
    {

        float randomizedValue = Random.Range(-WeaponSpread, WeaponSpread);

        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue / 2, randomizedValue);

        return spreadRotation * originalDirection;
    }
}
