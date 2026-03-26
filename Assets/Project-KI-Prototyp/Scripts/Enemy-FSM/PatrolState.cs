using System;
using UnityEngine;

public class PatrolState : StateBase
{
    public PatrolState(EnemyController enemyController, PlayerController playerController) : base(enemyController, playerController)
    {
    }

    private float actionInterval = 3f;

    private int nothingChance = 20;
    private int lookAroundChance = 30;
    private int walkChance = 50;

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        enemyController.CurrentAction = WhatAmIDoingNext.Nothing;

        if (EnemyJobSystem.Instance.UseCountdownJob)
        {
            EnemyJobSystem.Instance.AddEnemyToList(
                EnemyJobSystem.Instance.CountdownEnemyControllers, enemyController);
        }
    }

    public override void OnUpdate(IFiniteStateMachine ctx)
    {
        // countdown job not used
        if (!(EnemyJobSystem.Instance.UseCountdownJob))
            enemyController.Countdown -= Time.deltaTime;

        if (enemyController.Countdown <= 0f)
        {
            RemoveFromJobSystem();
            SetAction();
            enemyController.Countdown = actionInterval; // Reset the countdown
        }
        DoAction(Time.deltaTime);
    }

    public override void OnExit(IFiniteStateMachine ctx)
    {
        enemyController.CurrentAction = WhatAmIDoingNext.Nothing;

        if (EnemyJobSystem.Instance.UseCountdownJob)
        {
            EnemyJobSystem.Instance.RemoveEnemyFromList(
                EnemyJobSystem.Instance.CountdownEnemyControllers, enemyController);
        }
    }

    /// <summary>
    /// Remove enemy controller from list in job system.
    /// </summary>
    private void RemoveFromJobSystem()
    {
        switch (enemyController.CurrentAction)
        {
            case WhatAmIDoingNext.Nothing:
                break;

            case WhatAmIDoingNext.LookAround:
                if (EnemyJobSystem.Instance.UseRotationJob)
                {
                    EnemyJobSystem.Instance.RemoveEnemyFromList(
                        EnemyJobSystem.Instance.RotateEnemyControllers, enemyController);
                }
                break;

            case WhatAmIDoingNext.Walk:
                if (EnemyJobSystem.Instance.UseForwardJob)
                {
                    EnemyJobSystem.Instance.RemoveEnemyFromList(
                        EnemyJobSystem.Instance.ForwardEnemyControllers, enemyController);
                }
                break;
        }
    }

    /// <summary>
    /// Set the next action.
    /// </summary>
    private void SetAction()
    {
        var action = GetRandomAction();

        switch (action)
        {
            case WhatAmIDoingNext.Nothing:
                enemyController.CurrentAction = WhatAmIDoingNext.Nothing;
                //Debug.Log(action);
                break;

            case WhatAmIDoingNext.LookAround:
                enemyController.SetRandomRotation();
                enemyController.CurrentAction = WhatAmIDoingNext.LookAround;
                //Debug.Log(action);
                if (EnemyJobSystem.Instance.UseRotationJob)
                {
                    EnemyJobSystem.Instance.AddEnemyToList(
                         EnemyJobSystem.Instance.RotateEnemyControllers, enemyController);
                }
                break;

            case WhatAmIDoingNext.Walk:
                enemyController.CurrentAction = WhatAmIDoingNext.Walk;
                //Debug.Log(action);
                if (EnemyJobSystem.Instance.UseForwardJob)
                {
                    EnemyJobSystem.Instance.AddEnemyToList(
                        EnemyJobSystem.Instance.ForwardEnemyControllers, enemyController);
                }
                break;
        }
    }

    /// <summary>
    /// Do the current action.
    /// </summary>
    /// <param name="deltaTime"></param>
    private void DoAction(float deltaTime)
    {
        switch (enemyController.CurrentAction)
        {
            case WhatAmIDoingNext.Nothing:
                break;

            case WhatAmIDoingNext.LookAround:
                if (EnemyJobSystem.Instance.UseRotationJob)
                    break;
                enemyController.Rotate(deltaTime);
                break;

            case WhatAmIDoingNext.Walk:
                if (EnemyJobSystem.Instance.UseForwardJob)
                    break;
                enemyController.MoveWithSpeed(deltaTime);
                break;
        }
    }


    /// <summary>
    /// Gets a random action for the AI to perform while in idle state.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private WhatAmIDoingNext GetRandomAction()
    {
        int rnd = UnityEngine.Random.Range(0, 100);

        if (rnd < nothingChance)
        {
            return WhatAmIDoingNext.Nothing;
        }
        else if (rnd >= nothingChance && rnd < nothingChance + lookAroundChance)
        {
            return WhatAmIDoingNext.LookAround;
        }
        else if (rnd >= walkChance)
        {
            return WhatAmIDoingNext.Walk;
        }

        return WhatAmIDoingNext.Nothing; // Default case, should not happen
    }
}

