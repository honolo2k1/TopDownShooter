using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityState_Melee : EnemyState
{
    public Enemy_Melee Enemy;
    private Vector3 movementDirection;
    private float moveSpeed;
    private float lastTimeAxeThrow;
    private const float MAX_MOVEMENT_DISTANCE = 20;
    public AbilityState_Melee(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        Enemy.visuals.EnableWeaponModel(true);

        moveSpeed = Enemy.WalkSpeed;
        movementDirection = Enemy.transform.position + (Enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
    }

    public override void Exit()
    {
        base.Exit();
        Enemy.WalkSpeed = moveSpeed;
        Enemy.anim.SetFloat("RecoveryIndex", 0);
    }

    public override void Update()
    {
        base.Update();

        if (Enemy.ManualRotationActive())
        {
            Enemy.FaceTarget(Enemy.player.position);
            movementDirection = Enemy.transform.position + (Enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
        }

        if (Enemy.ManualMovementActive())
        {
            Enemy.transform.position = Vector3.MoveTowards(Enemy.transform.position, movementDirection, moveSpeed * Time.deltaTime);
        }

        if (triggerCalled)
        {
            stateMachine.ChangeState(Enemy.RecoveryState);
        }
    }
    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        // Check if ObjectPool.Instance is valid
        if (ObjectPool.Instance == null)
        {
            Debug.LogError("ObjectPool.Instance is null!");
            return;
        }

        // Check if the AxaPrefab is assigned
        if (Enemy.AxaPrefab == null)
        {
            Debug.LogError("Enemy.AxaPrefab is null!");
            return;
        }

        // Check if AxeStartPoint is assigned
        if (Enemy.AxeStartPoint == null)
        {
            Debug.LogError("Enemy.AxeStartPoint is null!");
            return;
        }

        Enemy.ThrowAxe();
    }

}
