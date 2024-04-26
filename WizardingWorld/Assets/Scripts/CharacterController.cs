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
}
