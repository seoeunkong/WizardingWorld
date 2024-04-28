using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : Enemy
{
    public StateMachine<BossMonsterController> stateMachine { get; private set; }

    void Start()
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
        BossMonsterController controller = GetComponent<BossMonsterController>();

        stateMachine = new StateMachine<BossMonsterController>(StateName.BMIDLE, new BMIdleState(controller));
        stateMachine.AddState(StateName.BMMISSILE, new BMMissileState(controller));
    }
}
