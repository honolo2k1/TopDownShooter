using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class IdlState_Range : EnemyState
{
    public Enemy_Range Enemy;
    public IdlState_Range(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Range;
    }
    public override void Enter()
    {
        base.Enter();

        Enemy.anim.SetFloat("IdleAnimIndex", Random.Range(0, 3));

        Enemy.visuals.EnableIK(true, false);

        if (Enemy.WeaponType == Enemy_RangeWeaponType.Pistol || Enemy.WeaponType == Enemy_RangeWeaponType.Shotgun || Enemy.WeaponType == Enemy_RangeWeaponType.Revolver)
        {
            Enemy.visuals.EnableIK(false, false);
        }
        stateTimer = Enemy.IdleTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(Enemy.MoveState);
        }
    }
}
