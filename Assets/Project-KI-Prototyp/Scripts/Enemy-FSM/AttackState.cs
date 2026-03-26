using System;
using UnityEngine;

public class AttackState : StateBase
{
    public AttackState(EnemyController enemyController, PlayerController playerController) : base(enemyController, playerController)
    {
    }

    private float attackInterval = 2f;
    private float animationDuration = 0.2f; // Duration of the attack animation

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        enemyController.ShowExclamationMark(true);
        enemyController.RaiseArms(true);

        if (EnemyJobSystem.Instance != null && EnemyJobSystem.Instance.UseCountdownJob)
        {
            EnemyJobSystem.Instance.AddEnemyToList(
                EnemyJobSystem.Instance.CountdownEnemyControllers, enemyController);
        }
    }

    public override void OnUpdate(IFiniteStateMachine ctx)
    {
        //if (!enemyController.AttackArea.IsPlayerInRange(playerController))
        //{
        //    ctx.SetState(new ChaseState(enemyController, playerController));
        //    return;
        //}

        // countdown job not used
        if (!(EnemyJobSystem.Instance != null && EnemyJobSystem.Instance.UseCountdownJob))
            enemyController.Countdown -= Time.deltaTime;

        if (enemyController.Countdown <= 0f) // attack
        {
            enemyController.TiltBodyWhileAttacking(true);
            enemyController.Countdown = attackInterval; // Reset the countdown
        }

        if (enemyController.Countdown <= attackInterval - animationDuration) // animation
        {
            enemyController.TiltBodyWhileAttacking(false);
        }
    }

    public override void OnExit(IFiniteStateMachine ctx)
    {
        enemyController.TiltBodyWhileAttacking(false);
        enemyController.ShowExclamationMark(false);
        enemyController.RaiseArms(false);

        if (EnemyJobSystem.Instance != null && EnemyJobSystem.Instance.UseCountdownJob)
        {
            EnemyJobSystem.Instance.RemoveEnemyFromList(
                EnemyJobSystem.Instance.CountdownEnemyControllers, enemyController);
        }
    }
}