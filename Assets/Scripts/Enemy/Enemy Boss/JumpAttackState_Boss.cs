using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAttackState_Boss : EnemyState
{
    public Enemy_Boss Enemy;
    private Vector3 lastPlayerPos;

    private float jumpAttackMovementSpeed;
    public JumpAttackState_Boss(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        lastPlayerPos = Enemy.player.position;
        Enemy.agent.isStopped = true;
        Enemy.agent.velocity = Vector3.zero;

        Enemy.BossVisuals.PlaceLadingZone(lastPlayerPos);
        Enemy.BossVisuals.EnableTrails(true);

        float distanceToPlayer = Vector3.Distance(lastPlayerPos, Enemy.transform.position);

        jumpAttackMovementSpeed = distanceToPlayer / Enemy.TravelTimeToTarget;

        Enemy.FaceTarget(lastPlayerPos, 1000);
    }

    public override void Update()
    {
        base.Update();
        Vector3 myPos = Enemy.transform.position;
        Enemy.agent.enabled = !Enemy.ManualMovementActive();

        if (Enemy.ManualMovementActive())
        {
            Enemy.transform.position = Vector3.MoveTowards(myPos, lastPlayerPos, jumpAttackMovementSpeed * Time.deltaTime);
        }
        if (triggerCalled)
        {
            stateMachine.ChangeState(Enemy.MoveState);
        }
    }
    public override void Exit()
    {
        base.Exit();

        Enemy.SetJumpAttackOnCooldown();
        Enemy.BossVisuals.EnableTrails(false);
    }
}
