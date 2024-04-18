using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class MonsterController : MonoBehaviour
{

    public StateMachine<MonsterController> stateMachine { get; private set; }
    public Rigidbody rigid { get; private set; }
    public Animator animator { get; private set; }
    public Vector3 gravity { get; private set; }

    #region #몬스터 스탯
    public float MaxHP { get { return _maxHP; } }
    public float CurrentHP { get { return _currentHP; } }
    public float MoveSpeed { get { return _moveSpeed; } }
    public float DashSpeed { get { return _dashSpeed; } }
    public float AttackPower { get { return _attackPower; } }
    public float CurrentSpeed { get { return _currentSpeed; } set { _currentSpeed = value; } }


    [Header("몬스터 스탯")]
    [SerializeField] protected float _maxHP;
    [SerializeField] protected float _currentHP;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _dashSpeed;
    [SerializeField] protected float _attackPower;
    [SerializeField] protected float _currentSpeed;
    [SerializeField] protected float _rotateSpeed;

    private float _fieldOfView = 90.0f; // 시야각 설정
    #endregion

    [Header("몬스터 순찰 속성")]
    [SerializeField] private float _patrolRadius;
    private Vector3 _patrolPoint;
    public void ResetPatrolPoint() => _patrolPoint = Vector3.zero;

    [Header("플레이어 감지 속성")]
    [SerializeField] private float _checkDistance;


    #region #경사 체크 변수
    [Header("경사 지형 검사")]
    private float _maxSlopeAngle = 50f;

    private const float RAY_DISTANCE = 2f;
    private const float GROUNDCHECK_DISTANCE = 1.5f;
    private RaycastHit _slopeHit;
    private bool _isOnSlope;
    #endregion

    #region #바닥 체크 변수
    [Header("땅 체크")]
    [SerializeField, Tooltip("캐릭터가 땅에 붙어 있는지 확인하기 위한 CheckBox 시작 지점입니다.")]
    private int _groundLayer;
    private bool _isGrounded;
    #endregion

    #region #몬스터 애니메이션
    public bool Sense { get; private set; }
    public void SenseTrue() => Sense = true;
    public void SenseFalse() => Sense = false;
    #endregion

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

    }

    void Start()
    {
        InitStateMachine();
        Init();
    }

    void Update()
    {
        stateMachine?.UpdateState();
    }

    void FixedUpdate()
    {
        stateMachine?.FixedUpdateState();
    }


    void InitStateMachine()
    {
        MonsterController controller = GetComponent<MonsterController>();
        stateMachine = new StateMachine<MonsterController>(StateName.IDLE, new IdleState(controller));
        stateMachine.AddState(StateName.MRUN, new MRunState(controller));
        //stateMachine.AddState(StateName.ATTACK, new AttackState(controller));
        //stateMachine.AddState(StateName.THROW, new ThrowState(controller));
    }

    private void Init()
    {
        _currentHP = MaxHP;
        _currentSpeed = _moveSpeed;
        _groundLayer = 1 << LayerMask.NameToLayer("Ground");
    }


    public void OnUpdateState(float maxHP, float currentHP, float moveSpeed, float dashSpeed)
    {
        this._maxHP = maxHP;
        this._currentHP = currentHP;
        this._moveSpeed = moveSpeed;
        this._dashSpeed = dashSpeed;
    }

    protected void ControlGravity()
    {
        gravity = Vector3.down * MathF.Abs(rigid.velocity.y);
        if (_isGrounded && _isOnSlope)
        {
            gravity = Vector3.zero;
            rigid.useGravity = false;
            return;
        }
        rigid.useGravity = true;
    }

    public bool IsGrounded()
    {
        _isGrounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, GROUNDCHECK_DISTANCE, _groundLayer);
        return _isGrounded;
    }

    bool IsOnSlope() //경사 지형 체크 
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out _slopeHit, RAY_DISTANCE, _groundLayer))
        {
            var angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle != 0f && angle < _maxSlopeAngle;
        }
        return false;
    }

    Vector3 AdjustDirectionToSlope(Vector3 direction) //경사 지형에 맞는 이동 벡터 
    {
        return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
    }


    public void Patrol()
    {
        Vector3 monster = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
        Vector3 patrol = Vector3.Scale(_patrolPoint, new Vector3(1, 0, 1));

        //PatrolPoint 초기화 
        if (_patrolPoint == Vector3.zero || Vector3.Distance(monster,patrol) <= 1.5f)
        {
            Vector3 point = UnityEngine.Random.insideUnitSphere * _patrolRadius + transform.position;
            _patrolPoint = new Vector3(point.x, 0, point.z);
        }
        _patrolPoint = new Vector3(_patrolPoint.x, transform.position.y, _patrolPoint.z);

        //PatrolPoint을 기준으로 몬스터 이동 및 회전
        Vector3 dir = (_patrolPoint - transform.position).normalized;
        rigid.velocity = GetDirection(dir) * CurrentSpeed;
        
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.Scale(dir, new Vector3(1, 0, 1))), Time.deltaTime * _rotateSpeed);
    }

    public Transform CheckPlayer(float size = 1) //감지 영역 내에 플레이어 존재 여부 체크 
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, _checkDistance * size);
        foreach (Collider coll in colls)
        {
            if(coll.gameObject.CompareTag("Player"))
            {
                if (stateMachine.GetState(StateName.IDLE) == stateMachine.CurrentState && IsPlayerInSight(coll.transform.position)) return coll.transform;
                if(stateMachine.GetState(StateName.MRUN) == stateMachine.CurrentState) return coll.transform;
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

    public void Hit(float damage)
    {
        _currentHP -= damage;
        Debug.Log("Ouch");
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

}
