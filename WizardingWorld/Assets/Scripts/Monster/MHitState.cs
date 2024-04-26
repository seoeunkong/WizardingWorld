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
        _Controller.Hit(Player.Instance.AttackPower);
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
