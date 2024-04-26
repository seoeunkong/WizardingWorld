using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class MonsterController : CharacterController
{
    public Monster monsterInfo { get; private set; }


    #region #���� ���� ����
    public const float runRadius = 2;
    public const float chaseRadius = 3;
    public const float chaseDist = 3f;
    public const float attackRadius = 0.1f;

    private float _fieldOfView = 90.0f; // �þ߰� ����
    #endregion

    [Header("���� ���� �Ӽ�")]
    [SerializeField] private float _patrolRadius;
    private Vector3 _patrolPoint;
    public void ResetPatrolPoint() => _patrolPoint = Vector3.zero;

    [Header("�÷��̾� ���� �Ӽ�")]
    [SerializeField] private float _checkDistance;

    public void ChangeStateToChase() => monsterInfo.stateMachine.ChangeState(StateName.MCHASE);

    #region #���� �ִϸ��̼�
    public bool Sense { get; private set; }
    public void SenseTrue() => Sense = true;
    public void SenseFalse() => Sense = false;
    #endregion

    public void MonsterDead() => transform.gameObject.SetActive(false);

    #region #����
    [Header("�÷��̾� ���� �Ӽ�")]
    [SerializeField] private float _attackCheckDistance;
    public bool IsAttack = false;
    #endregion

    void Awake()
    {
        monsterInfo = GetComponent<Monster>();
    }

    void Start()
    {
        _groundLayer = 1 << LayerMask.NameToLayer("Ground");
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

    public bool IsGrounded()
    {
        _isGrounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, GROUNDCHECK_DISTANCE, _groundLayer);
        return _isGrounded;
    }

    bool IsOnSlope() //��� ���� üũ 
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out _slopeHit, RAY_DISTANCE, _groundLayer))
        {
            var angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle != 0f && angle < _maxSlopeAngle;
        }
        return false;
    }

    Vector3 AdjustDirectionToSlope(Vector3 direction) //��� ������ �´� �̵� ���� 
    {
        return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
    }


    public void Patrol()
    {
        Vector3 monster = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
        Vector3 patrol = Vector3.Scale(_patrolPoint, new Vector3(1, 0, 1));

        //PatrolPoint �ʱ�ȭ 
        if (_patrolPoint == Vector3.zero || Vector3.Distance(monster,patrol) <= 1.5f)
        {
            Vector3 point = UnityEngine.Random.insideUnitSphere * _patrolRadius + transform.position;
            _patrolPoint = new Vector3(point.x, 0, point.z);
        }
        _patrolPoint = new Vector3(_patrolPoint.x, transform.position.y, _patrolPoint.z);

        //PatrolPoint�� �������� ���� �̵� �� ȸ��
        Vector3 dir = (_patrolPoint - transform.position).normalized;
        monsterInfo.rigid.velocity = GetDirection(dir) * monsterInfo.CurrentSpeed;
        
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.Scale(dir, new Vector3(1, 0, 1))), Time.deltaTime * monsterInfo.rotateSpeed);
    }

    public Transform CheckPlayer(float size = 1) //���� ���� ���� �÷��̾� ���� ���� üũ 
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, _checkDistance * size);
        foreach (Collider coll in colls)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                if (monsterInfo.stateMachine.GetState(StateName.IDLE) == monsterInfo.stateMachine.CurrentState && IsPlayerInSight(coll.transform.position)) return coll.transform;
                if(monsterInfo.stateMachine.GetState(StateName.IDLE) != monsterInfo.stateMachine.CurrentState) return coll.transform;
            }
        }
        return null;
    }

    bool IsPlayerInSight(Vector3 playerPos)    // ���� ������ ���� �÷��̾ �þ߰� ���� ���� �ִ��� Ȯ��
    {
        Vector3 enemyToPlayer = playerPos - transform.position;
        enemyToPlayer.Normalize();

        float angle = Vector3.Angle(transform.forward, enemyToPlayer);

        if (angle < _fieldOfView * 0.5f) return true;
        else return false;
    }


    IEnumerator OnDead()
    {
        yield return new WaitForSeconds(0.5f);
        monsterInfo.stateMachine.ChangeState(StateName.MDEAD);
    }


    //ChasePlayer�� true���, �÷��̾ ���� �Ѵ´�.
    //ChasePlayer�� false���, �÷��̾��� �ݴ� �������� ����.
    public Vector3 CalcRunDir(Transform player, bool ChasePlayer) 
    {
        int chase = ChasePlayer ? 1 : -1;

        if (player == null) return Vector3.zero; //�÷��̾ ���� �������� �߰����� ���� ��� 

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

    public void Attacking(Transform playerPos)
    {
        if (playerPos == null) return;
        PlayerController player = playerPos.GetComponent<PlayerController>();

        if (!IsAttack)
        {
            IsAttack = true;
            StartCoroutine(AttackingCor(player));
        }
    }

    IEnumerator AttackingCor(PlayerController player)
    {
        monsterInfo.animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(0.4f);
        player.Hit(monsterInfo.attackPower);
        yield return new WaitForSeconds(0.1f);
        monsterInfo.animator.SetBool("isAttack", false);
        yield return new WaitForSeconds(1.5f);
        IsAttack = false;
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sphere"))
        {
            PalSphere sphere = collision.gameObject.GetComponent<PalSphere>();
            if(sphere.isToCaptureMonster()) HitBySphere();
        }
    }

    void HitBySphere()
    {
        monsterInfo.FriendlyMode = true;
        Inventory.Instance.Add(monsterInfo);

        transform.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        if(monsterInfo.FriendlyMode)
        {
            monsterInfo.stateMachine.ChangeState(StateName.MCHASE);
            Debug.Log("friendlymode");
        }
    }

}
