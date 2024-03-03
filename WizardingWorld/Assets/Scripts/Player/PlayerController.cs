using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public Player player { get; private set; }
    public Vector3 inputDirection { get; private set; } //Ű���� �Է����� ���� �̵����� 
    public Vector3 calculatedDirection { get; private set; } //��� ���� ���� ����� ���� 
    public Vector3 gravity { get; private set; }


    #region #��� üũ ����
    [Header("��� ���� �˻�")]
    float maxSlopeAngle = 50f;

    private const float RAY_DISTANCE = 2f;
    private const float GROUNDCHECK_DISTANCE = 1.5f;
    private RaycastHit slopeHit;
    private bool isOnSlope;
    #endregion

    #region #�ٴ� üũ ����
    [Header("�� üũ")]
    [SerializeField, Tooltip("ĳ���Ͱ� ���� �پ� �ִ��� Ȯ���ϱ� ���� CheckBox ���� �����Դϴ�.")]
    private int groundLayer;
    private bool isGrounded;
    #endregion

    #region #�Է°� 
    public bool toggleCameraRotationInput { get; private set; } // �ѷ����� �Է� ����
    #endregion

    #region #ī�޶� 
    private Camera _camera;
    public float smoothness;
    #endregion

    void Start()
    {
        player = GetComponent<Player>();
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
        _camera = Camera.main;
    }

    void Update()
    {
        // �ѷ����� �Է� �ޱ�
        toggleCameraRotationInput = Input.GetKey(KeyCode.LeftAlt);

        player._CurrentSpeed = IsDash();

        calculatedDirection = GetDirection(player._CurrentSpeed);
        ControlGravity();

    }

    private void LateUpdate()
    {
        //Ű���� �Է°��� ���� ��쿡 �ѷ����� Ȱ��ȭ 
        bool input = (calculatedDirection == Vector3.zero);
        if (!toggleCameraRotationInput && input) 
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }
    }

    protected void ControlGravity()
    {
        gravity = Vector3.down * MathF.Abs(player._rigid.velocity.y);
        if(isGrounded && isOnSlope)
        {
            gravity = Vector3.zero;
            player._rigid.useGravity = false;
            return;
        }
        player._rigid.useGravity = true;
    }

    public bool IsGrounded()
    {
        isGrounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, GROUNDCHECK_DISTANCE, groundLayer);
        return isGrounded;
    }

    public float IsDash()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            return player._DashSpeed;
        }
        return player._MoveSpeed;
    }

    Vector3 OnMoveInput()
    {
        inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (inputDirection == Vector3.zero) return Vector3.zero; //�Է°��� ������ 

        LookAt(inputDirection.x, inputDirection.z);

        return transform.forward;
    }

    void LookAt(float x, float z)
    {
        Vector3 playerRotate = Vector3.zero;
        float inputValue = 0;
        
        if (x!=0)
        {
            playerRotate = _camera.transform.right;
            inputValue = x;
        }
        else if(z!=0)
        {
            playerRotate = _camera.transform.forward;
            inputValue = z;
        }
        playerRotate.y = 0;
        transform.rotation = Quaternion.LookRotation(playerRotate.normalized * inputValue);
    }

    Vector3 GetDirection(float currentMoveSpeed)
    {
        isOnSlope = IsOnSlope();
        isGrounded = IsGrounded();

        Vector3 input = OnMoveInput();

        calculatedDirection = (isOnSlope && isGrounded) ? AdjustDirectionToSlope(input): input;
        return calculatedDirection * currentMoveSpeed;
    }

    bool IsOnSlope() //��� ���� üũ 
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out slopeHit, RAY_DISTANCE, groundLayer))
        {
            var angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle != 0f && angle < maxSlopeAngle;
        }
        return false;
    }

    Vector3 AdjustDirectionToSlope(Vector3 direction) //��� ������ �´� �̵� ���� 
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
