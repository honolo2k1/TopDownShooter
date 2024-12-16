using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectedWeaponWindow : MonoBehaviour
{
    public Weapon_Data WeaponData;

    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponInfo;

    private void Start()
    {
        WeaponData = null;
        UpdateSlotInfo(null);
    }
    public void SetWeaponSlot(Weapon_Data weaponData)
    {
        WeaponData = weaponData;
        UpdateSlotInfo(weaponData);
    }
    public void UpdateSlotInfo(Weapon_Data weaponData)
    {
        if (weaponData == null)
        {
            weaponIcon.color = Color.clear;
            weaponInfo.text = "Select a weapon...";
            return;
        }
        weaponIcon.color = Color.white;
        weaponIcon.sprite = weaponData.WeaponIcon;
        weaponInfo.text = weaponData.WeaponInfo;
    }
    public bool IsEmpty() => WeaponData == null;
}
