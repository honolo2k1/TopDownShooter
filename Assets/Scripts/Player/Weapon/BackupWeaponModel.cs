using Unity.VisualScripting;
using UnityEngine;
using static Enums;

public class BackupWeaponModel : MonoBehaviour
{
    public WeaponType WeaponType;
    [SerializeField] private HangType hangType;

    public void Activate(bool activated) => gameObject.SetActive(activated);
    public bool HangTypeIs(HangType hangType) => this.hangType == hangType;
}
