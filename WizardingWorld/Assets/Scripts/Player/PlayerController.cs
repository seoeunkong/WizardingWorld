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

    #region #�뽬
    [Header("Dash �Ӽ�")]
    public float _currentdashTime;
    public float setDashTime = 10f;
    #endregion

    #region #���� 
    [Header("���� �Ӽ�")]
    [SerializeField] private Transform _attackPos;
    private RaycastHit hit;
    [SerializeField] private float _maxDistance;
    #endregion

    #region #��� ������ ȹ��
    [Header("��� ������ ȹ��")]
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
        // �ѷ����� �Է� �ޱ�
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
        //ī�޶� �� �÷��̾� ȸ�� 
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
        if (inputDirection == Vector3.zero) return Vector3.zero; //�Է°��� ������ 

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

    bool hasWeapon()
    {
        //�տ� ���Ⱑ �ִ��� üũ 
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

            if (!hasWeapon()) //�÷��̾����� ���Ⱑ ���� ��� 
            {
                player.stateMachine.ChangeState(StateName.PUNCHATTACK);
                attackPower = player.AttackPower;
            }
            else //���Ⱑ �ִ� ��� 
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

        // �ǻ��ڵ� 
        // ���� ���� �����ϴ� ��� item/weapon�� newDetect ����Ʈ�� �����ϱ�
        // newDetect ����Ʈ�� ����Ǿ� �ִ� ������ �� �÷��̾�� ������ ��ġ�� ������ �̾Ƽ� DropItemPos�� ������Ʈ�ϱ�
        // ������ ������ _detec�� newDetect ����Ʈ ��. _detect�� ���������� newDetect���� ���� ������ �ƿ����� ��Ȱ��ȭ�ϱ�.


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
