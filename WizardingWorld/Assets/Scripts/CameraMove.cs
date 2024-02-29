using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.SceneView;
using static UnityEngine.GraphicsBuffer;

public class CameraMove : MonoBehaviour
{
    public Transform _objectTofollow; //목표물 
    public float followSpeed = 10f;
    public float followjumpSpeed = 6f;
    public float sensitivity = 100f;
    public float clampAngle = 70f; //시야 제한 각도 

    //마우스 입력
    private float rotX;
    private float rotY;

    public Transform _realCam;
    public Vector3 dirNormalized;
    public Vector3 finalDir;
    public float minDistance;
    public float maxDistance;
    public float finalDistance;
    public float smoothness = 10f;

    public GameObject _Player;
    PlayerMove _playerMove;


    private void Start()
    {
        rotX = transform.localRotation.eulerAngles.x;
        rotY =  transform.localRotation.eulerAngles.y;

        dirNormalized = _realCam.localPosition.normalized; //방향 
        finalDistance = _realCam.localPosition.magnitude;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        _playerMove = _Player.GetComponent<PlayerMove>();
        
    }

    private void Update()
    {
        rotX += -(Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, _objectTofollow.position, followSpeed * Time.deltaTime);
        finalDir = transform.TransformPoint(dirNormalized * maxDistance);

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
