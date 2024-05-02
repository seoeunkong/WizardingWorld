using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BMChaseState : BaseState<BossMonsterController>
{
    bool startFollow = true;
    float timer = 0f;

    bool isAvailableFollow() => startFollow && timer > 0f && !stopFollowingTarget(Player.Instance.transform.position);

    bool stopFollowingTarget(Vector3 target) => Vector3.Distance(target, _Controller.transform.position) < BossMonsterController.stopChasingDist;

    public BMChaseState(BossMonsterController controller) : base(controller) { }


    public override void OnEnterState()
    {
        startFollow = true;
        timer = BossMonsterController.chasingTime;

        Debug.Log("Chase " + _Controller.attackTarget);
    }

    public override void OnExitState()
    {
        if(!startFollow) _Controller.StartCoolTime(AttackName.CHASING);
        _Controller.bossMonster.animator.SetFloat("Move", 0f);
        _Controller.bossMonster.rigid.velocity = Vector3.zero;
    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnUpdateState()
    {
        if (startFollow)
        {
            followTarget();
        }
    }

    void followTarget()
    {
        if (timer > 0f)
        {
            if (stopFollowingTarget(_Controller.attackTarget.position))
            {
                _Controller.bossMonster.stateMachine.ChangeState(StateName.BMMELEE);
                return;
            }
            _Controller.bossMonster.animator.SetFloat("Move", Mathf.Clamp(_Controller.bossMonster.CurrentSpeed * 0.2f, 1, 2.5f));
            _Controller.bossMonster.rigid.velocity = _Controller.CalcRunDir(_Controller.attackTarget) * _Controller.bossMonster.CurrentSpeed;

            timer -= Time.deltaTime;
        }
        else
        {
            startFollow = false;
            _Controller.bossMonster.stateMachine.ChangeState(StateName.BMIDLE);
        }
    }


}
