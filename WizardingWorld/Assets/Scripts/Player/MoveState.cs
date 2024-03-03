using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveState : BaseState
{
    enum MoveName
    {
        MOVE = 10,
        DASH = 11
    }


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
        Player.Instance._rigid.velocity = Controller.calculatedDirection + Controller.gravity;
        Player.Instance._animator.SetInteger("Move", AnimatorNum(Player.Instance._CurrentSpeed));

    }

    public override void OnExitState()
    {
        Player.Instance._rigid.velocity = Vector3.zero;
        Player.Instance._animator.SetInteger("Move", 0);


    }

    int AnimatorNum(float speed)
    {
        if(Controller.calculatedDirection == Vector3.zero) return 0;
        if(speed <= Player.Instance._MoveSpeed) return (int)MoveName.MOVE;
        if (speed <= Player.Instance._DashSpeed) return (int)MoveName.DASH;
        
        return 0;
    }



}
