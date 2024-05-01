using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColiderOff : MonoBehaviour
{
    
    void Start()
    {
        SphereCollider[] spheres = transform.GetComponentsInChildren<SphereCollider>();
        MeshCollider[] meshs = transform.GetComponentsInChildren<MeshCollider>();
        BoxCollider[] boxs = transform.GetComponentsInChildren<BoxCollider>();
        CapsuleCollider[] capsules = transform.GetComponentsInChildren<CapsuleCollider>();

        foreach (var c in spheres)
        {
            c.enabled = false;
        }

        foreach (var c in meshs)
        {
            c.enabled = false;
        }

        foreach (var c in boxs)
        {
            c.enabled = false;
        }

        foreach (var c in capsules)
        {
            c.enabled = false;
        }
    }

}
