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
        _Controller.animator.SetTrigger("onHit");

        if (_Controller.monsterInfo.CurrentHP > 0)
        {
            float hp = _Controller.monsterInfo.CurrentHP - Player.Instance.CurrentAttackPower > 0 ? _Controller.monsterInfo.CurrentHP - Player.Instance.CurrentAttackPower : 0;
            _Controller.monsterInfo.SetHP(hp);

            if (hp == 0)
            {
                _Controller.stateMachine.ChangeState(StateName.MDEAD);
            }
        }
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
