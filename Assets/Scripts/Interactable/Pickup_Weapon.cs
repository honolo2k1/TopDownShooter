using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Weapon : Interacable
{   
    [SerializeField] private Weapon_Data weaponData;
    [SerializeField] private Weapon weapon;

    [SerializeField] private BackupWeaponModel[] models;
    private bool isOldWeapon;

    private void Start()
    {

        if (isOldWeapon == false)
        {
            weapon = new Weapon(weaponData);
        }
        SetupGameObject();
    }

    public void SetupPickupWeapon(Weapon weapon, Transform transform)
    {
        isOldWeapon = true;

        this.weapon = weapon;
        weaponData = weapon.WeaponData;

        this.transform.position = transform.position + new Vector3(0, 0.75f, 0);
    }

    [ContextMenu("Update Item Model")]
    private void SetupGameObject()
    {
        gameObject.name = "Pickup_Weapon - " + weaponData.WeaponType.ToString();
        SetupWeaponModel();
    }
    private void SetupWeaponModel()
    {
        foreach (BackupWeaponModel model in models)
        {
            model.gameObject.SetActive(false);
            if (model.WeaponType == weaponData.WeaponType)
            {
                model.gameObject.SetActive(true);
                UpdateMeshAndMaterial(model.GetComponent<MeshRenderer>());
            }
        }
    }

    public override void Interaction()
    {
        weaponController.PickUpWeapon(weapon);

        ObjectPool.Instance.ReturnObject(gameObject);
    }
}
 