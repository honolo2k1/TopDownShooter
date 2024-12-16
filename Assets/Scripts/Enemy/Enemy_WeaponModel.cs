using UnityEngine;
using static Enums;

public class Enemy_WeaponModel : MonoBehaviour
{
    public Enemy_MeleeWeaponType WeaponType;
    public AnimatorOverrideController OverrideController;
    public Enemy_MeleeWeaponData WeaponData;

    [SerializeField] private GameObject[] trailEffects;

    [Header("Damage atributes")]
    public Transform[] DamagePoints;
    public float AttackRadius;


    [ContextMenu("Assign damage point transforms")]
    private void GetDamagePoints()
    {
        DamagePoints = new Transform[trailEffects.Length];
        for (int i = 0; i < trailEffects.Length; i++)
        {
            DamagePoints[i] = trailEffects[i].transform;
        }
    }
    public void EnableTrailEffect(bool enable)
    {
        foreach (var effect in trailEffects)
        {
            effect.SetActive(enable);
        }
    }
    private void OnDrawGizmos()
    {
        if (DamagePoints.Length > 0)
        {
            foreach (Transform point in DamagePoints)
            {
                Gizmos.DrawWireSphere(point.position, AttackRadius);
            }
        }
    }
}
