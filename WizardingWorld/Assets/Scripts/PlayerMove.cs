using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public float moveSpeed;

    Rigidbody rb;
    Animator ani;

    public GameObject camera;
    CameraMove cameraMove;


    void Start()
    {
        ani = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        cameraMove = camera.gameObject.GetComponent<CameraMove>();
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

        if(hAxis != 0 || vAxis != 0) //�Է°��� �־��� ��� 
        {
            Vector3 inputDir = new Vector3(hAxis, 0, vAxis).normalized;

            cameraMove.Rotate(transform.position + inputDir);

            transform.LookAt(transform.position + inputDir, Vector3.up);

            ani.SetBool("isRun", true);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        else
        {
            ani.SetBool("isRun", false);
        }
    }


}
