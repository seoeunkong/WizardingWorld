using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BMIdleState : BaseState<BossMonsterController>
{
    public BMIdleState(BossMonsterController controller) : base(controller) { }


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
        _Controller.StartCoroutine(_Controller.ThinkAction());

    }

}
