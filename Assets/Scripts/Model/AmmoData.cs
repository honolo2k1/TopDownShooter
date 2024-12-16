using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;


[Serializable]
public struct AmmoData
{
    public WeaponType weaponType;
    [Range(10 ,100)] public int minAmount;
    [Range(10, 100)] public int maxAmount;
}