using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAttackState : BaseState<MonsterController> 
{
    bool friendlyMode = false;

    public MAttackState(MonsterController controller) : base(controller) { }

    public override void OnEnterState()
    {
        friendlyMode = _Controller.monsterInfo.FriendlyMode;

        Transform target = whoToAttack();
        _Controller.transform.LookAt(target);
        _Controller.Attacking(target.gameObject.GetComponent<CharacterController>());

    }

    public override void OnExitState()
    {
        _Controller.IsAttack = false;
        _Controller.monsterInfo.animator.SetBool("isAttack", false);
    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnUpdateState()
    {
        if (!_Controller.IsAttack) _Controller.monsterInfo.stateMachine.ChangeState(StateName.MCHASE);
    }

    Transform whoToAttack()
    {
        Transform target = null;
        if (!friendlyMode) //일반 몬스터 
        {
            if(Player.Instance.currentPal != null && Player.Instance.currentPal.gameObject.activeSelf) 
            {
                target = _Controller.CheckTarget(false, MonsterController.attackRadius * 15); //팰로 공격 대상 설정 
                if (target == null) target = _Controller.CheckTarget(true, MonsterController.attackRadius); //팰이 없으면 플레이어로 공격 대상 설정
            }
            else target = _Controller.CheckTarget(true, MonsterController.attackRadius);
        }
        else //팰 
        {
            if (_Controller.attackTargetMonster != null) target = _Controller.attackTargetMonster.transform;
        }

        if (target == null) _Controller.monsterInfo.stateMachine.ChangeState(StateName.MCHASE);

        return target;
    }

}
