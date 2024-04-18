using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchAttackState : BaseState<PlayerController>
{
    public PunchAttackState(PlayerController controller) : base(controller)
    {
    }

    public override void OnEnterState()
    {
        PlayerController.IsAttack = true;
        Player.Instance.animator.SetTrigger("onPunch");
    }

    public override void OnExitState()
    {
        PlayerController.IsAttack = false;
        Player.Instance.rigid.velocity = Vector3.zero;
        Player.Instance.animator.SetInteger("Move", 0);
    }

    public override void OnFixedUpdateState()
    {
        Player.Instance.rigid.velocity = _Controller.calculatedDirection + _Controller.gravity;
        Player.Instance.animator.SetInteger("Move", Player.Instance.rigid.velocity != Vector3.zero ? (int)MoveName.MOVE : 0);
        
    }

    public override void OnUpdateState()
    {
        _Controller.Attacking(Player.Instance.AttackPower);
    }


}
