using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UIElements;

public class BMLaserState : BaseState<BossMonsterController>
{
    private bool shootLine = false;
    private bool shootLaser = false;
    private float timer = 0f;

    private Vector3 endPoint = Vector3.zero;

    public BMLaserState(BossMonsterController controller) : base(controller) { }


    public override void OnEnterState()
    {
        _Controller.bossMonster.animator.SetTrigger("islineOn");

        shootLine = true;
        timer = _Controller.lineTimer;

        _Controller.laserObject.transform.parent = _Controller.transform;
        _Controller.laserObject.transform.position = _Controller.missileStartPos.position;

        _Controller.laserLine.enabled = true;
    }

    public override void OnExitState()
    {
        shootLine = false;
        shootLaser = false;
        timer = 0f;
        endPoint = Vector3.zero;
        _Controller.laserLine.enabled = false;
        _Controller.laserObject.SetActive(false);

        _Controller.StartCoolTime(AttackName.LASER);
    }

    public override void OnUpdateState()
    {
        if (shootLine)
        {
            ShootLine();
        }
    }

    public override void OnFixedUpdateState()
    {
        if (shootLaser)
        {
            ShootLaser(endPoint);

            if (!_Controller.laserObject.activeSelf)
            {
                _Controller.bossMonster.stateMachine.ChangeState(StateName.BMIDLE);
            }
        }
    }

    void ShootLine()
    {
        Vector3 rayOrigin = _Controller.missileStartPos.position;
        _Controller.laserLine.SetPosition(0, _Controller.missileStartPos.position + Vector3.forward);

        if (timer > 0)
        {
            // 플레이어의 현재 위치와 이동 방향을 사용하여 예측 위치 계산
            Vector3 targetPosition = _Controller.attackTarget.position + Vector3.up;

            Monster monster = _Controller.attackTarget.GetComponent<Monster>();
            Vector3 targetVelocity = Vector3.zero;
            if (monster != null) targetVelocity = monster.rigid.velocity;
            else targetVelocity = Player.Instance.GetComponent<Rigidbody>().velocity;
            
            Vector3 predictedPosition = targetPosition + targetVelocity * Time.deltaTime;  // 다음 프레임에 대한 위치 예측

            Vector3 dir = (predictedPosition - rayOrigin).normalized;
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, dir, out hit, _Controller.laserRange))
            {
                if (hit.collider == _Controller.attackTarget)
                {
                    endPoint = targetPosition;
                }
                else
                {
                    endPoint = hit.point;
                }
            }
            else
            {
                endPoint = rayOrigin + _Controller.laserRange * dir;
            }
            _Controller.laserLine.SetPosition(1, endPoint);
            timer -= Time.deltaTime;
        }
        else
        {
            _Controller.laserLine.enabled = false;
            shootLine = false;
            shootLaser = true;

            _Controller.laserObject.SetActive(true);
            _Controller.bossMonster.animator.SetTrigger("isLaserAttack");
        }
    }

    void ShootLaser(Vector3 targetPoint)
    {
        if (targetPoint == Vector3.zero) return;
        Vector3 dir = (targetPoint - _Controller.missileStartPos.position).normalized;
        _Controller.laserObject.transform.Translate(_Controller.laserSpeed * Time.deltaTime * dir, Space.World);
    }

}
