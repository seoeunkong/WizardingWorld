using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState<PlayerController>
{
    public AttackState(PlayerController controller) : base(controller) { }

    public override void OnEnterState()
    {
        PlayerController.IsAttack = true;
        Player.Instance.weaponManager.Weapon?.Attack(this);
    }

    public override void OnExitState()
    {
        PlayerController.IsAttack = false;
        Player.Instance.rigid.velocity = Vector3.zero;
        Player.Instance.animator.SetInteger("Move", 0);
    }

    public override void OnFixedUpdateState()
    {
        Player.Instance.rigid.velocity = _Controller.calculatedDirection + _Controller.gravity;
        Player.Instance.animator.SetInteger("Move", Player.Instance.rigid.velocity != Vector3.zero ? (int)MoveName.MOVE : 0);
    }

    public override void OnUpdateState()
    {
        _Controller.Attacking(Player.Instance.weaponManager.Weapon.AttackDamage);
    }

   
}
