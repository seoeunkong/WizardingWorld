using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : Enemy
{
    public StateMachine<BossMonsterController> stateMachine { get; private set; }

    public bool currentStateIdle => stateMachine.CurrentState == stateMachine.GetState(StateName.BMIDLE);

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
        stateMachine.AddState(StateName.BMLASER, new BMLaserState(controller));
        stateMachine.AddState(StateName.BMMELEE, new BMMeleeState(controller));
        stateMachine.AddState(StateName.BMCHASE, new BMChaseState(controller));
        stateMachine.AddState(StateName.BMHIT, new BMHitState(controller));
        stateMachine.AddState(StateName.BMDEAD, new BMDeadState(controller));
    }
}
