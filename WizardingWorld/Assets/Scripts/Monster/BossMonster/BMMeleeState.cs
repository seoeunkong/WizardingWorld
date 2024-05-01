using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BMMeleeState : BaseState<BossMonsterController>
{ 
    public BMMeleeState(BossMonsterController controller) : base(controller) { }


    public override void OnEnterState()
    {
        _Controller.bossMonster.animator.SetTrigger("isMeleeAttack");
        Transform target = _Controller.CheckTarget();
        if (target != null)
        {
            _Controller.Attacking(target.GetComponent<CharacterController>());
        }

        _Controller.bossMonster.stateMachine.ChangeState(StateName.BMIDLE);
    }

    public override void OnExitState()
    {
        _Controller.SetOffEffect();
        _Controller.StartCoolTime(AttackName.CHASING);
    }

    public override void OnFixedUpdateState()
    {
      
    }

    public override void OnUpdateState()
    {
       
    }

   


}
