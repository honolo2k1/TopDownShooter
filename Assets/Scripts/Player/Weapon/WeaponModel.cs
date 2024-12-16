using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class WeaponModel : MonoBehaviour
{
    public WeaponType WeaponType;
    public EquipType EquipType;
    public HoldType HoldType;

    public Transform gunPoint;
    public Transform holdPoint;

    [Header("Audio")]
    public AudioSource FireSFX;
    public AudioSource ReloadSfx;
}
