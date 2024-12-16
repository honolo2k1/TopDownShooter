using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunToCoverState_Range : EnemyState
{
    public Enemy_Range Enemy;
    private Vector3 destination;
    
    public float LastTimeTookCover {  get; private set; }
    public RunToCoverState_Range(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();
        destination = Enemy.CurrentCover.transform.position;

        Enemy.visuals.EnableIK(true, false);

        Enemy.agent.isStopped = false;
        Enemy.agent.speed = Enemy.RunSpeed;

        Enemy.agent.SetDestination(destination);

     
    }

    public override void Exit()
    {
        base.Exit();

        LastTimeTookCover = Time.time;
    }

    public override void Update()
    {
        base.Update();
        Enemy.FaceTarget(GetNextPathPoint());

        if (Vector3.Distance(Enemy.transform.position, destination) < 0.7f)
        {
            stateMachine.ChangeState(Enemy.BattleState);
        }
    }
}
