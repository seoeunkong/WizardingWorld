using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeStick : BaseWeapon
{
    public override void Attack(BaseState state)
    {
        Player.Instance.animator.SetTrigger("onAttack");
    }

}
