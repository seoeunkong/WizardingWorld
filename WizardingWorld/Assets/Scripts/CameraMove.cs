using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.SceneView;
using static UnityEngine.GraphicsBuffer;

public class CameraMove : MonoBehaviour
{
    public Transform objectTofollow; //��ǥ�� 
    public float followSpeed = 10f;
    public float sensitivity = 100f;
    public float clampAngle = 70f; //�þ� ���� ���� 

    //���콺 �Է�
    private float rotX;
    private float rotY;

    public Transform realCam;
    public Vector3 dirNormalized;
    public Vector3 finalDir;
    public float minDistance;
    public float maxDistance;
    public float finalDistance;
    public float smoothness = 10f;


    private void Start()
    {
        rotX = transform.localRotation.eulerAngles.x;
        rotY =  transform.localRotation.eulerAngles.y;

        dirNormalized = realCam.localPosition.normalized; //���� 
        finalDistance = realCam.localPosition.magnitude;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        
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
        transform.position = Vector3.MoveTowards(transform.position, objectTofollow.position, followSpeed * Time.deltaTime);
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
        realCam.localPosition = Vector3.Lerp(realCam.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);
         
    }
   
}
