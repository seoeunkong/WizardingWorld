using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _laserDamage;
    public bool Active = true;

    private void OnTriggerEnter(Collider other)
    {
        CharacterController controller = other.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.Hit(_laserDamage);
            if (controller is MonsterController mon) mon.monsterInfo.stateMachine.ChangeState(StateName.MHIT);
        }

        if (other.transform != transform.parent)
        {
            transform.gameObject.SetActive(false);
        }
    }
}
