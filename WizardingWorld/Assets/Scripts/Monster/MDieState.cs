using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MDieState : BaseState<MonsterController>
{
    public MDieState(MonsterController controller) : base(controller) { }


    public override void OnEnterState()
    {
        _Controller.animator.SetBool("isDead", true);
    }

    public override void OnExitState()
    {
        throw new System.NotImplementedException();
    }

    public override void OnFixedUpdateState()
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdateState()
    {
        throw new System.NotImplementedException();
    }

}
