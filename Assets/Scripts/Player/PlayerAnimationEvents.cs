using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{

    private PlayerWeaponVisuals visualController;
    private PlayerWeaponController weaponController;

    private void Start()
    {
        visualController = GetComponentInParent<PlayerWeaponVisuals>();
        weaponController = GetComponentInParent<PlayerWeaponController>();
    }
    public void ReloadIsOver()
    {
        visualController.MaximizeRigWeigth();

        visualController.CurrentWeaponModel().ReloadSfx.Stop();
        weaponController.CurrentWeapon().RefillBullets();

        weaponController.SetWeaponReady(true);
        weaponController.UpdateWeaponUI();
    }

    public void ReturnRig()
    {
        visualController.MaximizeRigWeigth();
        visualController.MaximizeLeftHandIKWeigth();
    }
    public void WeaponEquipingIsOver()
    {
        weaponController.SetWeaponReady(true);
    }

    public void SwitchOnWeaponModel() => visualController.SwitchOnCurrentWeaponModel();
}
