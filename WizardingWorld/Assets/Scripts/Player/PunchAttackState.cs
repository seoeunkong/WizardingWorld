using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchAttackState : BaseState
{
    public static bool IsAttack = false;
    public PunchAttackState(PlayerController controller) : base(controller)
    {
    }

    public override void OnEnterState()
    {
        //IsAttack = true;
        Player.Instance.animator.SetTrigger("onPunch");
    }

    public override void OnExitState()
    {
        
    }

    public override void OnFixedUpdateState()
    {
        
    }

    public override void OnUpdateState()
    {

    }

    
}
