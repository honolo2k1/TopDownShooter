using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityState_Boss : EnemyState
{
    public Enemy_Boss Enemy;
    public AbilityState_Boss(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = Enemy.FlamethrowDuration;
        Enemy.agent.isStopped = true;
        Enemy.agent.velocity = Vector3.zero;
        Enemy.BossVisuals.EnableTrails(true);
    }

    public override void Update()
    {
        base.Update();

        Enemy.FaceTarget(Enemy.player.position);

        if (ShouldDisableFlamethrower())
        {
            DisableFlamethrower();
        }
        if (triggerCalled)
        {
            stateMachine.ChangeState(Enemy.MoveState);
        }
    }

    private bool ShouldDisableFlamethrower()
    {
        return stateTimer < 0;
    }

    public void DisableFlamethrower()
    {
        if (Enemy.BossWeaponType != Enums.BossWeaponType.Flamethrower)
        {
            return;
        }
        if (Enemy.FlamethrowActive == false)
            return;
        Enemy.ActivateFlamethrower(false);
    }
    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        if (Enemy.BossWeaponType == Enums.BossWeaponType.Flamethrower)
        {
            Enemy.ActivateFlamethrower(true);
            Enemy.BossVisuals.EnableTrails(false);
        }

        if (Enemy.BossWeaponType == Enums.BossWeaponType.Fist)
        {
            Enemy.ActivateFirt();
        }

    }
    public override void Exit()
    {
        base.Exit();

        Enemy.SetAbilityOnCooldown();
    }
}
