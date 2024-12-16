using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancePlayerState_Range : EnemyState
{
    public Enemy_Range Enemy;
    private Vector3 playerPos;

    public float LastTimeAdvance { get; private set; }
    public AdvancePlayerState_Range(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        Enemy.visuals.EnableIK(true, true);

        Enemy.agent.isStopped = false;
        Enemy.agent.speed = Enemy.AdvanceSpeed;

        if (Enemy.IsUnstoppable())
        {
            Enemy.visuals.EnableIK(true, false);
            stateTimer = Enemy.AdvanceDuration;
        }
    }
    public override void Exit()
    {
        base.Exit();
        LastTimeAdvance = Time.time;
    }
    public override void Update()
    {
        base.Update();
        playerPos = Enemy.player.transform.position;

        Enemy.UpdateAimPosition();

        Enemy.agent.SetDestination(Enemy.player.transform.position);
        Enemy.FaceTarget(GetNextPathPoint());

        if (CanEnterBattleState() && Enemy.IsSeeingPlayer())
        {
            stateMachine.ChangeState(Enemy.BattleState);
        }
    }

    private bool CanEnterBattleState()
    {
        bool closeEnoughToPlayer = Vector3.Distance(Enemy.transform.position, playerPos) < Enemy.AdvanceStoppingDistance;
        if (Enemy.IsUnstoppable())
        {
            return closeEnoughToPlayer || stateTimer < 0;
        }
        else
        {
            return closeEnoughToPlayer;
        }
    }
}
