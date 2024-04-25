using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : BaseObject
{
    [Header("몬스터 생성"), Tooltip("몬스터 스탯 정보")]
    [SerializeField] protected MonsterData monsterData;


    [Header("몬스터 스탯")]
    [SerializeField] private float _currentHP; //후에 접근제어자 수정할 예정 
    [SerializeField] private float _currentSpeed;

    public float maxHP { get; private set; }
    public float moveSpeed { get; private set; }
    public float dashSpeed { get; private set; }
    public float attackPower { get; private set; }
    public float rotateSpeed { get; private set; }
    public int level { get; private set; }
    public float CurrentHP { get { return _currentHP; } }
    public float CurrentSpeed { get { return _currentSpeed; } set { _currentSpeed = value; } }

    public void SetHP(float hp) => this._currentHP = hp;

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
