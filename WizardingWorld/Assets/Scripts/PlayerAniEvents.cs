using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAniEvents : MonoBehaviour
{
   public void OnFinishedAttack()
    {
        //PunchAttackState.IsAttack = false;
        Player.Instance.stateMachine.ChangeState(StateName.MOVE);

    }
}

