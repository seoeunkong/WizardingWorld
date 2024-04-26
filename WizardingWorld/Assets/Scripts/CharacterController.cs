using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterController : MonoBehaviour
{
    #region #경사 체크 변수
    [Header("경사 지형 검사")]
    protected float _maxSlopeAngle = 50f;

    protected const float RAY_DISTANCE = 2f;
    protected const float GROUNDCHECK_DISTANCE = 1.5f;
    protected RaycastHit _slopeHit;
    protected bool _isOnSlope;
    #endregion

    #region #바닥 체크 변수
    [Header("땅 체크")]
    [SerializeField, Tooltip("캐릭터가 땅에 붙어 있는지 확인하기 위한 CheckBox 시작 지점입니다.")]
    protected int _groundLayer;
    protected bool _isGrounded;
    #endregion

    public Vector3 gravity { get; protected set; }

    private void Start()
    {
        _groundLayer = 1 << LayerMask.NameToLayer("Ground");
    }

    public bool IsGrounded()
    {
        _isGrounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, GROUNDCHECK_DISTANCE, _groundLayer);
        return _isGrounded;
    }

    protected bool IsOnSlope() //경사 지형 체크 
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out _slopeHit, RAY_DISTANCE, _groundLayer))
        {
            var angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle != 0f && angle < _maxSlopeAngle;
        }
        return false;
    }

    protected Vector3 AdjustDirectionToSlope(Vector3 direction) //경사 지형에 맞는 이동 벡터 
    {
        return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
    }

    public abstract void Hit(float damage);

    public abstract void Attacking(CharacterController characterController);
}
