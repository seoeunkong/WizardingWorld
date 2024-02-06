using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class CameraMove : MonoBehaviour
{
    public GameObject Target;               // 카메라가 따라다닐 타겟

    public float offsetY;
    public float offsetZ;          // 카메라의 z좌표

    public float CameraSpeed ;       // 카메라의 속도
    Vector3 TargetPos;                      // 타겟의 위치

    private void Start()
    {
       
    }

    private void LateUpdate()
    {
        // 타겟의 x, y, z 좌표에 카메라의 좌표를 더하여 카메라의 위치를 결정
        //TargetPos = new Vector3(
        //    Target.transform.position.x + offsetX,
        //    Target.transform.position.y + offsetY,
        //    Target.transform.position.z + offsetZ
        //    );


        //Vector3 targetPos = new Vector3(Target.transform.position.x, Target.transform.position.y + offsetY, Target.transform.position.z + offsetZ);
        //transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * CameraSpeed);
        

    }

    public void Rotate(Vector3 Pos)
    {
        // transform.LookAt(Pos, Vector3.up);

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
        Vector3 targetDirection = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 0.3f);
        transform.rotation = newRotation;

    }
}
