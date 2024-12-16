using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState_Boss : EnemyState
{
    public Enemy_Boss Enemy;
    private bool interactionDisable;
    public DeadState_Boss(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        Enemy.AbilityState.DisableFlamethrower();

        interactionDisable = false;

        stateTimer = 1.5f;
    }

    public override void Update()
    {
        base.Update();
        //DisableInteraction();
    }

    private void DisableInteraction()
    {
        if (stateTimer <= 0 && !interactionDisable)
        {
            interactionDisable = true;
            Enemy.ragdoll.CollidersActive(false);
            Enemy.ragdoll.RagdollActive(true);
        }
    }
}
