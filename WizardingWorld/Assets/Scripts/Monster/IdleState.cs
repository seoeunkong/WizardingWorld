using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState<MonsterController>
{
    public IdleState(MonsterController controller) : base(controller) { }


    public override void OnEnterState()
    {
        
    }

    public override void OnFixedUpdateState()
    {
        _Controller.monsterInfo.animator.SetFloat("Move", _Controller.monsterInfo.CurrentSpeed);
        _Controller.Patrol();
    }

    public override void OnExitState()
    {
        _Controller.monsterInfo.animator.SetFloat("Move", 0);
        _Controller.monsterInfo.rigid.velocity = Vector3.zero;
        _Controller.ResetPatrolPoint();
    }

    public override void OnUpdateState()
    {
        if(_Controller.CheckPlayer())
        {
            _Controller.monsterInfo.stateMachine.ChangeState(StateName.MRUN);
        }
    }
}
