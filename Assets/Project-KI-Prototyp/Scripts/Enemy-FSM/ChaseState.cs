using System;
using UnityEngine;

public class ChaseState : StateBase
{
    public ChaseState(EnemyController enemyController, PlayerController playerController) : base(enemyController, playerController)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        enemyController.ShowExclamationMark(true);
        enemyController.RaiseArms(true);
    }

    public override void OnUpdate(IFiniteStateMachine ctx)
    {
        if (enemyController == null)
            return;

        enemyController.MoveToPlayer(playerController, true);

        //if (!enemyController.DetectorAura.IsPlayerInSights(playerController))
        //{
        //    ctx.SetState(new PatrolState(owner, playerController));
        //    return;
        //}

        //if (enemyController.AttackArea.IsPlayerInRange(playerController))
        //{
        //    ctx.SetState(new AttackState(enemyController, playerController));
        //    return;
        //}
    }

    public override void OnExit(IFiniteStateMachine ctx)
    {
        enemyController.MoveToPlayer(playerController, false);
        enemyController.ShowExclamationMark(false);
        enemyController.RaiseArms(false);
    }
}
