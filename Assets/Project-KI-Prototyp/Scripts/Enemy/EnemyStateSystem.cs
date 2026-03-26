using System;
using UnityEngine;

/// <summary>
/// Update-methods have been called by a manager.
/// </summary>
public class EnemyStateSystem : MonoBehaviour, IFiniteStateMachine
{
    // public to debug in inspector
    public string CurrentState;
    private StateBase currentState = null;

    [SerializeField]
    private EnemyController enemyController;

    private void OnEnable()
    {
        SetState(new IdleState(enemyController, null));
    }

    public void UpdateMe()
    {
        if (currentState == null)
            return;

        CurrentState = currentState.ToString();
        currentState.OnUpdate(this);
    }

    public void OnDisable()
    {
        currentState = null;
    }

    /// <summary>
    /// When player is detected, current state is changed to chase state, otherwise to patrol state.
    /// </summary>
    public void DetectPlayer(PlayerController playerController, bool value)
    {
        if (value == true)
            SetState(new ChaseState(enemyController, playerController));
        else
            SetState(new PatrolState(enemyController, playerController));
    }

    /// <summary>
    ///  When player is detected, current state is changed to attack state, otherwise to chase state.
    /// </summary>
    /// <param name="playerController"></param>
    /// <param name="value"></param>
    public void AttackPlayer(PlayerController playerController, bool value)
    {
        if (value == true)
            SetState(new AttackState(enemyController, playerController));
        else
            SetState(new ChaseState(enemyController, playerController));
    }

    /// <summary>
    /// SetState is called to change the current state of the finite state machine.
    /// </summary>
    /// <param name="state"></param>
    public void SetState(StateBase state)
    {
        if (currentState != null)
            currentState.OnExit(this);
        currentState = state;
        Debug.Log($"State changed to: {currentState.GetType().Name}");
        currentState.OnEnter(this);
    }


}

