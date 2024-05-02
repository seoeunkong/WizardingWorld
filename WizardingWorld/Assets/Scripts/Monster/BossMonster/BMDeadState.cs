using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BMDeadState : BaseState<BossMonsterController>
{
    public BMDeadState(BossMonsterController controller) : base(controller) { }


    public override void OnEnterState()
    {
        _Controller.bossMonster.animator.SetTrigger("onDead");
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
