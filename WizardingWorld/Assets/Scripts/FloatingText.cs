using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{

    private float _DestroyTime = 0.3f;
    private Vector3 offset = new Vector3(0,1.88f,0);

    void Start()
    {
        Destroy(gameObject, _DestroyTime);

        transform.localPosition += offset;
    }
}
