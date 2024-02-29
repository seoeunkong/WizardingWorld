using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public float moveSpeed;
    public float jumpForce;

    public float groundCheckDistance = 0.1f;
    private const float RAY_DISTANCE = 2f;
    public bool isGrounded = true;
    public bool forwardjump = false;

    RaycastHit _slopeHit;

    Rigidbody _rb;
    Animator _ani;
    Camera _camera;

    bool isJump = false;

    public bool toggleCameraRotation;
    public float smoothness = 5f;
    float maxSlopeAngle = 50f;

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

        CheckInput();

        Move();
        Jump();

        if (isGrounded) isJump = false;

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
       //CheckInput();

        bool input = (hAxis == 0 && vAxis == 0) && !Input.GetButtonDown("Jump");
        if (!toggleCameraRotation && input)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }
    }

    void CheckInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
    }

    void Move() //키보드 입력을 통한 움직임 
    {
        if (isJump) return;

       // CheckInput();

        if (hAxis != 0 || vAxis != 0) //입력값이 주어진 경우 
        {
            _ani.SetBool("isRun", false);
            _ani.SetBool("isLeft", false);
            _ani.SetBool("isRight", false);

            bool isOnSlope = IsOnSlope();

            if (vAxis == 0) //좌우 이동 
            {
                Vector3 dir = isOnSlope ? AdjustDirectionToSlope(transform.right * hAxis) : transform.right * hAxis;
                transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
               
                if(hAxis < 0) _ani.SetBool("isLeft", true);
                else _ani.SetBool("isRight", true);
            }
            else if(vAxis != 0) //앞뒤 이동 
            {
                if (vAxis < 0) //뒤로 이동하는 경우 
                {
                    Vector3 playerRotate = _camera.transform.forward;
                    playerRotate.y = 0;
                    transform.rotation = Quaternion.LookRotation(playerRotate.normalized * vAxis);
                }

                Vector3 dir = isOnSlope ? AdjustDirectionToSlope(transform.forward) : transform.forward;
                transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
               
                _ani.SetBool("isRun", true);
            }
        }
        else //입력값이 없는 경우 
        {
            _ani.SetBool("isRun", false);
            _ani.SetBool("isLeft", false);
            _ani.SetBool("isRight", false);
        }

    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && !isJump)
        {
            isJump = true;

            Vector3 jumpVec = Vector3.zero;

           // CheckInput();
            forwardjump = (hAxis != 0 || vAxis != 0);

            if (forwardjump) jumpVec = transform.forward.normalized + Vector3.up;
            else jumpVec = Vector3.up;

            _rb.AddForce(jumpVec * jumpForce, ForceMode.Impulse);


            _ani.SetTrigger("doJump");
        }
        
    }

    bool IsOnSlope() //경사 지형 체크 
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out _slopeHit, RAY_DISTANCE, LayerMask.GetMask("Ground")))
        {
            var angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
           // Debug.Log(angle);
            return angle != 0f && angle < maxSlopeAngle;
        }
        return false;
    }

    Vector3 AdjustDirectionToSlope(Vector3 direction) //경사 지형에 맞는 이동 벡터 
    {
        return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
    }

}
