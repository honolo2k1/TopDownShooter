using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class AttackState_Melee : EnemyState
{
    public Enemy_Melee Enemy;
    private Vector3 attackDirection;
    private float attackMoveSpeed;

    private const float MAX_ATTACK_DISTANCE = 50f;
    public AttackState_Melee(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();
        Enemy.UpdateAttackData();
        Enemy.visuals.EnableWeaponModel(true);
        Enemy.visuals.EnableWeaponTrail(true);

        attackMoveSpeed = Enemy.AttackData.MoveSpeed;
        Enemy.anim.SetFloat("AttackAnimationSpeed", Enemy.AttackData.AnimationSpeed);
        Enemy.anim.SetFloat("AttackIndex", Enemy.AttackData.AttackIndex);
        Enemy.anim.SetFloat("SlashAttackIndex", Random.Range(0, 6));

        Enemy.agent.isStopped = true;
        Enemy.agent.velocity = Vector3.zero;

        attackDirection = Enemy.transform.position + (Enemy.transform.forward * MAX_ATTACK_DISTANCE);
    }

    public override void Exit()
    {
        base.Exit();
        SetupNextAttack();
        Enemy.visuals.EnableWeaponTrail(false);
    }

    private void SetupNextAttack()
    {
        int recoveryIndex = PlayerClose() ? 1 : 0;
        Enemy.anim.SetFloat("RecoveryIndex", recoveryIndex);

        Enemy.AttackData = UpdateAttackData();
    }

    public override void Update()
    {
        base.Update();

        if (Enemy.ManualRotationActive())
        {
            Enemy.FaceTarget(Enemy.player.position);
            attackDirection = Enemy.transform.position + (Enemy.transform.forward * MAX_ATTACK_DISTANCE);
        }

        if (Enemy.ManualMovementActive())
        {
            Enemy.transform.position = Vector3.MoveTowards(Enemy.transform.position, attackDirection, attackMoveSpeed * Time.deltaTime);
        }

        if (triggerCalled)
        {
            if (Enemy.PlayerInAttackRange())
            {
                stateMachine.ChangeState(Enemy.RecoveryState);
            }
            else
            {
                stateMachine.ChangeState(Enemy.ChaseState);
            }
        }
    }

    private bool PlayerClose() => Vector3.Distance(Enemy.transform.position, Enemy.player.position) <= 1;

    private AttackData_EnemyMelee UpdateAttackData()
    {
        List<AttackData_EnemyMelee> validAttacks = new List<AttackData_EnemyMelee>(Enemy.AttackList);

        if (PlayerClose())
        {
            validAttacks.RemoveAll(parameter => parameter.AttackType == AttackType_Melee.Charge);
        }

        int random = Random.Range(0, validAttacks.Count);
        return validAttacks[random];
    }
}
