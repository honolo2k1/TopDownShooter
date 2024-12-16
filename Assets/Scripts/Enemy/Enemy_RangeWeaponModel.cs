using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Enemy_RangeWeaponModel : MonoBehaviour
{
    public Transform Gunpoint;
    [Space]
    public Enemy_RangeWeaponType WeaponType;
    public Enemy_RangeWeaponHoldType WeaponHoldType;

    public Transform leftHandTarget;
    public Transform leftElbowTarget;
}
