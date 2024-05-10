using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MHitState : BaseState<MonsterController>
{
    public MHitState(MonsterController controller) : base(controller)
    {
    }

    public override void OnEnterState()
    {
        _Controller.monsterInfo.animator.SetTrigger("onHit");
    }

    public override void OnExitState()
    {
        _Controller.monsterInfo.animator.ResetTrigger("onHit");
    }

    public override void OnFixedUpdateState()
    {
        
    }

    public override void OnUpdateState()
    {
        
    }
}
