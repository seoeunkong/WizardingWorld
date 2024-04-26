using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Monster : BaseObject
{
    public StateMachine<MonsterController> stateMachine { get; private set; }
    public Rigidbody rigid { get; private set; }
    public Animator animator { get; private set; }

    [Header("몬스터 생성"), Tooltip("몬스터 스탯 정보")]
    [SerializeField] protected MonsterData monsterData;


    [Header("몬스터 스탯")]
    [SerializeField] private float _currentHP; //후에 접근제어자 수정할 예정 
    [SerializeField] private float _currentSpeed;
    private bool _friendlyMode = false;

    public float maxHP { get; private set; }
    public float moveSpeed { get; private set; }
    public float dashSpeed { get; private set; }
    public float attackPower { get; private set; }
    public float rotateSpeed { get; private set; }
    public int level { get; private set; }
    public float CurrentHP { get { return _currentHP; } }
    public float CurrentSpeed { get { return _currentSpeed; } set { _currentSpeed = value; } }
    public bool FriendlyMode { get { return _friendlyMode; } set { _friendlyMode = value; } }

    public void SetHP(float hp) => this._currentHP = hp; //이 부분도 이후에 수정

    private void Awake()
    {
        InitializeData(monsterData);
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        InitStateMachine();
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
        stateMachine.AddState(StateName.MCHASE, new MChaseState(controller));
        stateMachine.AddState(StateName.MATTACK, new MAttackState(controller));
        stateMachine.AddState(StateName.MHIT, new MHitState(controller));
        stateMachine.AddState(StateName.MDEAD, new MDeadState(controller));
    }

    public override void InitializeData(ObjectData monsterData)
    {
        _objData = monsterData;
        OnUpdateState();
    }

    public void OnUpdateState()
    {
        this.maxHP = monsterData.MaxHP;
        this.attackPower = monsterData.AttackPower;
        this.moveSpeed = monsterData.MoveSpeed;
        this.dashSpeed = monsterData.DashSpeed;
        this.level = monsterData.Level;
        this.rotateSpeed = monsterData.RotateSpeed;

        this._currentSpeed = moveSpeed;
        this._currentHP = maxHP;
    }


}
