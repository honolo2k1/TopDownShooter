using UnityEngine;

public class AttackState_Boss : EnemyState
{
    public Enemy_Boss Enemy;
    public float LastTimeAttacked {  get; private set; }
    public AttackState_Boss(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        Enemy.BossVisuals.EnableTrails(true);
        if (Enemy.BossWeaponType == Enums.BossWeaponType.Fist)
        {
            Enemy.anim.SetFloat("AttackAnimIndex", Random.Range(0, 5));
        }
        else
        {
            Enemy.anim.SetFloat("AttackAnimIndex", Random.Range(0, 2));
        }
        Enemy.agent.isStopped = true;

        stateTimer = 1f;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            Enemy.FaceTarget(Enemy.player.position, 20);

        if (triggerCalled)
        {
            if (Enemy.PlayerInAttackRange())
            {
                stateMachine.ChangeState(Enemy.IdleState);
            }
            else
            {
                stateMachine.ChangeState(Enemy.MoveState);
            }
        }

    }
    public override void Exit()
    {
        base.Exit();
        LastTimeAttacked = Time.time;
        Enemy.BossVisuals.EnableTrails(false);
    }
}
