using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneTemplate;
using UnityEngine;

public class MChaseState : BaseState<MonsterController>
{
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
        Transform targetPos = _Controller.CheckTarget(true, MonsterController.attackRadius);
        if (targetPos != null) _Controller.monsterInfo.stateMachine.ChangeState(StateName.MATTACK);
    }

    Transform whoToChase()
    {
        Transform target = null;
        if(!friendlyMode) //일반 몬스터 
        {
            target = _Controller.CheckTarget(true, MonsterController.chaseRadius);
            if (target == null)  _Controller.monsterInfo.stateMachine.ChangeState(StateName.IDLE);
        }
        else //팰 
        {
            if(_Controller.attackTargetMonster == null) target = Player.Instance.transform; 
            else
            {
                target = _Controller.attackTargetMonster.transform;
            }
        }

        return target;
    }

    void Chase()
    {
        Transform target = whoToChase();

        if (friendlyMode)
        {
            if (_Controller.attackTargetMonster && _Controller.checkPlayerPalDist != 0) //Enemy가 있고, 팰이 플레이어에게 돌아오는 상황이 아닌 경우 
            {
                if (StopFollowEnemy())
                {
                    _Controller.attackTargetMonster = null;
                    target = Player.Instance.transform;
                }
                if(StartToAttackEnemy(target))
                {
                    _Controller.monsterInfo.stateMachine.ChangeState(StateName.MATTACK);
                    return;
                }
            }
            else
            {
                if (TooClose())
                {
                    _Controller.monsterInfo.animator.SetFloat("Move", 0f);
                    _Controller.monsterInfo.rigid.velocity = Vector3.zero;

                    return;
                }
            }
        }

        _Controller.monsterInfo.rigid.velocity = _Controller.CalcRunDir(target, true) * _Controller.monsterInfo.CurrentSpeed;
        _Controller.monsterInfo.animator.SetFloat("Move", Mathf.Clamp(_Controller.monsterInfo.CurrentSpeed * 0.2f, 1, 2.5f));
    }

    bool StopFollowEnemy()
    {
        float distPlayerEnemy = Vector3.Distance(Player.Instance.transform.position, _Controller.attackTargetMonster.transform.position);
        return (distPlayerEnemy > MonsterController.chasePlayerDist);
    }

    bool StartToAttackEnemy(Transform target)
    {
        float distPalEnemy = Vector3.Distance(target.position, _Controller.transform.position);
        return distPalEnemy <= MonsterController.changeToAttackDist;
    }

    bool TooClose()
    {
        float distPalPlayer = Vector3.Distance(Player.Instance.transform.position, _Controller.transform.position);
        return distPalPlayer <= _Controller.checkPlayerPalDist;
    }

}
