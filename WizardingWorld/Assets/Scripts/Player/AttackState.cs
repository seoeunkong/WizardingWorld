using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState<PlayerController>
{
    public AttackState(PlayerController controller) : base(controller) { }

    public override void OnEnterState()
    {
        PlayerController.IsAttack = true;
        Player.Instance.SetAttackPower(Player.Instance.weaponManager.Weapon.AttackDamage);

        if (_Controller.canAttackCombo) Player.Instance.animator.CrossFade("MeleeAttack2", 0.2f);
        else Player.Instance.weaponManager.Weapon?.Attack(this);
    }

    public override void OnExitState()
    {
        PlayerController.IsAttack = false;
        PlayerController.startAttackAni = false;
        Player.Instance.rigid.velocity = Vector3.zero;
        Player.Instance.animator.CrossFade("Empty", 0.3f);
        Player.Instance.SetAttackPower(0);

        _Controller.CountAttackCombo();
    }

    public override void OnFixedUpdateState()
    {
        Player.Instance.rigid.velocity = _Controller.calculatedDirection + _Controller.gravity;
        Player.Instance.animator.SetInteger("Move", Player.Instance.rigid.velocity != Vector3.zero ? (int)MoveName.MOVE : 0);
    }

    public override void OnUpdateState()
    {
        if (!PlayerController.IsAttack) return;
        if (!PlayerController.startAttackAni) _Controller.CheckEnemy(true);
        if (!_Controller.IsLookFoward()) Player.Instance.stateMachine.ChangeState(StateName.MOVE);
    }

   
}
