using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ThrowState : BaseState<PlayerController>
{
    public static bool throwSphere = false;
    private Rigidbody _sphereRb;
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
        _sphereRb = null;
    }

    public override void OnFixedUpdateState()
    {
        if (throwSphere)
        {
            _ps = Player.Instance.hasSphere();
            if (_ps != null) //플레이어 손에 스피어가 있다면 
            {
                _ps.InitThrowSphere();

                _sphereRb = _ps.GetComponent<Rigidbody>();

                _Controller.CheckEnemy(false);

                Vector3 dir = Player.Instance.transform.forward;
                if (_Controller.closeMonster != null)
                {
                    Vector3 cam = _Controller._camera.transform.position - new Vector3(0,2f,0);
                    dir = (_Controller.closeMonster.position - cam).normalized;

                    _sphereRb.AddForce(dir * Player.Instance.ThrowPower, ForceMode.Impulse);

                }
                else _sphereRb.AddForce(_Controller._camera.transform.forward * Player.Instance.ThrowPower, ForceMode.Impulse);
            }
            throwSphere = false;
        }
    }

    public override void OnUpdateState()
    {

    }

}
