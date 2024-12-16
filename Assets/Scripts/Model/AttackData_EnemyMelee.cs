using System;
using UnityEngine;
using static Enums;


[Serializable]
public struct AttackData_EnemyMelee
{
    public int AttackDamage;
    public string AttackName;
    public float AttackRange;
    public float MoveSpeed;
    public float AttackIndex;
    [Range(1f, 2f)]
    public float AnimationSpeed;

    public AttackType_Melee AttackType;
}
