using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : BaseWeapon
{
    public override void Attack(BaseState state)
    {
        Debug.Log(Animator.StringToHash("onAttack"));
        //Player.Instance.animator.SetTrigger(Animator.StringToHash("onAttack"));
        Player.Instance.animator.SetTrigger("onAttack");
    }

   
}
