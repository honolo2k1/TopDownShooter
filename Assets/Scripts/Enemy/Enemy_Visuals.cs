using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static Enums;

public class Enemy_Visuals : MonoBehaviour
{
    public GameObject CurrentWeaponModel { get; private set; }
    public GameObject GrenadeModel;

    [Header("Color")]
    [SerializeField] private Material[] Body;
    [SerializeField] private Material[] Ammor;
    [SerializeField] private Material[] Eye;
    [SerializeField] private Material[] Gear;

    [SerializeField] private SkinnedMeshRenderer Belt;
    [SerializeField] private SkinnedMeshRenderer Mallearian;

    [Header("Rig References")]
    [SerializeField] private Transform leftHandIK;
    [SerializeField] private Transform leftElbowIK;
    [SerializeField] private TwoBoneIKConstraint leftHandIKConstraint;
    [SerializeField] private MultiAimConstraint weaponAimConstraint;

    private float leftHandTargetWeight;
    private float weaponAimTargetWeight;
    private float rigChangeRate;

    private void Awake()
    {
        // InvokeRepeating(nameof(SetupLook), 0, 1.5f);
    }

    private void Update()
    {
        // Skip IK update if both weights are already at target
        bool leftAtTarget = leftHandIKConstraint == null || Mathf.Abs(leftHandIKConstraint.weight - leftHandTargetWeight) <= 0.05f;
        bool weaponAtTarget = weaponAimConstraint == null || Mathf.Abs(weaponAimConstraint.weight - weaponAimTargetWeight) <= 0.05f;

        if (leftAtTarget && weaponAtTarget)
        {
            // Snap to exact target and skip
            if (leftHandIKConstraint != null) leftHandIKConstraint.weight = leftHandTargetWeight;
            if (weaponAimConstraint != null) weaponAimConstraint.weight = weaponAimTargetWeight;
            return;
        }

        if (leftHandIKConstraint != null)
        {
            leftHandIKConstraint.weight = AdjustIKWeight(leftHandIKConstraint.weight, leftHandTargetWeight);
        }

        if (weaponAimConstraint != null)
        {
            weaponAimConstraint.weight = AdjustIKWeight(weaponAimConstraint.weight, weaponAimTargetWeight);
        }
    }
    public void EnableGrenadeModel(bool active) => GrenadeModel?.SetActive(active); 
    public void EnableWeaponModel(bool active)
    {
        CurrentWeaponModel?.SetActive(active);
    }

    public void EnableSecondWeaponModel(bool active)
    {
        FindSeconderyWeaponModel()?.SetActive(active);
    }

    public void EnableWeaponTrail(bool enable)
    {
        if (CurrentWeaponModel != null)
        {
            Enemy_WeaponModel currentWeaponScript = CurrentWeaponModel.GetComponent<Enemy_WeaponModel>();
            currentWeaponScript?.EnableTrailEffect(enable);
        }
    }

    public void SetupLook()
    {
        SetupRandomColor();
        SetupRandomWeapon();
    }

    private void SetupRandomColor()
    {
        if (Belt != null && Mallearian != null)
        {
            // Generate random indices for each material array
            int randomBodyIndex = Random.Range(0, Body.Length);
            int randomAmmorIndex = Random.Range(0, Ammor.Length);
            int randomEyeIndex = Random.Range(0, Eye.Length);
            int randomGearIndex = Random.Range(0, Gear.Length);

            // Create material arrays for Belt and Mallearian
            Material[] beltMaterials = new Material[3];
            Material[] mallearianMaterials = new Material[3];

            // Assign random materials
            beltMaterials[0] = Body[randomBodyIndex];
            beltMaterials[1] = Ammor[randomAmmorIndex];
            beltMaterials[2] = Gear[randomGearIndex];

            mallearianMaterials[0] = Body[randomBodyIndex];
            mallearianMaterials[1] = Ammor[randomAmmorIndex];
            mallearianMaterials[2] = Eye[randomEyeIndex];

            // Apply the new materials
            Belt.materials = beltMaterials;
            Mallearian.materials = mallearianMaterials;
        }
    }

