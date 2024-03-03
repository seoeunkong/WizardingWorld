using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.SceneView;
using static UnityEngine.GraphicsBuffer;

public class CameraMove : MonoBehaviour
{
    public Transform _objectTofollow; //��ǥ�� 
    public Transform _realCam; //���� ī�޶� ��ġ 

    [Header("ī�޶� ���� ������ ")]
    public float followSpeed = 10f;
    public float sensitivity = 100f; //���콺 ���� 
    public float clampAngle = 70f; //�þ� ���� ���� 
    public float smoothness = 10f; //ī�޶� �̵� �� ��ȯ ���ǵ� 
    public float minDistance;
    public float maxDistance;

    //���콺 �Է�
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

        dirNormalized = _realCam.localPosition.normalized; // ī�޶� �̵� ���� ����ȭ
        finalDistance = _realCam.localPosition.magnitude; // ī�޶� ���� �Ÿ� ����

        // ���콺 Ŀ�� ����
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        
    }

    private void Update()
    {
        //  ���콺 �Է°��� �޾� ī�޶� ������ ����

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

        // ��ֹ� ���� �� ���� �Ÿ� ����
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
