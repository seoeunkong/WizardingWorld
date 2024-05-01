using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private float MissileDistance;
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
        return dist < MissileDistance;
    }


}
