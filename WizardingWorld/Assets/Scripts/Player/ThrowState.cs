using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowState : BaseState<PlayerController>
{
    public static bool throwSphere = false;
    private PalSphere _ps;

    public ThrowState(PlayerController controller) : base(controller) { }

    public override void OnEnterState()
    {
        _ps = null;
        Player.Instance.animator.SetTrigger("onThrow");
    }

    public override void OnExitState()
    {
            int amount = _ps.Amount;
            _ps.SetAmount(amount - 1);

            int index = Inventory.Instance.FindItem(_ps._objData);
            Inventory.Instance.UpdateSlot(index);

    }

    public override void OnFixedUpdateState()
    {
      if(throwSphere)
        {
            _ps = Player.Instance.hasSphere();
            if(_ps != null) //플레이어 손에 스피어가 있다면 
            {
                _ps.InitThrowSphere();

                Rigidbody rb = _ps.GetComponent<Rigidbody>();
                Vector3 dir = Player.Instance.transform.forward + new Vector3(0, 0.2f, 0);
                rb.AddForce(dir * Player.Instance.ThrowPower ,ForceMode.Impulse);
            }
            throwSphere = false;
        }
    }

    public override void OnUpdateState()
    {
       
    }

}
