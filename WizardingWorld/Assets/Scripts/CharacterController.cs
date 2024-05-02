using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Burst.CompilerServices;
using UnityEngine;

public interface IStateChangeable
{
    void ChangeState(StateName state);
}

public abstract class CharacterController : MonoBehaviour
{
    #region #��� üũ ����
    [Header("��� ���� �˻�")]
    protected float _maxSlopeAngle = 50f;

    protected const float RAY_DISTANCE = 2f;
    protected const float GROUNDCHECK_DISTANCE = 1.7f;
    protected RaycastHit _slopeHit;
    protected bool _isOnSlope;
    #endregion

    #region #�ٴ� üũ ����
    [Header("�� üũ")]
    protected int _groundLayer;
    protected bool _isGrounded;
    #endregion

    public Vector3 gravity { get; protected set; }

    public bool IsGrounded()
    {
        _isGrounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, GROUNDCHECK_DISTANCE, _groundLayer);
        return _isGrounded;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(this.transform.position + Vector3.up, Vector3.down * GROUNDCHECK_DISTANCE);
    }

    protected bool IsOnSlope() //��� ���� üũ 
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out _slopeHit, RAY_DISTANCE, _groundLayer))
        {
            var angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle != 0f && angle < _maxSlopeAngle;
        }
        return false;
    }

    protected Vector3 AdjustDirectionToSlope(Vector3 direction) //��� ������ �´� �̵� ���� 
    {
        return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
    }

    protected StateName GetHitState(CharacterController character)
    {
        if (character is BossMonsterController)
            return StateName.BMHIT;
        else
            return StateName.MHIT;
    }


    public abstract void Hit(float damage);

    public abstract void Attacking(CharacterController characterController);
}
