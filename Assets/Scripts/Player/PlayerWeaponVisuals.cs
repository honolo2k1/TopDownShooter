using UnityEngine;
using UnityEngine.Animations.Rigging;
using static Enums;

public class PlayerWeaponVisuals : MonoBehaviour
{
    private Animator anim;
    private Player player;


    [SerializeField] private WeaponModel[] weaponModels;
    [SerializeField] private BackupWeaponModel[] backupWeaponModels;


    [Header("Rig")]
    [SerializeField] private float rigWeigthIncreaseRate;
    private bool shouldIncreaseRigWeigth;
    private Rig rig;

    [Header("Left Hand IK")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandIK_Target;
    [SerializeField] private float leftHandIKWeigthIncreaseRate;
    private bool shouldIncreaseLeftHandIKWeigth;


    private void Start()
    {
        player = GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        backupWeaponModels = GetComponentsInChildren<BackupWeaponModel>(true);


    }
    private void Update()
    {

        UpdateRigWeigth();
        UpdateLeftHandIKWeigth();

    }
    public void PlayFireAnimation() => anim.SetTrigger("Fire");
    public void PlayReloadAnimation()
    {
        float reloadSpeed = player.Weapon.CurrentWeapon().ReloadSpeed;
        anim.SetTrigger("Reload");
        anim.SetFloat("ReloadSpeed", reloadSpeed);
        ReduceRigWeigth();
    }

    public void PlayWeaponEquipAnimation()
    {
        EquipType equipType = CurrentWeaponModel().EquipType;

        float equipmentSpeed = player.Weapon.CurrentWeapon().EquipmentSpeed;

        leftHandIK.weight = 0.0f;
        ReduceRigWeigth();
        anim.SetTrigger("EquipWeapon");
        anim.SetFloat("EquipType", ((float)equipType));
        anim.SetFloat("EquipSpeed", equipmentSpeed);
    }

    public void SwitchOnCurrentWeaponModel()
    {
        int animationIndex = ((int)CurrentWeaponModel().HoldType);

        SwitchOffWeaponModels();
        SwitchOffBackupWeaponModels();

        if (player.Weapon.HasOnlyOneWeapon() == false)
        {
            SwitchOnBackupWeaponModel();
        }

        SwitchAnimationLayer(animationIndex);
        CurrentWeaponModel().gameObject.SetActive(true);
        AtachLeftHand();
    }

    public void SwitchOffWeaponModels()
    {
        for (int i = 0; i < weaponModels.Length; i++)
        {
            weaponModels[i].gameObject.SetActive(false);
        }
    }

    public void SwitchOnBackupWeaponModel()
    {
        SwitchOffBackupWeaponModels();

        BackupWeaponModel lowHangWeapon = null;
        BackupWeaponModel backHangWeapon = null;
        BackupWeaponModel sideHangWeapon = null;

        foreach (BackupWeaponModel backupWeaponModel in backupWeaponModels)
        {
            if (backupWeaponModel.WeaponType == player.Weapon.CurrentWeapon().WeaponType) continue;

            if (player.Weapon.WeaponInSlots(backupWeaponModel.WeaponType) != null)
            {
                if (backupWeaponModel.HangTypeIs(HangType.LowBackHang))
                {
                    lowHangWeapon = backupWeaponModel;
                }
                if (backupWeaponModel.HangTypeIs(HangType.BackHang)/* && backHangWeapon != null*/)
                {
                    backHangWeapon = backupWeaponModel;
                }
                if (backupWeaponModel.HangTypeIs(HangType.SideHang))
                {
                    sideHangWeapon = backupWeaponModel;
                }
            }
        }
        lowHangWeapon?.Activate(true);
        backHangWeapon?.Activate(true);
        sideHangWeapon?.Activate(true);
    }

    public void SwitchOffBackupWeaponModels()
    {
        foreach (BackupWeaponModel backupWeaponModel in backupWeaponModels)
        {
            backupWeaponModel.Activate(false);
        }
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }

    public WeaponModel CurrentWeaponModel()
    {
        WeaponModel weaponModel = null;

        WeaponType weaponType = player.Weapon.CurrentWeapon().WeaponType;

        for (int i = 0; i < weaponModels.Length; i++)
        {
            if (weaponModels[i].WeaponType == weaponType)
            {
                weaponModel = weaponModels[i];
            }
        }

        return weaponModel;
    }

    #region Animation Rigging Methods
    private void AtachLeftHand()
    {
        Transform targetTransform = CurrentWeaponModel().holdPoint;

        leftHandIK_Target.localPosition = targetTransform.localPosition;
        leftHandIK_Target.localRotation = targetTransform.localRotation;
    }
    private void UpdateLeftHandIKWeigth()
    {
        if (shouldIncreaseLeftHandIKWeigth)
        {
            leftHandIK.weight += leftHandIKWeigthIncreaseRate * Time.deltaTime;

            if (leftHandIK.weight >= 1.0f)
            {
                shouldIncreaseLeftHandIKWeigth = false;
            }
        }
    }

    private void UpdateRigWeigth()
    {
        if (shouldIncreaseRigWeigth)
        {
            rig.weight += rigWeigthIncreaseRate * Time.deltaTime;

            if (rig.weight >= 1.0f)
            {
                shouldIncreaseRigWeigth = false;
            }
        }
    }

    private void ReduceRigWeigth()
    {
        rig.weight = 0.15f;
    }
    public void MaximizeRigWeigth() => shouldIncreaseRigWeigth = true;
    public void MaximizeLeftHandIKWeigth() => shouldIncreaseLeftHandIKWeigth = true;
    #endregion
}
