using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEditor.SceneView;

public class PlayerController : MonoBehaviour
{
    public Player player { get; private set; }
    public Vector3 inputDirection { get; private set; } //키보드 입력으로 들어온 이동방향 
    public Vector3 calculatedDirection { get; private set; } //경사 지형 등을 계산한 방향 
    public Vector3 gravity { get; private set; }


    #region #경사 체크 변수
    [Header("경사 지형 검사")]
    private float _maxSlopeAngle = 50f;

    private const float RAY_DISTANCE = 2f;
    private const float GROUNDCHECK_DISTANCE = 1.5f;
    private RaycastHit _slopeHit;
    private bool _isOnSlope;
    #endregion

    #region #바닥 체크 변수
    [Header("땅 체크")]
    [SerializeField, Tooltip("캐릭터가 땅에 붙어 있는지 확인하기 위한 CheckBox 시작 지점입니다.")]
    private int _groundLayer;
    private bool _isGrounded;
    #endregion

    #region #카메라 
    [Header("카메라 속성")]
    private Camera _camera;
    public float smoothness;
    public bool toggleCameraRotationInput { get; private set; } // 둘러보기 입력 여부
    #endregion

    #region #대쉬
    [Header("Dash 속성")]
    public float _currentdashTime;
    public float setDashTime = 10f;
    #endregion

    #region #공격 
    [Header("공격 속성")]
    [SerializeField] private Transform _attackPos;
    private RaycastHit hit;
    [SerializeField] private float _maxDistance;
    #endregion

    #region #드랍 아이템 획득
    [Header("드랍 아이템 획득")]
    [SerializeField] private float _dropItemCheckDistance;
    public BaseObject baseObject { get; private set; }
    public Transform DropItemPos { get; private set; }
    private List<Transform> _detect = new List<Transform>();

    #endregion

    void Start()
    {
        player = GetComponent<Player>();
        _groundLayer = 1 << LayerMask.NameToLayer("Ground");
        _camera = Camera.main;
        _currentdashTime = setDashTime;

        _detect = new List<Transform>();
    }

    void Update()
    {
        // 둘러보기 입력 받기
        toggleCameraRotationInput = Input.GetKey(KeyCode.LeftAlt);

        player.CurrentSpeed = IsDash();

        calculatedDirection = GetDirection(player.CurrentSpeed);
        ControlGravity();

        OnAttack();

        CheckDropItem();

        ChangeWeapon();

        ThrowSphere();

    }

    private void LateUpdate()
    {
        //카메라 및 플레이어 회전 
        //키보드 입력값이 없는 경우에 둘러보기 활성화 
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
        if (_isGrounded && _isOnSlope)
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

        if (!Input.GetKey(KeyCode.LeftShift) && _currentdashTime < setDashTime)
        {
            _currentdashTime += Time.deltaTime;
        }

        return player.MoveSpeed;
    }

    Vector3 OnMoveInput()
    {
        inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (inputDirection == Vector3.zero) return Vector3.zero; //입력값이 없으면 

        LookAt(inputDirection.x, inputDirection.z);

        return transform.forward;
    }

    void LookAt(float x, float z)
    {
        Vector3 playerRotate = Vector3.zero;
        float inputValue = 0;

        if (x != 0)
        {
            playerRotate = _camera.transform.right;
            inputValue = x;
        }
        else if (z != 0)
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

        calculatedDirection = (_isOnSlope && _isGrounded) ? AdjustDirectionToSlope(input) : input;
        return calculatedDirection * currentMoveSpeed;
    }

