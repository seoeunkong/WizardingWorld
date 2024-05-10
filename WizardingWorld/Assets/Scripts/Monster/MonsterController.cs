using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class MonsterController : CharacterController, IStateChangeable
{
    public Monster monsterInfo { get; private set; }


    #region #반지름 및 거리
    public const float runRadius = 2;
    public const float chaseRadius = 1.5f;
    public const float changeToAttackDist = 5f;
    public const float chasePlayerDist = 15f;
    public const float attackRadius = 0.2f;
    public const float distanceWithPlayer = 30f;
    private float _fieldOfView = 90.0f; // 시야각 설정
    public const float playerPalDist = 5f;
    public float checkPlayerPalDist = playerPalDist;
    #endregion

    [Header("몬스터 순찰 속성")]
    [SerializeField] private float _patrolRadius;
    private Vector3 _patrolPoint;
    public void ResetPatrolPoint() => _patrolPoint = Vector3.zero;

    [Header("플레이어 감지 속성")]
    [SerializeField] private float _checkDistance;

    #region #몬스터 애니메이션
    public bool Sense { get; private set; }
    public void SenseTrue() => Sense = true;
    public void SenseFalse() => Sense = false;
    public void ChangeStateToChase() => monsterInfo.stateMachine.ChangeState(StateName.MCHASE);
    public void MonsterDead() => transform.gameObject.SetActive(false);
    #endregion

    #region #공격
    [Header("플레이어 공격 속성")]
    public bool IsAttack = false;
    public Transform attackTargetMonster { get { return _attackTargetMonster; } set { _attackTargetMonster = value; } }
    private Transform _attackTargetMonster;
    #endregion

    [Header("이팩트")]
    private TrailController _trailController;
    private bool _backToPlayer = false;
    public bool BackToPlayer { get { return _backToPlayer; } set { _backToPlayer = value; } }

    #region #Die
    public delegate void PalDie();
    public static event PalDie OnPalDie;
    #endregion

    void Awake()
    {
        monsterInfo = GetComponent<Monster>();
    }

    private void Start()
    {
        _trailController = GetComponent<TrailController>();
        _groundLayer = 1 << LayerMask.NameToLayer("Ground");
    }

    public void ChangeState(StateName state)
    {
        monsterInfo.stateMachine.ChangeState(state);
    }

    protected void ControlGravity()
    {
        gravity = Vector3.down * MathF.Abs(monsterInfo.rigid.velocity.y);
        if (_isGrounded && _isOnSlope)
        {
            gravity = Vector3.zero;
            monsterInfo.rigid.useGravity = false;
            return;
        }
        monsterInfo.rigid.useGravity = true;
    }


    public void Patrol()
    {
        Vector3 monster = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
        Vector3 patrol = Vector3.Scale(_patrolPoint, new Vector3(1, 0, 1));

        //PatrolPoint 초기화 
        if (_patrolPoint == Vector3.zero || Vector3.Distance(monster, patrol) <= 1.5f)
        {
            Vector3 point = UnityEngine.Random.insideUnitSphere * _patrolRadius + transform.position;
            _patrolPoint = new Vector3(point.x, 0, point.z);
        }
        _patrolPoint = new Vector3(_patrolPoint.x, transform.position.y, _patrolPoint.z);

        //PatrolPoint을 기준으로 몬스터 이동 및 회전
        Vector3 dir = (_patrolPoint - transform.position).normalized;
        monsterInfo.rigid.velocity = GetDirection(dir) * monsterInfo.CurrentSpeed;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.Scale(dir, new Vector3(1, 0, 1))), Time.deltaTime * monsterInfo.rotateSpeed);
    }

    public Transform CheckTarget(bool isPlayer, float size = 1) //감지 영역 내에 플레이어 존재 여부 체크 
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, _checkDistance * size);
        foreach (Collider coll in colls)
        {
            if (isPlayer && coll.gameObject.CompareTag("Player"))
            {
                if (monsterInfo.stateMachine.GetState(StateName.IDLE) == monsterInfo.stateMachine.CurrentState && IsPlayerInSight(coll.transform.position)) return coll.transform;
                if (monsterInfo.stateMachine.GetState(StateName.IDLE) != monsterInfo.stateMachine.CurrentState) return coll.transform;
            }

            if (!isPlayer && coll.transform.gameObject == Player.Instance.currentPal.gameObject)
            {
                return coll.transform;
            }
        }
        return null;
    }

    bool IsPlayerInSight(Vector3 playerPos)    // 계산된 각도를 통해 플레이어가 시야각 범위 내에 있는지 확인
    {
        Vector3 enemyToPlayer = playerPos - transform.position;
        enemyToPlayer.Normalize();

        float angle = Vector3.Angle(transform.forward, enemyToPlayer);

        if (angle < _fieldOfView * 0.5f) return true;
        else return false;
    }

    //ChasePlayer이 true라면, 플레이어를 향해 쫓는다.
    //ChasePlayer이 false라면, 플레이어의 반대 방향으로 도망.
    public Vector3 CalcRunDir(Transform player, bool ChasePlayer)
    {
        int chase = ChasePlayer ? 1 : -1;

        if (player == null) return Vector3.zero; //플레이어를 감지 영역에서 발견하지 못한 경우 

        Vector3 playerPos = player.position;

        Vector3 dir = Vector3.Scale((playerPos - transform.position), new Vector3(chase, 0, chase)).normalized;
        transform.rotation = Quaternion.LookRotation(dir);

        return GetDirection(dir);
    }


    Vector3 GetDirection(Vector3 dir)
    {
        _isOnSlope = IsOnSlope();
        _isGrounded = IsGrounded();

        Vector3 calculatedDirection = (_isOnSlope && _isGrounded) ? AdjustDirectionToSlope(dir) : dir;
        return calculatedDirection;
    }


    public override void Attacking(CharacterController targetController)
    {
        if (targetController == null) return;
        if (!monsterInfo.IsTargetAttackable(targetController)) return;

        if (!IsAttack)
        {
            IsAttack = true;
            StartCoroutine(AttackCoolDown(targetController));
        }
    }

    IEnumerator AttackingCor<T>(T target) where T : CharacterController
    {
        monsterInfo.animator.SetBool("isAttack", true);

        if (target is PlayerController ps) ps.Hit(monsterInfo.attackPower);
        if (target is IStateChangeable stateChangeable)
        {
            target.Hit(monsterInfo.attackPower);
            stateChangeable.ChangeState(GetHitState(target));
        }

        yield return new WaitForSeconds(1.5f);
        monsterInfo.animator.SetBool("isAttack", false);
        IsAttack = false;
    }

    IEnumerator AttackCoolDown(CharacterController targetController)
    {
        StartCoroutine(AttackingCor(targetController));

        float coolTime = UnityEngine.Random.Range(2, 10);

        while (coolTime > 0)
        {
            coolTime -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sphere"))
        {
            PalSphere sphere = collision.gameObject.GetComponent<PalSphere>();
            if (sphere.isToCaptureMonster()) HitBySphere();
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (checkPlayerPalDist == 0) gameObject.SetActive(false);
        }
    }

    void HitBySphere()
    {
        monsterInfo.FriendlyMode = true;
        monsterInfo.SetHP(monsterInfo.maxHP);
        Inventory.Instance.Add(monsterInfo);

        transform.gameObject.SetActive(false);
    }

    public void backToPlayer() //팰 활성화 -> 비활성화 모드 
    {
        _trailController.bodyActiveFalse();
        _trailController.trailActiveTrue();
        checkPlayerPalDist = 0f;
        monsterInfo.stateMachine.ChangeState(StateName.MCHASE);

        Player.Instance.currentPal = null;
    }


    private void OnEnable()
    {
        if (monsterInfo.FriendlyMode)
        {
            _trailController.trailActiveFalse();
            _trailController.bodyActiveTrue();
            checkPlayerPalDist = playerPalDist;
            monsterInfo.stateMachine.ChangeState(StateName.MCHASE);
        }
    }
    public override void Hit(float damage)
    {
        if (monsterInfo.stateMachine.CurrentState == monsterInfo.stateMachine.GetState(StateName.MDEAD)) return;

        if (monsterInfo.CurrentHP > 0)
        {
            float hp = monsterInfo.CurrentHP - damage > 0 ? monsterInfo.CurrentHP - damage : 0;
            monsterInfo.SetHP(hp);

            if (hp <= 0)
            {
                if (monsterInfo.FriendlyMode) OnPalDie.Invoke();
                monsterInfo.stateMachine.ChangeState(StateName.MDEAD);
            }
        }
    }
}
