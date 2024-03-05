using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Player player { get; private set; }
    public Vector3 inputDirection { get; private set; } //Ű���� �Է����� ���� �̵����� 
    public Vector3 calculatedDirection { get; private set; } //��� ���� ���� ����� ���� 
    public Vector3 gravity { get; private set; }


    #region #��� üũ ����
    [Header("��� ���� �˻�")]
    private float _maxSlopeAngle = 50f;

    private const float RAY_DISTANCE = 2f;
    private const float GROUNDCHECK_DISTANCE = 1.5f;
    private RaycastHit _slopeHit;
    private bool _isOnSlope;
    #endregion

    #region #�ٴ� üũ ����
    [Header("�� üũ")]
    [SerializeField, Tooltip("ĳ���Ͱ� ���� �پ� �ִ��� Ȯ���ϱ� ���� CheckBox ���� �����Դϴ�.")]
    private int _groundLayer;
    private bool _isGrounded;
    #endregion

    #region #ī�޶� 
    [Header("ī�޶� �Ӽ�")]
    private Camera _camera;
    public float smoothness;
    public bool toggleCameraRotationInput { get; private set; } // �ѷ����� �Է� ����
    #endregion


    [Header("Dash �Ӽ�")]
    public float _currentdashTime;
    public float setDashTime = 10f;

    public bool onAttack;


    void Start()
    {
        player = GetComponent<Player>();
        _groundLayer = 1 << LayerMask.NameToLayer("Ground");
        _camera = Camera.main;
        _currentdashTime = setDashTime;
    }

    void Update()
    {
        // �ѷ����� �Է� �ޱ�
        toggleCameraRotationInput = Input.GetKey(KeyCode.LeftAlt);

        player.CurrentSpeed = IsDash();

        calculatedDirection = GetDirection(player.CurrentSpeed);
        ControlGravity();

        onAttack = OnAttack();

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
        gravity = Vector3.down * MathF.Abs(player.rigid.velocity.y);
        if(_isGrounded && _isOnSlope)
        {
            gravity = Vector3.zero;
            player.rigid.useGravity = false;
            return;
        }
        player.rigid.useGravity = true;
    }

    public bool IsGrounded()
    {
        _isGrounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, GROUNDCHECK_DISTANCE, _groundLayer);
        return _isGrounded;
    }

    public float IsDash()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _currentdashTime >= 0)
        {
            _currentdashTime -= Time.deltaTime;
            return player.DashSpeed;
        }

        if(!Input.GetKey(KeyCode.LeftShift) && _currentdashTime < setDashTime)
        {
            _currentdashTime += Time.deltaTime;
        }

        return player.MoveSpeed;
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
        _isOnSlope = IsOnSlope();
        _isGrounded = IsGrounded();

        Vector3 input = OnMoveInput();

        calculatedDirection = (_isOnSlope && _isGrounded) ? AdjustDirectionToSlope(input): input;
        return calculatedDirection * currentMoveSpeed;
    }

    bool IsOnSlope() //��� ���� üũ 
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out _slopeHit, RAY_DISTANCE, _groundLayer))
        {
            var angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle != 0f && angle < _maxSlopeAngle;
        }
        return false;
    }

    Vector3 AdjustDirectionToSlope(Vector3 direction) //��� ������ �´� �̵� ���� 
    {
        return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
    }

    bool OnAttack()
    {
        if(Input.GetMouseButtonDown(0))
        {
            return true;
        }
        return false;
    }
}
