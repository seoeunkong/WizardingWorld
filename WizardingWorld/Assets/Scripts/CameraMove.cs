using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static UnityEditor.SceneView;
using static UnityEngine.GraphicsBuffer;

public class CameraMove : MonoBehaviour
{
    public Transform objectTofollow; //목표물 
    public Transform realCam; //실제 카메라 위치 

    [Header("카메라 동작 설정값 ")]
    public float followSpeed = 10f;
    public float sensitivity = 100f; //마우스 감도 
    public float clampAngle = 70f; //시야 제한 각도 
    public float smoothness = 10f; //카메라 이동 시 전환 스피드 
    [SerializeField] private float _minDistance;
    [SerializeField] private float _maxDistance;
    private float _initmaxDistance;

    //마우스 입력
    private float _rotX;
    private float _rotY;

    private Vector3 _dirNormalized;
    private Vector3 _finalDir;
    private float _finalDistance;

    private Vector3 followPos;

    private PlayerController _playerController;


    private void Start()
    {
        _playerController = Player.Instance.GetComponent<PlayerController>();

        _rotX = transform.localRotation.eulerAngles.x;
        _rotY =  transform.localRotation.eulerAngles.y;

        _dirNormalized = realCam.localPosition.normalized; // 카메라 이동 방향 정규화
        _finalDistance = realCam.localPosition.magnitude; // 카메라 최종 거리 설정

        _initmaxDistance = _maxDistance;

        // 마우스 커서 설정
       // UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        
    }

    private void Update()
    {
        //  마우스 입력값을 받아 카메라 시점을 조절
        _rotX += -(UnityEngine.Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;
        _rotY += UnityEngine.Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        _rotX = Mathf.Clamp(_rotX, -clampAngle, clampAngle);

        Quaternion rot = Quaternion.Euler(_rotX, _rotY, 0);
        transform.rotation = rot;

       
        Vector3 inputDir = _playerController.inputDirection;
        followPos = CalcPos(inputDir);
    }

    private void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, followPos, followSpeed * Time.deltaTime);
        _finalDir = transform.TransformPoint(_dirNormalized * _maxDistance);

        // 장애물 감지 후 최종 거리 설정
        RaycastHit hit;
        if(Physics.Linecast(transform.position, _finalDir, out hit))
        {
            _finalDistance = Mathf.Clamp(hit.distance, _minDistance, _maxDistance);
        }
        else
        {
            _finalDistance = _maxDistance;
        }
        realCam.localPosition = Vector3.Lerp(realCam.localPosition, _dirNormalized * _finalDistance, Time.deltaTime * smoothness);
         
    }

    public Vector3 CalcPos(Vector3 inputDir)
    {
        Vector3 targetPos = objectTofollow.position;

        if (inputDir.x != 0)
        {
            targetPos = objectTofollow.position - objectTofollow.right;
            targetPos += objectTofollow.forward * inputDir.x;
        }

        if (inputDir.z < 0)
        {
            targetPos -= objectTofollow.right;
        }
        else if (inputDir.z >= 0)
        {
            targetPos += objectTofollow.right;
        }

        return targetPos;
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
