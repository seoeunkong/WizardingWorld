using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public float moveSpeed;
    public float jumpForce;

    public float groundCheckDistance = 0.1f;
    bool isGrounded = true;

    Rigidbody _rb;
    Animator _ani;
    Camera _camera;

    bool isJump = false;

    public bool toggleCameraRotation;

    public float smoothness = 10f;

    float hAxis, vAxis;

    void Start()
    {
        _ani = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _camera = Camera.main;

 
    }

    
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        isGrounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, groundCheckDistance, LayerMask.GetMask("Ground"));

        if (isGrounded)
        {
             Run();

            if (Input.GetButtonDown("Jump"))
            {
             //   Jump();
            }
        }
        

        if(Input.GetKey(KeyCode.LeftAlt)) 
        {
            toggleCameraRotation = true; //둘러보기 활성화 
        }
        else
        {
            toggleCameraRotation = false; //둘러보기 비활성화 
        }
        
     
    }


    void FixedUpdate()
    {
        
    }

    private void LateUpdate()
    {
        if (!toggleCameraRotation && (hAxis == 0 && vAxis == 0))
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }
    }

    void Run() //키보드 입력을 통한 움직임 
    {
        if(hAxis != 0 || vAxis != 0) //입력값이 주어진 경우 
        {
            Vector3 inputDir = new Vector3(hAxis, 0, vAxis).normalized;

            Vector3 moveVec = transform.position + inputDir;
            transform.LookAt(moveVec, Vector3.up);

            _ani.SetBool("isRun", true);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        else _ani.SetBool("isRun", false);

    }

    void Jump()
    {
        isJump = true;
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        _ani.SetTrigger("doJump");
    }

}
