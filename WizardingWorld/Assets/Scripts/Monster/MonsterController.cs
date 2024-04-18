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

    #region #���� ����
    public float MaxHP { get { return _maxHP; } }
    public float CurrentHP { get { return _currentHP; } }
    public float MoveSpeed { get { return _moveSpeed; } }
    public float DashSpeed { get { return _dashSpeed; } }
    public float AttackPower { get { return _attackPower; } }
    public float CurrentSpeed { get { return _currentSpeed; } set { _currentSpeed = value; } }


    [Header("���� ����")]
    [SerializeField] protected float _maxHP;
    [SerializeField] protected float _currentHP;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _dashSpeed;
    [SerializeField] protected float _attackPower;
    [SerializeField] protected float _currentSpeed;
    [SerializeField] protected float _rotateSpeed;

    private float _fieldOfView = 90.0f; // �þ߰� ����
    #endregion

    [Header("���� ���� �Ӽ�")]
    [SerializeField] private float _patrolRadius;
    private Vector3 _patrolPoint;
    public void ResetPatrolPoint() => _patrolPoint = Vector3.zero;

    [Header("�÷��̾� ���� �Ӽ�")]
    [SerializeField] private float _checkDistance;


    #region #��� üũ ����
    [Header("��� ���� �˻�")]
    private float _maxSlopeAngle = 50f;

    private const float RAY_DISTANCE = 2f;
    private const float GROUNDCHECK_DISTANCE = 1.5f;
    private RaycastHit _slopeHit;
    private bool _isOnSlope;
    #endregion

    #region #�ٴ� üũ ����
    [Header("�� üũ")]
    [SerializeField, Tooltip("ĳ���Ͱ� ���� �پ� �ִ��� Ȯ���ϱ� ���� CheckBox ���� �����Դϴ�.")]
    private int _groundLayer;
    private bool _isGrounded;
    #endregion

    #region #���� �ִϸ��̼�
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
        rigid.velocity = GetDirection(dir) * CurrentSpeed;
        
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.Scale(dir, new Vector3(1, 0, 1))), Time.deltaTime * _rotateSpeed);
    }

    public Transform CheckPlayer(float size = 1) //���� ���� ���� �÷��̾� ���� ���� üũ 
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

    bool IsPlayerInSight(Vector3 playerPos)    // ���� ������ ���� �÷��̾ �þ߰� ���� ���� �ִ��� Ȯ��
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

}
