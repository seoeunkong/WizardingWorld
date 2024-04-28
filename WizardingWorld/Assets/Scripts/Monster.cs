using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Monster : Enemy
{
    public StateMachine<MonsterController> stateMachine { get; private set; }

    [Header("몬스터 스탯")]
    private bool _friendlyMode = false;

    [Header("체력")]
    [SerializeField] private float _dangerHpWeight = 0.3f;
    public bool dangerState => maxHP * _dangerHpWeight >= CurrentHP;

 
    public bool FriendlyMode { get { return _friendlyMode; } set { _friendlyMode = value; } }


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


    public bool IsTargetAttackable(CharacterController target)
    {
        if (target is MonsterController monster && (monster.monsterInfo.stateMachine.CurrentState == stateMachine.GetState(StateName.MHIT) ||
            monster.Dead))
        {
            return true;
        }
        return false;
    }



}
