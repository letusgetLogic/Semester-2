using System;
using UnityEngine;

public class IdleState : StateBase
{
    public IdleState(EnemyController enemyController, PlayerController playerController) : base(enemyController, playerController)
    {
    }

    public override void OnUpdate(IFiniteStateMachine ctx)
    {
        // wait until instance of job system is created 
        if (EnemyJobSystem.Instance == null)
            return;

        ctx.SetState(new PatrolState(enemyController, null));
    }
}