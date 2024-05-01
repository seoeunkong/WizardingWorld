using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BMMissileState : BaseState<BossMonsterController>
{
    Vector3[] missileDir;
    public BMMissileState(BossMonsterController controller) : base(controller) { }


    public override void OnEnterState()
    {
        _Controller.bossMonster.animator.SetTrigger("isMissileOn");

        missileDir = _Controller.GetMissileDirs(_Controller.missileCount);
        _Controller.resetMissile();

        _Controller.missiles = missileDir;
    }

    public override void OnExitState()
    {
        _Controller.StartCoolTime(AttackName.MISSILE);
    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnUpdateState()
    {
        _Controller.MissileAttack(missileDir);
        if (_Controller.allMissileOff())
        {
            _Controller.bossMonster.stateMachine.ChangeState(StateName.BMIDLE);
        }
    }
}
