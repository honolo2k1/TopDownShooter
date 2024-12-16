using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState_Range : EnemyState
{
    Enemy_Range Enemy;
    private Vector3 destination;
    public MoveState_Range(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        Enemy.agent.speed = Enemy.WalkSpeed;

        destination = Enemy.GetPatrolDestination();

        Enemy.agent.SetDestination(destination);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        Enemy.FaceTarget(GetNextPathPoint());

        if (Enemy.agent.remainingDistance <= Enemy.agent.stoppingDistance + 0.05f)
        {
            stateMachine.ChangeState(Enemy.IdleState);
        }
    }
}
