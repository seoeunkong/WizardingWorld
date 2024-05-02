using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BMMeleeState : BaseState<BossMonsterController>
{
    Transform target = null;
    float timer = 0f;
    bool isAttack = false;

    public BMMeleeState(BossMonsterController controller) : base(controller) { }

    public override void OnEnterState()
    {
        _Controller.bossMonster.animator.SetTrigger("isMeleeAttack");
        target = _Controller.CheckTarget();
        if (target != null)
        {
           isAttack = true;
        }
    }

    public override void OnExitState()
    {
        isAttack = true;
        target = null;
        timer = 0f;

        _Controller.SetOffEffect();
        _Controller.StartCoolTime(AttackName.CHASING);
    }

    public override void OnFixedUpdateState()
    {
      
    }

    public override void OnUpdateState()
    {
       if(isAttack && _Controller.effectObject.activeSelf)
        {
            if(timer > 0.2f)
            {
                isAttack = false;
                _Controller.Attacking(target.GetComponent<CharacterController>());
                _Controller.bossMonster.stateMachine.ChangeState(StateName.BMIDLE);
            }
            timer += Time.deltaTime;
        }
    }

   


}
