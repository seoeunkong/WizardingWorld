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
        _Controller.CalcRunDir(Player.Instance.transform, true);
        _Controller.animator.SetTrigger("onSense");
        _Controller.CurrentSpeed = _Controller.DashSpeed;
    }

    public override void OnExitState()
    {
        _Controller.SenseFalse();
        _Controller.CurrentSpeed = _Controller.MoveSpeed;
        _Controller.animator.SetFloat("Move", 0);
        _Controller.rigid.velocity = Vector3.zero;
    }

    public override void OnFixedUpdateState()
    {
        playerPos = _Controller.CheckPlayer(MonsterController.runRadius);
        if(playerPos == null ) //플레이어를 감지 영역 내에 못 찾은 경우 
        {
            _Controller.stateMachine.ChangeState(StateName.IDLE);
        }

        if (_Controller.Sense) //플레이어와 반대 방향으로 도망 
        {
            _Controller.rigid.velocity = _Controller.CalcRunDir(playerPos, false) * _Controller.CurrentSpeed;
            _Controller.animator.SetFloat("Move", Mathf.Clamp(_Controller.CurrentSpeed * 0.2f, 1, 2.5f));
        }
    }

    public override void OnUpdateState()
    {

    }

}
