using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState_Range : EnemyState
{
    public Enemy_Range Enemy;
    private bool interactionDisable;
    public DeadState_Range(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        if (!Enemy.ThrowGrenadeState.FinishedThrowingGrenade)
        {
            Enemy.ThrowGrenade();
        }

        interactionDisable = false;

        stateTimer = 1.5f;
    }

    public override void Update()
    {
        base.Update();

        DisableInteraction();
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
