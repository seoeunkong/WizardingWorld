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
    public BaseState<TController> CurrentState { get; private set; } //현재 상태 
    private Dictionary<StateName, BaseState<TController>> _states = new Dictionary<StateName, BaseState<TController>>();


    public StateMachine(StateName stateName, BaseState<TController> state)
    {
        AddState(stateName, state);
        CurrentState = GetState(stateName);
    }

    public void AddState(StateName stateName, BaseState<TController> state)  // 상태 등록
    {
        if (!_states.ContainsKey(stateName))
        {
            _states.Add(stateName, state);
        }
    }

    public BaseState<TController> GetState(StateName stateName)  // 상태 꺼내오기
    {
        if (_states.TryGetValue(stateName, out BaseState<TController> state))
            return state;
        return null;
    }

    public void DeleteState(StateName removeStateName)  // 상태 삭제
    {
        if (_states.ContainsKey(removeStateName))
        {
            _states.Remove(removeStateName);
        }
    }

    public void ChangeState(StateName nextStateName)    // 상태 전환
    {
        CurrentState?.OnExitState();   //현재 상태를 종료하는 메소드를 실행하고,
        if (_states.TryGetValue(nextStateName, out BaseState<TController> newState)) // 상태 전환
        {
            CurrentState = newState;
        }
        CurrentState?.OnEnterState();  // 다음 상태 진입 메소드 실행
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

