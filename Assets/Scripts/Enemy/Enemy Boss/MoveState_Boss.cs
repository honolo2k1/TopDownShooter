using UnityEngine;

public class MoveState_Boss : EnemyState
{
    public Enemy_Boss Enemy;
    private Vector3 destination;

    private float actionTimer;
    private float timeBeforeSpeedUp = 5;

    private bool speedUpActived;
    public MoveState_Boss(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        SpeedReset();
        Enemy.agent.isStopped = false;

        destination = Enemy.GetPatrolDestination();

        Enemy.agent.SetDestination(destination);

        actionTimer = Enemy.ActionCooldown;
    }

    private void SpeedReset()
    {
        speedUpActived = false;
        Enemy.anim.SetFloat("MoveAnimIndex", 0);
        Enemy.anim.SetFloat("MoveAnimSpeedMultiplier", 1f);
        Enemy.agent.speed = Enemy.WalkSpeed;
    }
    private void SpeedUp()
    {
        Enemy.agent.speed = Enemy.RunSpeed;
        Enemy.anim.SetFloat("MoveAnimIndex", 1);
        Enemy.anim.SetFloat("MoveAnimSpeedMultiplier", 1.5f);
        speedUpActived = true;
    }
    public override void Update()
    {
        base.Update();

        actionTimer -= Time.deltaTime;
        Enemy.FaceTarget(GetNextPathPoint());

        if (Enemy.InBattleMode)
        {
            if (ShouldSpeedUp())
            {
                SpeedUp();
            }

            Vector3 playerPos = Enemy.player.position;
            Enemy.agent.SetDestination(playerPos);

            if (actionTimer < 0)
            {
                PerformRandomAction();
            }
            else if (Enemy.PlayerInAttackRange())
            {
                stateMachine.ChangeState(Enemy.AttackState);
            }
        }
        else
        {
            if (Vector3.Distance(Enemy.transform.position, destination) < 0.25f)
            {
                stateMachine.ChangeState(Enemy.IdleState);
            }
        }
    }

    private void PerformRandomAction()
    {
        actionTimer = Enemy.ActionCooldown;

        if (Random.Range(0, 2) == 0)
        {
            TryAbility();
        }
        else
        {
            if (Enemy.CanDoJumpAttack())
            {
                stateMachine.ChangeState(Enemy.JumpAttackState);
            }
            else if (Enemy.BossWeaponType == Enums.BossWeaponType.Fist)
            {
                TryAbility();
            }
        }
    }

    private void TryAbility()
    {
        if (Enemy.CanDoAbility())
        {
            stateMachine.ChangeState(Enemy.AbilityState);
        }
    }

    private bool ShouldSpeedUp()
    {
        if (speedUpActived)
        {
            return false;
        }
        if (Time.time > Enemy.AttackState.LastTimeAttacked + timeBeforeSpeedUp)
        {
            return true;
        }
        return false;
    }
}
