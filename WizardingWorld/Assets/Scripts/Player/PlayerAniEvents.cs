using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAniEvents : MonoBehaviour
{
    public void OnFinishedAttack() => Player.Instance.stateMachine.ChangeState(StateName.MOVE);
    public void ActivateThrowSphere() => ThrowState.throwSphere = true;

    public void SetOnAttack() => PlayerController.startAttackAni = true;

}

