using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
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
    private const float GROUNDCHECK_DISTANCE = 1.0f;
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
    [SerializeField] private float smoothness;
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
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _angleRange;
    private const float _attackComboTime = 3f;
    private RaycastHit hit;
    public static bool startAttackAni = false;
    public static bool IsAttack = false;
    public bool canAttackCombo { get; private set; }
    #endregion

    [Header("스피어볼 던지기 속성")]
    [SerializeField] private float _throwAngleRange;

    #region #드랍 아이템 획득
    [Header("드랍 아이템 획득")]
    [SerializeField] private float _dropItemCheckDistance;
    public BaseObject baseObject { get; private set; }
    public Transform DropItemPos { get; private set; }
    private List<Transform> _detect = new List<Transform>();
    #endregion

    public bool IsLookFoward() => (inputDirection.x == 0 && inputDirection.z >= 0);
    public void CountAttackCombo() => StartCoroutine(IsAttackCombo());
    public void StopCountAttackCombo() => StopCoroutine(IsAttackCombo());

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
        PlayerRotate();
    }

    void PlayerRotate()
    {
        //카메라 및 플레이어 회전 
        //키보드 입력값이 없는 경우에 둘러보기 활성화 
        bool input = (calculatedDirection == Vector3.zero);
        if (!toggleCameraRotationInput && input)
        {
            CameraMove cameraMove = _camera.GetComponentInParent<CameraMove>();
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
        if (!IsAttack && Input.GetKey(KeyCode.LeftShift) && _currentdashTime >= 0)
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
        if (z != 0)
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
        Vector3 calDirection = (_isOnSlope && _isGrounded) ? AdjustDirectionToSlope(input) : input;
        return calDirection * currentMoveSpeed;
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

    void OnAttack()
    {
        if (IsAttack ) return;

        if (IsLookFoward() && Input.GetMouseButtonDown(0))
        {
            if (!player.weaponManager.hasWeapon()) player.stateMachine.ChangeState(StateName.PUNCHATTACK); //플레이어한테 무기가 없는 경우 
            else player.stateMachine.ChangeState(StateName.ATTACK); //무기가 있는 경우     
        }
    }

    bool IsValidTarget(float angle, Transform enemy) //몬스터가 부채꼴 내부에 있는지 판단 
    {
        Vector3 playerToEnemy = enemy.position - transform.position;
        playerToEnemy.Normalize();

        float dot = Vector3.Dot(playerToEnemy, transform.forward);
        float theta = Mathf.Acos(dot);
        float degree = Mathf.Rad2Deg * theta;

        // 시야각 판별
        if (degree <= angle / 2f) return true;

        return false;
    }

    public void CheckEnemy(float attackPower) //플레이어 공격 영역에 있는 몬스터들 검출 
    {
        // attackPower == 0 : 스피어볼 던지기
        // attackPower > 0 : 일반 공격 

        List<Transform> targets = new List<Transform>();
        Collider[] colls = Physics.OverlapSphere(transform.position, _maxDistance * (attackPower > 0 ? 1 : 10f));
        foreach (Collider coll in colls)
        {
            if (coll.gameObject.CompareTag("Enemy"))
            {
                bool canAttack = IsValidTarget(_angleRange, coll.transform);
                if(canAttack)
                {
                    Attacking(coll.transform, attackPower);
                    targets.Add(coll.transform);
                }
            }
        }

        //스피어볼을 던질 수 있는 경우라면 
        if (targets.Count > 0 && attackPower == 0)
        {
            Transform monster = FindCloseItem(targets);
            Debug.Log(monster.name);
        }
        else Debug.Log("nothing");
    }

    void Attacking(Transform enemy, float attackPower) //몬스터에게 데미지 주기 
    {
        MonsterController monster = enemy.GetComponent<MonsterController>();
        monster?.Hit(attackPower);   
        IsAttack = false;
    }

    IEnumerator IsAttackCombo()
    {
        if (canAttackCombo || !IsAttack) yield return null;

        canAttackCombo = true;
        yield return new WaitForSeconds(_attackComboTime); 
        canAttackCombo = false;
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
            if (coll.GetComponent<DropItem>() != null && coll.GetComponent<DropItem>().enabled == true)
            {
                if (!_detect.Contains(coll.transform) && coll.gameObject != transform) _detect.Add(coll.transform);
                newdetect.Add(coll.transform);
            }
        }

        for (int i = _detect.Count - 1; i >= 0; i--)
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
            if (closeDropItem == null) return;

            closeDropItem.AddOutlineMat(true);
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
            int dir = Input.GetAxis("Mouse ScrollWheel") < 0 ? -1 : 1;
            Inventory.Instance.SetNextWeapon(dir);
        }
    }

    void ThrowSphere()
    {
        CameraMove cameraMove = _camera.GetComponentInParent<CameraMove>();
        if (Input.GetKey("q"))
        {
            Inventory.Instance.SetSphere();
            if (player.hasSphere() == null) return; //플레이어 손에 스피어가 장착 x 
            cameraMove.ZoomIn();
        }

        if (Input.GetKeyUp("q") && player.hasSphere())
        {
            StartCoroutine(ZoomOut(cameraMove));
        }
    }

    IEnumerator ZoomOut(CameraMove camera)
    {
        player.stateMachine.ChangeState(StateName.THROW);
        yield return new WaitForSeconds(0.2f);
        camera.ZoomOut();
    }

    public void Hit(float damage)
    {
        float hp = player.CurrentHP - damage;
        if(hp < 0) hp = 0;

        player.SetHPValue(hp);
        player.animator.SetTrigger("onHit");
        player.rigid.AddForce(Vector3.Scale(transform.forward, new Vector3(-1,0,-1)) * 5f, ForceMode.Impulse);
    }

}
