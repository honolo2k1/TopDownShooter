using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState_Melee : EnemyState
{
    public Enemy_Melee Enemy;

    private bool interactionDisable;
    public DeadState_Melee(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        interactionDisable = false;


        stateTimer = 1.5f;
    }

    public override void Exit()
    {
        base.Exit();
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
