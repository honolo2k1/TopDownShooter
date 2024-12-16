using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_WeaponSelection : MonoBehaviour
{
    [SerializeField] private GameObject nextUIToSwitchOn;
    public UI_SelectedWeaponWindow[] SelectedWeapon;

    [Header("Warning Info")]
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private float disaperaingSpeed = 0.25f;

    private float currentWarningAlpha;
    private float targetWarningAlpha;

    private void Start()
    {
        SelectedWeapon = GetComponentsInChildren<UI_SelectedWeaponWindow>();
    }
    private void Update()
    {
        if (currentWarningAlpha > targetWarningAlpha)
        {
            currentWarningAlpha -= Time.deltaTime * disaperaingSpeed;
            warningText.color = new Color(1, 1, 1, currentWarningAlpha);
        }
    }

    public void ConfirmWeaponSelection()
    {
        if (AtLeastOneWeaponSelected())
        {
            StartCoroutine(WaitForLevelGeneration());
        }
        else
        {
            ShowWarningMessage("Select at least one weapon.");
        }
    }

    private IEnumerator WaitForLevelGeneration()
    {
        // Bắt đầu tạo level
        UI.Instance.StartLevelGeneration();

        // Chờ cho đến khi generationOver là true
        while (!LevelGenarator.Instance.generationOver)
        {
            yield return null; // Đợi đến frame tiếp theo
        }

        // Sau khi hoàn thành, chuyển sang UI tiếp theo
        UI.Instance.SwitchToUI(nextUIToSwitchOn);
    }

    private bool AtLeastOneWeaponSelected() => SelectedWeaponData().Count > 0;
    public List<Weapon_Data> SelectedWeaponData()
    {
        List<Weapon_Data> selectedData = new List<Weapon_Data>();

        foreach (var weapon in SelectedWeapon)
        {
            if (weapon.WeaponData != null)
            {
                selectedData.Add(weapon.WeaponData);
            }
        }

        return selectedData;
    }

    public UI_SelectedWeaponWindow FindEmptySlot()
    {
        for (int i = 0; i < SelectedWeapon.Length; i++)
        {
            if (SelectedWeapon[i].IsEmpty())
            {
                return SelectedWeapon[i];
            }
        }
        return null;
    }

    public UI_SelectedWeaponWindow FindSlowWithWeaponOfType(Weapon_Data weaponData)
    {
        for (int i = 0; i < SelectedWeapon.Length; i++)
        {
            if (SelectedWeapon[i].WeaponData == weaponData)
            {
                return SelectedWeapon[i];
            }
        }
        return null;
    }

    public void ShowWarningMessage(string message)
    {
        warningText.color = Color.white;
        warningText.text = message;

        currentWarningAlpha = warningText.color.a;
        targetWarningAlpha = 0;
    }
}
