using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static UnityEditor.SceneView;
using static UnityEngine.GraphicsBuffer;

public class CameraMove : MonoBehaviour
{
    public Transform objectTofollow; //��ǥ�� 
    public Transform realCam; //���� ī�޶� ��ġ 

    [Header("ī�޶� ���� ������ ")]
    public float followSpeed = 10f;
    public float sensitivity = 100f; //���콺 ���� 
    public float clampAngle = 70f; //�þ� ���� ���� 
    public float smoothness = 10f; //ī�޶� �̵� �� ��ȯ ���ǵ� 
    [SerializeField] private float _minDistance;
    [SerializeField] private float _maxDistance;
    private float _initmaxDistance;

    //���콺 �Է�
    private float _rotX;
    private float _rotY;

    private Vector3 _dirNormalized;
    private float _finalDistance;
    public float offset = 1f; // �÷��̾ ȭ�� ���ʿ� ��ġ��Ű�� ���� z�� ������
    public Vector3 followPos {  get; private set; }


    private PlayerController _playerController;


    private void Start()
    {
        _playerController = Player.Instance.GetComponent<PlayerController>();

        _rotX = transform.localRotation.eulerAngles.x;
        _rotY = transform.localRotation.eulerAngles.y;

        _dirNormalized = realCam.localPosition.normalized; // ī�޶� �̵� ���� ����ȭ
        _finalDistance = realCam.localPosition.magnitude; // ī�޶� ���� �Ÿ� ����

        _initmaxDistance = _maxDistance;

        // ���콺 Ŀ�� ����
        // UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void Update()
    {
        //  ���콺 �Է°��� �޾� ī�޶� ������ ����
        _rotX += -(UnityEngine.Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;
        _rotY += UnityEngine.Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        _rotX = Mathf.Clamp(_rotX, -clampAngle, clampAngle);

        Quaternion rot = Quaternion.Euler(_rotX, _rotY, 0);
        transform.rotation = rot;
    }

    private void LateUpdate()
    {
        Vector3 inputDir = _playerController.inputDirection;

        objectTofollow.localRotation = Quaternion.Euler(0, (-1) * inputDir.x * 90, 0);

        followPos = objectTofollow.position + transform.right * offset; 
        //followPos = objectTofollow.position + objectTofollow.right * offset; 

        transform.position = Vector3.MoveTowards(transform.position, followPos, followSpeed * Time.deltaTime);

        // ��ֹ� ���� �� ���� �Ÿ� ����
        Vector3 _finalDir = transform.TransformPoint(_dirNormalized * _maxDistance);

        RaycastHit hit;
        if (Physics.Linecast(transform.position, _finalDir, out hit))
        {
            _finalDistance = Mathf.Clamp(hit.distance, _minDistance, _maxDistance);
        }
        else
        {
            _finalDistance = _maxDistance;
        }
        realCam.localPosition = Vector3.Lerp(realCam.localPosition, _dirNormalized * _finalDistance, Time.deltaTime * smoothness);

    }

    public void ZoomIn()
    {
        if (_initmaxDistance == _maxDistance) _maxDistance -= 1.5f;
    }

    public void ZoomOut()
    {
        if (_initmaxDistance > _maxDistance) _maxDistance += 1.5f;
    }
}