    bool IsOnSlope() //경사 지형 체크 
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out _slopeHit, RAY_DISTANCE, _groundLayer))
        {
            var angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle != 0f && angle < _maxSlopeAngle;
        }
        return false;
    }

    Vector3 AdjustDirectionToSlope(Vector3 direction) //경사 지형에 맞는 이동 벡터 
    {
        return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
    }

    bool hasWeapon()
    {
        //손에 무기가 있는지 체크 
        foreach (Transform child in player.rightHand)
        {
            if (child.gameObject.activeSelf) return true;
        }
        return false;
    }

    void OnAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float attackPower = 0;

            if (!hasWeapon()) //플레이어한테 무기가 없는 경우 
            {
                player.stateMachine.ChangeState(StateName.PUNCHATTACK);
                attackPower = player.AttackPower;
            }
            else //무기가 있는 경우 
            {
                player.stateMachine.ChangeState(StateName.ATTACK);
                attackPower = player.weaponManager.Weapon.AttackDamage;
            }

            if (Physics.Raycast(_attackPos.position, _attackPos.forward, out hit, _maxDistance))
            {
                hit.collider.GetComponent<HitTest>().HP -= attackPower;
                //Debug.DrawRay(_attackPos.position, _attackPos.forward * hit.distance, Color.red);
            }
        }

    }

    Transform FindCloseItem(List<Transform> newDetect)
    {
        Transform result = newDetect[0];
        float distance = Vector3.Distance(player.transform.position, result.position);

        foreach (Transform item in newDetect)
        {
            float dist = Vector3.Distance(player.transform.position, item.transform.position);
            if (dist < distance)
            {
                distance = dist;
                result = item.transform;
            }
        }

        return result;
    }

    void CheckDropItem()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, _dropItemCheckDistance);
        List<Transform> newdetect = new List<Transform>();

        // 의사코드 
        // 범위 내에 존재하는 모든 item/weapon을 newDetect 리스트에 저장하기
        // newDetect 리스트에 저장되어 있는 데이터 중 플레이어와 가깝게 위치한 데이터 뽑아서 DropItemPos로 업데이트하기
        // 이전에 저장한 _detec와 newDetect 리스트 비교. _detect에 존재하지만 newDetect에는 없는 아이템 아웃라인 비활성화하기.


        foreach (Collider coll in colls)
        {
            if (coll.GetComponent<DropItem>() != null)
            {
                if(!_detect.Contains(coll.transform) && coll.gameObject != transform) _detect.Add(coll.transform);
                newdetect.Add(coll.transform);
            }
        }

        for(int i = _detect.Count - 1;  i >= 0; i--)
        {
            if (!newdetect.Contains(_detect[i].transform))
            {
                _detect[i].GetComponent<DropItem>().AddOutlineMat(false);
                _detect.Remove(_detect[i].transform);
            }
        }

        Transform closeItem = null;
        if (_detect.Count > 0)
        {
            closeItem = FindCloseItem(_detect);
        }


        if (DropItemPos?.position != closeItem?.position)
        {
            DropItemPos = closeItem;
        }
        

        if (closeItem != null)
        {

            DropItem closeDropItem = closeItem.GetComponent<DropItem>();

            closeDropItem?.AddOutlineMat(true);
            foreach (Transform item in _detect)
            {
                if (item != closeItem) item.GetComponent<DropItem>().AddOutlineMat(false);
            }
            GetDropItem(closeDropItem);
        }
    }

    void GetDropItem(DropItem item)
    {
        if (DropItemPos == null || item == null) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            item.AddOutlineMat(false);
            BaseObject bo = DropItemPos.GetComponent<BaseObject>();
            Inventory.Instance.Add(bo);

            DropItemPos.gameObject.SetActive(false);
            //Destroy(DropItemPos.gameObject); 

            _detect.Remove(DropItemPos);
            DropItemPos = null;
        }
    }

    void ChangeWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            int dir = Input.GetAxis("Mouse ScrollWheel") < 0? -1 : 1;
            Inventory.Instance.SetNextWeapon(dir);
        }
    }

    void ThrowSphere()
    {
        CameraMove cameraMove = _camera.GetComponentInParent<CameraMove>();
        if (Input.GetKey("q"))
        {
            //player.animator.SetTrigger("onThrow");
           
            cameraMove.ZoomIn();
        }

        if (Input.GetKeyUp("q"))
        {
            StartCoroutine(ZoomOut(cameraMove));
        }
    }

    IEnumerator ZoomOut(CameraMove camera)
    {
        player.animator.SetTrigger("onThrow");
        yield return new WaitForSeconds(0.2f);
        camera.ZoomOut();
    }


}
