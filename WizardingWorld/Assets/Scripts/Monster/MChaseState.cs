using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MChaseState : BaseState<MonsterController>
{
    Transform playerPos;
    bool friendlyMode;
    public MChaseState(MonsterController controller) : base(controller) { }

    public override void OnEnterState()
    {
        _Controller.monsterInfo.CurrentSpeed = _Controller.monsterInfo.dashSpeed;
        friendlyMode = _Controller.monsterInfo.FriendlyMode;
    }

    public override void OnExitState()
    {
        _Controller.monsterInfo.CurrentSpeed = _Controller.monsterInfo.moveSpeed;
        _Controller.monsterInfo.animator.SetFloat("Move", 0f);
        _Controller.monsterInfo.rigid.velocity = Vector3.zero;
    }

    public override void OnFixedUpdateState()
    {
       

       
    }

    public override void OnUpdateState()
    {
        //CheckToAttack();
        //Chase();

        Chase();
        if(!friendlyMode) CheckToAttack();
    }

    void CheckToAttack()
    {
        playerPos = _Controller.CheckPlayer(MonsterController.attackRadius);
        if (playerPos != null) _Controller.monsterInfo.stateMachine.ChangeState(StateName.MATTACK);
    }

    void Chase()
    {
        playerPos = _Controller.CheckPlayer(MonsterController.chaseRadius * (friendlyMode? 5 : 1));
        if (playerPos == null) _Controller.monsterInfo.stateMachine.ChangeState(StateName.IDLE);

        if (friendlyMode)
        {
            float dist = Vector3.Distance(playerPos.position, _Controller.transform.position);
            if (dist <= MonsterController.chaseDist)
            {
                _Controller.monsterInfo.rigid.velocity = Vector3.zero;
                _Controller.monsterInfo.animator.SetFloat("Move", 0f);
                return;
            }
        }

        _Controller.monsterInfo.rigid.velocity = _Controller.CalcRunDir(playerPos, true) * _Controller.monsterInfo.CurrentSpeed;
        _Controller.monsterInfo.animator.SetFloat("Move", Mathf.Clamp(_Controller.monsterInfo.CurrentSpeed * 0.2f, 1, 2.5f));
    }
}
