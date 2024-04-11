using System.Collections.Generic;
using System.Diagnostics;

public enum StateName
{
    MOVE = 100,
    PUNCHATTACK,
    ATTACK,
    THROW
}

public class StateMachine
{
    public BaseState CurrentState { get; private set; }  // ���� ����
    private Dictionary<StateName, BaseState> _states =
                                        new Dictionary<StateName, BaseState>();


    public StateMachine(StateName stateName, BaseState state)
    {
        AddState(stateName, state);
        CurrentState = GetState(stateName);
    }

    public void AddState(StateName stateName, BaseState state)  // ���� ���
    {
        if (!_states.ContainsKey(stateName))
        {
            _states.Add(stateName, state);
        }
    }

    public BaseState GetState(StateName stateName)  // ���� ��������
    {
        if (_states.TryGetValue(stateName, out BaseState state))
            return state;
        return null;
    }

    public void DeleteState(StateName removeStateName)  // ���� ����
    {
        if (_states.ContainsKey(removeStateName))
        {
            _states.Remove(removeStateName);
        }
    }

    public void ChangeState(StateName nextStateName)    // ���� ��ȯ
    {
        CurrentState?.OnExitState();   //���� ���¸� �����ϴ� �޼ҵ带 �����ϰ�,
        if (_states.TryGetValue(nextStateName, out BaseState newState)) // ���� ��ȯ
        {
            CurrentState = newState;
        }
        CurrentState?.OnEnterState();  // ���� ���� ���� �޼ҵ� ����
    }

    public void UpdateState()
    {
        CurrentState?.OnUpdateState();
    }

    public void FixedUpdateState()
    {
        CurrentState?.OnFixedUpdateState();
    }
}

