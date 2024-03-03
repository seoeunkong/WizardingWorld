using UnityEngine;

public abstract class BaseState
{
    protected PlayerController Controller { get; private set; }

    public BaseState(PlayerController controller)
    {
        this.Controller = controller;
    }

    public abstract void OnEnterState();
    public abstract void OnUpdateState();
    public abstract void OnFixedUpdateState();
    public abstract void OnExitState();
}