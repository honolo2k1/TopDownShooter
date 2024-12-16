public class RecoveryState_Melee : EnemyState
{
    public Enemy_Melee Enemy;
    public RecoveryState_Melee(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        Enemy.agent.isStopped = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        Enemy.FaceTarget(Enemy.player.position);

        if (triggerCalled)
        {
            if (Enemy.CanThrowAxe())
            {
                stateMachine.ChangeState(Enemy.AbilityState);
            }

            else if (Enemy.PlayerInAttackRange())
            {
                stateMachine.ChangeState(Enemy.AttackState);
            }
            else
            {
                stateMachine.ChangeState(Enemy.ChaseState);
            }
        }
    }
}
