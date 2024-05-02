using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public enum StateName
{
    MOVE = 100,
    PUNCHATTACK,
    ATTACK,
    THROW,
    IDLE,
    MRUN,
    MCHASE,
    MATTACK,
    MHIT,
    MDEAD,
    BMIDLE,
    BMMISSILE,
    BMLASER,
    BMMELEE,
    BMCHASE,
    BMHIT,
    BMDEAD

}

public class StateMachine<TController> where TController : MonoBehaviour
{
    public BaseState<TController> CurrentState { get; private set; } //���� ���� 
    private Dictionary<StateName, BaseState<TController>> _states = new Dictionary<StateName, BaseState<TController>>();


    public StateMachine(StateName stateName, BaseState<TController> state)
    {
        AddState(stateName, state);
        CurrentState = GetState(stateName);
    }

    public void AddState(StateName stateName, BaseState<TController> state)  // ���� ���
    {
        if (!_states.ContainsKey(stateName))
        {
            _states.Add(stateName, state);
        }
    }

    public BaseState<TController> GetState(StateName stateName)  // ���� ��������
    {
        if (_states.TryGetValue(stateName, out BaseState<TController> state))
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
        if (_states.TryGetValue(nextStateName, out BaseState<TController> newState)) // ���� ��ȯ
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

