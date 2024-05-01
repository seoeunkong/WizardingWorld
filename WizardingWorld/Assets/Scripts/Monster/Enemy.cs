using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : BaseObject
{
    public Rigidbody rigid { get; protected set; }
    public Animator animator { get; protected set; }

    [Header("몬스터 스탯")]
    [SerializeField] protected float _currentHP; //후에 접근제어자 수정할 예정 
    [SerializeField] protected float _currentSpeed;

    public float maxHP { get; protected set; }
    public float moveSpeed { get; protected set; }
    public float dashSpeed { get; protected set; }
    public float attackPower { get; protected set; }
    public float rotateSpeed { get; protected set; }
    public int level { get; protected set; }
    public float CurrentHP { get { return _currentHP; } }
    public float CurrentSpeed { get { return _currentSpeed; } set { _currentSpeed = value; } }

    public void SetHP(float hp)
    {
        this._currentHP = hp;
        OnHealthChanged?.Invoke(this._currentHP); ;
    }

    [Header("몬스터 생성"), Tooltip("몬스터 스탯 정보")]
    [SerializeField] protected MonsterData monsterData;

    public delegate void HealthChanged(float newHealth);
    public event HealthChanged OnHealthChanged;

    private void Awake()
    {
        InitializeData(monsterData);
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
