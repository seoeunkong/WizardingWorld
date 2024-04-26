using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MDeadState : BaseState<MonsterController>
{
    public MDeadState(MonsterController controller) : base(controller) { }



    public override void OnEnterState()
    {
        _Controller.monsterInfo.animator.SetBool("isDead", true);
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
