using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveState : BaseState
{
    enum MoveName
    {
        MOVE = 1,
        LEFT,
        RIGHT
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
        int aniNum = GetAni(Controller.inputDirection.z != 0, Controller.inputDirection.x < 0, Controller.inputDirection.x > 0);
        Player.Instance._animator.SetInteger("Move", aniNum);

    }

    public override void OnExitState()
    {
        Player.Instance._rigid.velocity = Vector3.zero;
        Player.Instance._animator.SetInteger("Move", 0);


    }

    private int GetAni(bool isRun, bool isLeft, bool isRight)
    {
        if (isRun) return (int)MoveName.MOVE;
        if (isLeft) return (int)MoveName.LEFT;
        if (isRight) return (int) MoveName.RIGHT;
       
        return 0;  
    }

}
