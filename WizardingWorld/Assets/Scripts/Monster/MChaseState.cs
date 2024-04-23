using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MChaseState : BaseState<MonsterController>
{
    Transform playerPos;
    public MChaseState(MonsterController controller) : base(controller) { }

    public override void OnEnterState()
    {
        _Controller.CurrentSpeed = _Controller.dashSpeed;
    }

    public override void OnExitState()
    {
        _Controller.CurrentSpeed = _Controller.moveSpeed;
        _Controller.animator.SetFloat("Move", 0f);
        _Controller.rigid.velocity = Vector3.zero;
    }

    public override void OnFixedUpdateState()
    {
       

       
    }

    public override void OnUpdateState()
    {
        //CheckToAttack();
        //Chase();

        Chase();
        CheckToAttack();
    }

    void CheckToAttack()
    {
        playerPos = _Controller.CheckPlayer(MonsterController.attackRadius);
        if (playerPos != null) _Controller.stateMachine.ChangeState(StateName.MATTACK);
    }

    void Chase()
    {
        playerPos = _Controller.CheckPlayer(MonsterController.chaseRadius);
        if (playerPos == null) _Controller.stateMachine.ChangeState(StateName.IDLE);
        _Controller.rigid.velocity = _Controller.CalcRunDir(playerPos, true) * _Controller.CurrentSpeed;
        _Controller.animator.SetFloat("Move", Mathf.Clamp(_Controller.CurrentSpeed * 0.2f, 1, 2.5f));
    }
}
