using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAttackState : BaseState<MonsterController> 
{
    Transform playerPos;
    public MAttackState(MonsterController controller) : base(controller) { }

    public override void OnEnterState()
    {
        
    }

    public override void OnExitState()
    {

    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnUpdateState()
    {
        CheckToAttack();
    }

    void CheckToAttack()
    {
        playerPos = _Controller.CheckPlayer(MonsterController.attackRadius);
        if (playerPos == null) _Controller.stateMachine.ChangeState(StateName.MCHASE);
    }
}
