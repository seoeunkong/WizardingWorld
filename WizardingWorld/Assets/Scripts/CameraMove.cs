using System.Collections;
using System.Collections.Generic;
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
    public float minDistance;
    public float maxDistance;

    //���콺 �Է�
    private float _rotX;
    private float _rotY;

    private Vector3 _dirNormalized;
    private Vector3 _finalDir;
    private float _finalDistance;

    private PlayerController _playerController; 


    private void Start()
    {
        _playerController = Player.Instance.GetComponent<PlayerController>();

        _rotX = transform.localRotation.eulerAngles.x;
        _rotY =  transform.localRotation.eulerAngles.y;

        _dirNormalized = realCam.localPosition.normalized; // ī�޶� �̵� ���� ����ȭ
        _finalDistance = realCam.localPosition.magnitude; // ī�޶� ���� �Ÿ� ����

        // ���콺 Ŀ�� ����
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
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
        Vector3 followPos = CalcPos(inputDir);

        transform.position = Vector3.MoveTowards(transform.position, followPos, followSpeed * Time.deltaTime);
        _finalDir = transform.TransformPoint(_dirNormalized * maxDistance);

        // ��ֹ� ���� �� ���� �Ÿ� ����
        RaycastHit hit;
        if(Physics.Linecast(transform.position, _finalDir, out hit))
        {
            _finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            _finalDistance = maxDistance;
        }
        realCam.localPosition = Vector3.Lerp(realCam.localPosition, _dirNormalized * _finalDistance, Time.deltaTime * smoothness);
         
    }

    public Vector3 CalcPos(Vector3 inputDir)
    {
        Vector3 followPos = objectTofollow.position;

        if (inputDir.x != 0)
        {
            followPos = objectTofollow.position - objectTofollow.right;
            followPos += objectTofollow.forward * inputDir.x;
        }

        if (inputDir.z < 0)
        {
            followPos -= objectTofollow.right;
        }
        else if (inputDir.z >= 0)
        {
            followPos += objectTofollow.right;
        }

        return followPos;
    }

}
