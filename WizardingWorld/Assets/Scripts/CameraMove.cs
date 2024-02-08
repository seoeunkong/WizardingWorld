using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.SceneView;
using static UnityEngine.GraphicsBuffer;

public class CameraMove : MonoBehaviour
{
    public Transform targetTr;//Ÿ�ٴ�� 
    Transform camTr;

    public float distance; //Ÿ�� ������ �Ÿ�
    public float height; //���� 
    public float damping; //ī�޶� �����ӵ� 

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
