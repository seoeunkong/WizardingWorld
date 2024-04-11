using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowState : BaseState
{
    public ThrowState(PlayerController controller) : base(controller) { }

    public override void OnEnterState()
    {
        Player.Instance.animator.SetTrigger("onThrow");
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
