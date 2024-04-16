using UnityEngine;

public abstract class BaseState<TController> where TController : MonoBehaviour
{
    protected TController _Controller { get; private set; }

    public BaseState(TController controller)
    {
        this._Controller = controller;
    }

    public abstract void OnEnterState();
    public abstract void OnUpdateState();
    public abstract void OnFixedUpdateState();
    public abstract void OnExitState();
}