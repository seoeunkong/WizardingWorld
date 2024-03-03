using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.SceneView;
using static UnityEngine.GraphicsBuffer;

public class CameraMove : MonoBehaviour
{
    public Transform _objectTofollow; //목표물 
    public Transform _realCam; //실제 카메라 위치 

    [Header("카메라 동작 설정값 ")]
    public float followSpeed = 10f;
    public float sensitivity = 100f; //마우스 감도 
    public float clampAngle = 70f; //시야 제한 각도 
    public float smoothness = 10f; //카메라 이동 시 전환 스피드 
    public float minDistance;
    public float maxDistance;

    //마우스 입력
    private float rotX;
    private float rotY;

    private Vector3 dirNormalized;
    private Vector3 finalDir;
    private float finalDistance;

    private PlayerController playerController; 


    private void Start()
    {
        playerController = Player.Instance.GetComponent<PlayerController>();

        rotX = transform.localRotation.eulerAngles.x;
        rotY =  transform.localRotation.eulerAngles.y;

        dirNormalized = _realCam.localPosition.normalized; // 카메라 이동 방향 정규화
        finalDistance = _realCam.localPosition.magnitude; // 카메라 최종 거리 설정

        // 마우스 커서 설정
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        
    }

    private void Update()
    {
        //  마우스 입력값을 받아 카메라 시점을 조절

        rotX += -(Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }

    private void LateUpdate()
    {
        Vector3 input = playerController.inputDirection;
        Vector3 followPos = _objectTofollow.position;
        followPos += input.z < 0 ? -_objectTofollow.right : _objectTofollow.right;

        transform.position = Vector3.MoveTowards(transform.position, followPos, followSpeed * Time.deltaTime);
        finalDir = transform.TransformPoint(dirNormalized * maxDistance);

        // 장애물 감지 후 최종 거리 설정
        RaycastHit hit;
        if(Physics.Linecast(transform.position, finalDir, out hit))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            finalDistance = maxDistance;
        }
        _realCam.localPosition = Vector3.Lerp(_realCam.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);
         
    }
   
}
