using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AnimationEvents : MonoBehaviour
{
    private Enemy enemy;
    private Enemy_Melee enemyMelee;
    private Enemy_Boss enemyBoss;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        enemyMelee = GetComponentInParent<Enemy_Melee>();
        enemyBoss = GetComponentInParent<Enemy_Boss>();
    }

    public void AnimationTrigger() => enemy.AnimationTrigger();

    public void StartManualMovement() => enemy.ActiveManualMovement(true);

    public void StopManualMovement() => enemy.ActiveManualMovement(false);

    public void StartManualRotation() => enemy.ActiveManualRotation(true);

    public void StopManualRotation() => enemy.ActiveManualRotation(false);

    public void AbilityEvent()
    {
        enemy.AbilityTrigger();

        enemy?.audioManager.PlaySFX(enemyBoss?.BossSFX.ability, true);
    }

    public void EnableIK() => enemy.visuals.EnableIK(true, true, 1f);

    public void EnableWeaponModel()
    {
        enemy.visuals.EnableWeaponModel(true);
        enemy.visuals.EnableSecondWeaponModel(false);
    }
    public void BossJumpImpact()
    {
        enemyBoss?.JumpImpact();

        enemy?.audioManager.PlaySFX(enemyBoss?.BossSFX.jump, true);
    }
    public void BeginMeleeAttackCheck()
    {
        enemy?.EnableMeleeAttackCheck(true);

        enemy?.audioManager.PlaySFX(enemyMelee?.MeleeSFX.swoosh, true);
    }
    public void FinishMeleeAttackCheck()
    {
        enemy?.EnableMeleeAttackCheck(false);
    }
}
