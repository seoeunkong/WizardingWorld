using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MRunState : BaseState<MonsterController>
{
    Transform playerPos = null;

    public MRunState(MonsterController controller) : base(controller)
    {
    }

    public override void OnEnterState()
    {
        _Controller.CalcRunDir(Player.Instance.transform, true);
        _Controller.monsterInfo.animator.SetTrigger("onSense");
        _Controller.monsterInfo.CurrentSpeed = _Controller.monsterInfo.dashSpeed;
    }

    public override void OnExitState()
    {
        _Controller.SenseFalse();
        _Controller.monsterInfo.CurrentSpeed = _Controller.monsterInfo.moveSpeed;
        _Controller.monsterInfo.animator.SetFloat("Move", 0);
        _Controller.monsterInfo.rigid.velocity = Vector3.zero;
    }

    public override void OnFixedUpdateState()
    {
        playerPos = _Controller.CheckPlayer(MonsterController.runRadius);
        if(playerPos == null ) //플레이어를 감지 영역 내에 못 찾은 경우 
        {
            _Controller.monsterInfo.stateMachine.ChangeState(StateName.IDLE);
        }

        if (_Controller.Sense) //플레이어와 반대 방향으로 도망 
        {
            _Controller.monsterInfo.rigid.velocity = _Controller.CalcRunDir(playerPos, false) * _Controller.monsterInfo.CurrentSpeed;
            _Controller.monsterInfo.animator.SetFloat("Move", Mathf.Clamp(_Controller.monsterInfo.CurrentSpeed * 0.2f, 1, 2.5f));
        }
    }

    public override void OnUpdateState()
    {

    }

}
