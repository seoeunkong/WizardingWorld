using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MDeadState : BaseState<MonsterController>
{
    public MDeadState(MonsterController controller) : base(controller) { }



    public override void OnEnterState()
    {
        //if(Player.Instance.currentPal == _Controller.transform)
        //{
        //    Player.Instance.currentPal = null;

        //}
        _Controller.monsterInfo.animator.SetTrigger("onDead");
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
