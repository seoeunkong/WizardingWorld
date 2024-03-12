using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(PlayerController controller) : base(controller) { }

    public override void OnEnterState()
    {
        Player.Instance.weaponManager.Weapon?.Attack(this);
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
