using UnityEngine;

public class ChaseState_Melee : EnemyState
{
    public Enemy_Melee Enemy;
    private float lastTimeUpdatedDestination;

    public ChaseState_Melee(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        //CheckChaseAnimation();
        base.Enter();

        Enemy.agent.speed = Enemy.RunSpeed;

        Enemy.agent.isStopped = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (Enemy.PlayerInAttackRange())
        {
            stateMachine.ChangeState(Enemy.AttackState);
        }

        Enemy.FaceTarget(GetNextPathPoint());

        if (CanUpdateDestination())
        {
            Enemy.agent.destination = Enemy.player.position;
        }
    }

    public bool CanUpdateDestination()
    {
        if (Time.time > lastTimeUpdatedDestination + 0.25f)
        {
            lastTimeUpdatedDestination = Time.time;
            return true;
        }
        return false;
    }
    //private void CheckChaseAnimation()
    //{
    //    if (Enemy.MeleeType == Enums.EnemyMelee_Type.Shield && Enemy.shieldTransform == null)
    //    {
    //        Enemy.anim.SetFloat("ChaseIndex", 0);
    //    }
    //}
}
