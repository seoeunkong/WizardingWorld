using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BMHitState : BaseState<BossMonsterController>
{
    public BMHitState(BossMonsterController controller) : base(controller) { }


    public override void OnEnterState()
    {
        _Controller.bossMonster.animator.SetTrigger("onHit");
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
