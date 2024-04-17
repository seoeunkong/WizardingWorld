using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MRunState : BaseState<MonsterController>
{
    Transform playerPos;

    public MRunState(MonsterController controller) : base(controller)
    {
    }

    public override void OnEnterState()
    {
        _Controller.CurrentSpeed = _Controller.DashSpeed;
    }

    public override void OnExitState()
    {
        _Controller.CurrentSpeed = _Controller.MoveSpeed;
        _Controller.animator.SetFloat("Move", 0);
        _Controller.rigid.velocity = Vector3.zero;
    }

    public override void OnFixedUpdateState()
    {
        playerPos = _Controller.CheckPlayer(2);
        if(playerPos == null )
        {
            _Controller.stateMachine.ChangeState(StateName.IDLE);
        }
        _Controller.rigid.velocity = _Controller.CalcRunDir(playerPos) * _Controller.CurrentSpeed;
        _Controller.animator.SetFloat("Move", Mathf.Clamp(_Controller.CurrentSpeed * 0.2f, 1, 2.5f));
    }

    public override void OnUpdateState()
    {

    }

}
