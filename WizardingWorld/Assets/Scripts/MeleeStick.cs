using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeStick : BaseWeapon
{
    public override void Attack(BaseState<PlayerController> state)
    {
        Player.Instance.animator.SetTrigger("onAttack");
    }

}
