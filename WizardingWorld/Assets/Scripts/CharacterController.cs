using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterController : MonoBehaviour
{
    #region #��� üũ ����
    [Header("��� ���� �˻�")]
    protected float _maxSlopeAngle = 50f;

    protected const float RAY_DISTANCE = 2f;
    protected const float GROUNDCHECK_DISTANCE = 1.5f;
    protected RaycastHit _slopeHit;
    protected bool _isOnSlope;
    #endregion

    #region #�ٴ� üũ ����
    [Header("�� üũ")]
    [SerializeField, Tooltip("ĳ���Ͱ� ���� �پ� �ִ��� Ȯ���ϱ� ���� CheckBox ���� �����Դϴ�.")]
    protected int _groundLayer;
    protected bool _isGrounded;
    #endregion

    public Vector3 gravity { get; protected set; }
}
