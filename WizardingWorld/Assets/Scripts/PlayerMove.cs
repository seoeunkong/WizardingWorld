using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    
    public float moveSpeed;
    public float jumpForce;

    public float groundCheckDistance = 1.5f;
    private const float RAY_DISTANCE = 2f;
    public bool isGrounded = true;
    public bool forwardjump = false;

    RaycastHit _slopeHit;

    Rigidbody _rb;
    Animator _ani;
    Camera _camera;

    public bool isJump = false;

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

        if (isGrounded || IsOnSlope())
        {
            isJump = false;
            if(hAxis == 0 && vAxis == 0) _rb.velocity = Vector3.zero;
        }
        

        if(Input.GetKey(KeyCode.LeftAlt)) 
        {
            toggleCameraRotation = true; //�ѷ����� Ȱ��ȭ 
        }
        else
        {
            toggleCameraRotation = false; //�ѷ����� ��Ȱ��ȭ 
        }

        Debug.Log(Animator.StringToHash("Velocity"));
     
    }


    void FixedUpdate()
    {
        
    }

    private void LateUpdate()
    {
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

    void Move() //Ű���� �Է��� ���� ������ 
    {
        bool isOnSlope = IsOnSlope();
        if (!isGrounded && !isOnSlope) return;

        if (hAxis != 0 || vAxis != 0) //�Է°��� �־��� ��� 
        {
            _ani.SetBool("isRun", false);
            _ani.SetBool("isLeft", false);
            _ani.SetBool("isRight", false);

            Vector3 dir = Vector3.one;

            if (vAxis == 0) //�¿� �̵� 
            {
                dir = isOnSlope ? AdjustDirectionToSlope(transform.right * hAxis) : transform.right * hAxis;

                if (hAxis < 0) _ani.SetBool("isLeft", true);
                else _ani.SetBool("isRight", true);
            }
            else if(vAxis != 0) //�յ� �̵� 
            {
                if (vAxis < 0) //�ڷ� �̵��ϴ� ��� 
                {
                    Vector3 playerRotate = _camera.transform.forward;
                    playerRotate.y = 0;
                    transform.rotation = Quaternion.LookRotation(playerRotate.normalized * vAxis);
                }

                dir = isOnSlope ? AdjustDirectionToSlope(transform.forward) : transform.forward;

                _ani.SetBool("isRun", true);
            }
            _rb.velocity = dir * moveSpeed;
        }
        else //�Է°��� ���� ��� 
        {
            _ani.SetBool("isRun", false);
            _ani.SetBool("isLeft", false);
            _ani.SetBool("isRight", false);
        }

        Jump();

    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {

            Vector3 jumpVec = Vector3.zero;

            forwardjump = (hAxis != 0 || vAxis != 0);

            if (forwardjump) jumpVec = transform.forward.normalized + Vector3.up;
            else jumpVec = Vector3.up;

            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            _ani.SetTrigger("doJump");
        }
        
    }

    bool IsOnSlope() //��� ���� üũ 
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out _slopeHit, RAY_DISTANCE, LayerMask.GetMask("Ground")))
        {
            var angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle != 0f && angle < maxSlopeAngle;
        }
        return false;
    }

    Vector3 AdjustDirectionToSlope(Vector3 direction) //��� ������ �´� �̵� ���� 
    {
        return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
    }

}
