using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Boss : EnemyState
{
    public Enemy_Boss Enemy;
    public IdleState_Boss(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = Enemy.IdleTime;
    }

    public override void Update()
    {
        base.Update();

        if (Enemy.InBattleMode && Enemy.PlayerInAttackRange())
        {
            stateMachine.ChangeState(Enemy.AttackState);
        }

        if (stateTimer <= 0)
        {
            stateMachine.ChangeState(Enemy.MoveState);
        }
    }
}
