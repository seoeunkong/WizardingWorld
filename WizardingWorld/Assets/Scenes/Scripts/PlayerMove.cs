using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public float moveSpeed;

    Rigidbody rb;
    Animator ani;


    void Start()
    {
        ani = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        Run();
    }


    void FixedUpdate()
    {
        
    }

    void Run() //Ű���� �Է��� ���� ������ 
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(hAxis, 0, vAxis).normalized;

        transform.LookAt(transform.position + inputDir,Vector3.up);

        if(hAxis != 0 || vAxis != 0) //�Է°��� �־��� ��� 
        {
            ani.SetBool("isRun", true);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        else
        {
            ani.SetBool("isRun", false);
        }
    }


}
