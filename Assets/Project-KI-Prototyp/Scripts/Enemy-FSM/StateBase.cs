using UnityEngine;

public abstract class StateBase
{
    protected EnemyController enemyController;
    protected PlayerController playerController;

    public StateBase(EnemyController enemyController, PlayerController playerController)
    {
        this.enemyController = enemyController;
        this.playerController = playerController;
    }

    /// <summary>
    /// OnEnter is called when the state is entered.
    /// </summary>
    public virtual void OnEnter(IFiniteStateMachine ctx)
    {}

    /// <summary>
    /// OnUpdate is called every frame while the state is active.
    /// </summary>
    public abstract void OnUpdate(IFiniteStateMachine ctx);

    /// <summary>
    /// OnExit is called when the state is exited.
    /// </summary>
    public virtual void OnExit(IFiniteStateMachine ctx)
    {}
}
