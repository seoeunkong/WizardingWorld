using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private float _missileDistance;
    [SerializeField] private float _missileDamage;
    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    private void OnEnable()
    {
        if(transform.parent != null) startPos = transform.parent.position;
    }

    void Update()
    {
        if(!SetOn())
        {
            transform.gameObject.SetActive(false);
        }
    }

    bool SetOn()
    {
        float dist = Vector3.Distance(startPos, transform.position);
        return dist < _missileDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterController controller = other.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.Hit(_missileDamage);
            if (controller is MonsterController mon) mon.monsterInfo.stateMachine.ChangeState(StateName.MHIT);
        }

        if(other != transform.parent) transform.gameObject.SetActive(false);
    }


}
