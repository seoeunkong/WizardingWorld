using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public float moveSpeed;
    public float jumpForce;

    public float groundCheckDistance = 0.1f;
    public bool isGrounded = true;

    Rigidbody _rb;
    Animator _ani;
    Camera _camera;

    bool isJump = false;

    public bool toggleCameraRotation;

    public float smoothness = 5f;

    float hAxis, vAxis;

    void Start()
    {
        _ani = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _camera = Camera.main;

 
    }

    
    void Update()
    {

        isGrounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, groundCheckDistance, LayerMask.GetMask("Ground"));

        Run();
        Jump();

        if (isGrounded) isJump = false;

        if(Input.GetKey(KeyCode.LeftAlt)) 
        {
            toggleCameraRotation = true; //�ѷ����� Ȱ��ȭ 
        }
        else
        {
            toggleCameraRotation = false; //�ѷ����� ��Ȱ��ȭ 
        }
     
    }


    void FixedUpdate()
    {
        
    }

    private void LateUpdate()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        bool input = (hAxis == 0 && vAxis == 0) && !Input.GetButtonDown("Jump");
        if (!toggleCameraRotation && input)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }
    }

    void Run() //Ű���� �Է��� ���� ������ 
    {
        if(!isGrounded) return;

        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        if (hAxis != 0 || vAxis != 0) //�Է°��� �־��� ��� 
        {
            if(hAxis != 0 && vAxis == 0)
            {
                _ani.SetBool("isRun", false);
              
                transform.Translate(transform.right * hAxis * moveSpeed * Time.deltaTime, Space.World);
               
                if(hAxis < 0) _ani.SetBool("isLeft", true);
                else _ani.SetBool("isRight", true);
            }
            else if(vAxis != 0)
            {
                _ani.SetBool("isLeft", false);
                _ani.SetBool("isRight", false);

                if (vAxis < 0)
                {
                    Vector3 playerRotate = _camera.transform.forward;
                    playerRotate.y = 0;
                    transform.rotation = Quaternion.LookRotation(playerRotate.normalized * vAxis);
                }

                transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
               
                _ani.SetBool("isRun", true);
            }
        }
        else //�Է°��� ���� ��� 
        {
            _ani.SetBool("isRun", false);
            _ani.SetBool("isLeft", false);
            _ani.SetBool("isRight", false);
        }

    }

    void Jump()
    {
        if (!isGrounded) return;

        if (Input.GetButtonDown("Jump"))
        {
            isJump = true;
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _ani.SetTrigger("doJump");
        }
        
    }

}
