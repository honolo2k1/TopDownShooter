using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Pickup_Ammo : Interacable
{    
    [SerializeField] private AmmoBoxType boxType;

    [SerializeField] private List<AmmoData> smallBoxAmmo;
    [SerializeField] private List<AmmoData> bigBoxAmmo;

    [SerializeField] private GameObject[] boxModel;
    private void Start()
    {
        SetupBoxModel();
    }

    public override void Interaction()
    {
        List<AmmoData> currentAmmoList = smallBoxAmmo;
        if (boxType == AmmoBoxType.BigBox)
        {
            currentAmmoList = bigBoxAmmo;
        }

        foreach (AmmoData ammo in currentAmmoList)
        {
            Weapon weapon = weaponController.WeaponInSlots(ammo.weaponType);
            AddBulletsToWeapon(weapon, GetBulletAmount(ammo));
        }

        UI.Instance.InGameUI.UpdateWeaponUI(GameManager.Instance.Player.Weapon.weaponSlots, GameManager.Instance.Player.Weapon.currentWeapon);
        ObjectPool.Instance.ReturnObject(gameObject);
    }

    private int GetBulletAmount(AmmoData ammoData)
    {
        float min = Mathf.Min(ammoData.minAmount, ammoData.maxAmount);
        float max = Mathf.Max(ammoData.minAmount, ammoData.maxAmount);

        float randomAmmoAmount = Random.Range(min, max);

        return Mathf.RoundToInt(randomAmmoAmount);
    }
    private void AddBulletsToWeapon(Weapon weapon, int amount)
    {
        if (weapon == null) return;
        weapon.TotalReverseAmmo += amount;
    }
    private void SetupBoxModel()
    {
        for (int i = 0; i < boxModel.Length; i++)
        {
            boxModel[i].gameObject.SetActive(false);

            if (i == (int)boxType)
            {
                boxModel[i].SetActive(true);
                UpdateMeshAndMaterial(boxModel[i].GetComponent<MeshRenderer>());
            }
        }
    }
}
