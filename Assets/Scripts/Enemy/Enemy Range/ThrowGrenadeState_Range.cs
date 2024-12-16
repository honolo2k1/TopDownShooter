using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrenadeState_Range : EnemyState
{
    public Enemy_Range Enemy;
    public bool FinishedThrowingGrenade {  get; private set; } = true;
    public ThrowGrenadeState_Range(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        FinishedThrowingGrenade = false;

        Enemy.visuals.EnableWeaponModel(false);
        Enemy.visuals.EnableIK(false, false);

        Enemy.visuals.EnableSecondWeaponModel(true);
        Enemy.visuals.EnableGrenadeModel(true);
    }
    public override void Update()
    {
        base.Update();

        Enemy.FaceTarget(Enemy.player.position + Vector3.up);
        Enemy.Aim.position = Enemy.player.position + Vector3.up;

        if (triggerCalled)
            stateMachine.ChangeState(Enemy.BattleState);
    }
    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        FinishedThrowingGrenade = true;
        Enemy.ThrowGrenade();
    }
}
