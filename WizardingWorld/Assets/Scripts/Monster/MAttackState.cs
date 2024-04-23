using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAttackState : BaseState<MonsterController> 
{
    Transform playerPos;
    public MAttackState(MonsterController controller) : base(controller) { }

    public override void OnEnterState()
    {
        CheckToAttack();
        _Controller.Attacking(playerPos);
    }

    public override void OnExitState()
    {
        _Controller.IsAttack = false;
        _Controller.animator.SetBool("isAttack", false);
    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnUpdateState()
    {
        if (!_Controller.IsAttack) _Controller.stateMachine.ChangeState(StateName.MCHASE);
    }

    void CheckToAttack()
    {
        playerPos = _Controller.CheckPlayer(MonsterController.attackRadius);
        if (playerPos == null) _Controller.stateMachine.ChangeState(StateName.MCHASE);
    }
}
