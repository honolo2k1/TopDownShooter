using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeaponSlot : MonoBehaviour
{
    private Image weaponIcon;
    private TextMeshProUGUI ammoText;

    private void Awake()
    {
        weaponIcon = GetComponentInChildren<Image>();
        ammoText = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void UpdateWeaponSlot(Weapon weapon, bool activeWeapon)
    {
        if (weapon == null)
        {
            weaponIcon.color = Color.clear;
            ammoText.text = string.Empty;
            return;
        }

        Color newColor = activeWeapon ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1);

        weaponIcon.color = newColor;
        weaponIcon.sprite = weapon.WeaponData.WeaponIcon;

        ammoText.text = weapon.BulletsInMagazine + "/" + weapon.TotalReverseAmmo;
        ammoText.color = Color.white;
    }
}