    private void SetupRandomWeapon()
    {
        bool thisEnemyIsMelee = GetComponent<Enemy_Melee>() != null;
        bool thisEnemyIsRange = GetComponent<Enemy_Range>() != null;

        if (thisEnemyIsMelee)
        {
            CurrentWeaponModel = FindMeleeWeaponModel();
        }
        else if (thisEnemyIsRange)
        {
            CurrentWeaponModel = FindRangeWeaponModel();
        }

        CurrentWeaponModel?.SetActive(true);
        OverrideAnimatorController();
    }

    private GameObject FindMeleeWeaponModel()
    {
        Enemy_WeaponModel[] weaponModels = GetComponentsInChildren<Enemy_WeaponModel>(true);
        Enemy_MeleeWeaponType weaponType = GetComponent<Enemy_Melee>().WeaponType;

        List<Enemy_WeaponModel> filteredWeaponModel = new List<Enemy_WeaponModel>();

        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.WeaponType == weaponType)
            {
                filteredWeaponModel.Add(weaponModel);
            }
        }

        if (filteredWeaponModel.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredWeaponModel.Count);
            return filteredWeaponModel[randomIndex].gameObject;
        }

        return null;
    }

    private GameObject FindRangeWeaponModel()
    {
        Enemy_RangeWeaponModel[] weaponModels = GetComponentsInChildren<Enemy_RangeWeaponModel>(true);
        Enemy_RangeWeaponType weaponType = GetComponent<Enemy_Range>().WeaponType;

        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.WeaponType == weaponType)
            {
                SwitchAnimationLayer((int)weaponModel.WeaponHoldType);
                SetupLeftHandIK(weaponModel.leftHandTarget, weaponModel.leftElbowTarget);
                return weaponModel.gameObject;
            }
        }

        return null;
    }

    private GameObject FindSeconderyWeaponModel()
    {
        Enemy_SecondRangeWeaponModel[] weaponModels = GetComponentsInChildren<Enemy_SecondRangeWeaponModel>(true);
        Enemy_RangeWeaponType weaponType = GetComponentInParent<Enemy_Range>().WeaponType;

        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
                return weaponModel.gameObject;
        }

        return null;
    }

    private void OverrideAnimatorController()
    {
        if (CurrentWeaponModel != null)
        {
            AnimatorOverrideController overrideController = CurrentWeaponModel.GetComponent<Enemy_WeaponModel>()?.OverrideController;
            if (overrideController != null)
            {
                GetComponentInChildren<Animator>().runtimeAnimatorController = overrideController;
            }
        }
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            for (int i = 1; i < anim.layerCount; i++)
            {
                anim.SetLayerWeight(i, 0);
            }

            anim.SetLayerWeight(layerIndex, 1);
        }
    }

    public void EnableIK(bool enableLeftHand, bool enableAnim, float changeRate = 1)
    {
        rigChangeRate = changeRate;
        leftHandTargetWeight = enableLeftHand ? 1 : 0;
        weaponAimTargetWeight = enableAnim ? 1 : 0;
    }

    private void SetupLeftHandIK(Transform leftHandTarget, Transform leftElbowTarget)
    {
        if (leftHandIK != null && leftElbowIK != null && leftHandTarget != null && leftElbowTarget != null)
        {
            leftHandIK.localPosition = leftHandTarget.localPosition;
            leftHandIK.localRotation = leftHandTarget.localRotation;

            leftElbowIK.localPosition = leftElbowTarget.localPosition;
            leftElbowIK.localRotation = leftElbowTarget.localRotation;
        }
    }

    private float AdjustIKWeight(float currentWeight, float targetWeight)
    {
        return Mathf.Abs(currentWeight - targetWeight) > 0.05f
            ? Mathf.Lerp(currentWeight, targetWeight, rigChangeRate * Time.deltaTime)
            : targetWeight;
    }
}
