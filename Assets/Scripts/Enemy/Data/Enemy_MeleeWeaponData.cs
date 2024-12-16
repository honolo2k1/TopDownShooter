using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy Data/Melee Weapon Data")]
public class Enemy_MeleeWeaponData : ScriptableObject
{

    public List<AttackData_EnemyMelee> AttackData;
    public float TurnSpeed = 10;
}
