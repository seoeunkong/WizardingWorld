using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BMMissileState : BaseState<BossMonsterController>
{
    Vector3[] missileDir;
    public BMMissileState(BossMonsterController controller) : base(controller) { }


    public override void OnEnterState()
    {
        missileDir = _Controller.GetMissileDirs(_Controller.missileCount);
        _Controller.resetMissile();

        _Controller.bossMonster.animator.SetTrigger("isMissileOn");
    }

    public override void OnExitState()
    {
        
    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnUpdateState()
    {
        _Controller.MissileAttack(missileDir);
        if (_Controller.allMissileOff()) _Controller.bossMonster.stateMachine.ChangeState(StateName.BMIDLE);
    }
}
