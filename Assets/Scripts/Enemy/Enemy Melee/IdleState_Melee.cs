using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Melee : EnemyState
{
    public Enemy_Melee Enemy;
    public IdleState_Melee(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Melee;
    }
    public override void Enter()
    {
        base.Enter();

        stateTimer = enemyBase.IdleTime;
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
