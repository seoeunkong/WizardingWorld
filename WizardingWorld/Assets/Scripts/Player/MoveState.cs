using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum MoveName
{
    MOVE = 10,
    DASH = 11
}

public class MoveState : BaseState<PlayerController>
{
    public MoveState(PlayerController controller) : base(controller)
    {
        
    }

    public override void OnEnterState()
    {

    }

    public override void OnUpdateState()
    {
        
    }

    public override void OnFixedUpdateState()
    {
        Player.Instance.rigid.velocity = _Controller.calculatedDirection + _Controller.gravity;
        Player.Instance.animator.SetInteger("Move", AnimatorNum(Player.Instance.CurrentSpeed));

    }

    public override void OnExitState()
    {
        Player.Instance.rigid.velocity = Vector3.zero;
        //Player.Instance.animator.SetInteger("Move", 0);
    }

    public int AnimatorNum(float speed)
    {
        if(_Controller.calculatedDirection == Vector3.zero) return 0;
        if(speed <= Player.Instance.MoveSpeed) return (int)MoveName.MOVE;
        if (speed <= Player.Instance.DashSpeed) return (int)MoveName.DASH;
        
        return 0;
    }



}
