using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.SceneView;
using static UnityEngine.GraphicsBuffer;

public class CameraMove : MonoBehaviour
{
    public Transform targetTr;//타겟대상 
    Transform camTr;

    public float distance; //타겟 대상과의 거리
    public float height; //높이 
    public float damping; //카메라 반응속도 

    private void Start()
    {
       camTr = GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        
 
    }

    private void LateUpdate()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        if (hAxis != 0 || vAxis != 0)
        {
            Vector3 pos = targetTr.position + (-targetTr.forward * distance) + (Vector3.up * height);
            camTr.position = Vector3.Slerp(camTr.position, pos, Time.deltaTime * damping);
            camTr.LookAt(targetTr.position);
        }

         
    }

   
}
