using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveState_Melee : EnemyState
{
    Enemy_Melee Enemy;
    private Vector3 destination;
    public MoveState_Melee(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        Enemy.agent.speed = Enemy.WalkSpeed;

        destination = Enemy.GetPatrolDestination();

        if (Enemy.agent.isActiveAndEnabled && Enemy.agent.isOnNavMesh)
        {
            Enemy.agent.SetDestination(destination);
        }
    }
    public override void Update()
    {
        base.Update();

        Enemy.FaceTarget(GetNextPathPoint());

        if (Enemy.agent.isActiveAndEnabled && Enemy.agent.isOnNavMesh)
        {
            if (Enemy.agent.remainingDistance <= Enemy.agent.stoppingDistance + 0.05f)
            {
                stateMachine.ChangeState(Enemy.IdleState);
            }
        }
    }
}
