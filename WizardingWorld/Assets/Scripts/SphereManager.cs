using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereManager
{
    // 스피어를 쥐는 손의 트랜스폼
    private Transform handPosition;

    public SphereManager(Transform hand)
    {
        handPosition = hand;
    }


    // 스피어 손에 장착 
    public void SetPalSphere(GameObject sphere)
    {
        if (sphere == null) return;

        PalSphere sphereInfo = sphere.GetComponent<PalSphere>();
        sphere.transform.SetParent(handPosition);

        sphere.transform.localPosition = sphereInfo.sphereData.localPosition;
        sphere.transform.localEulerAngles = sphereInfo.sphereData.localRotation;
        sphere.transform.localScale = sphereInfo.sphereData.localScale;

        sphere.SetActive(true);
    }

    
}
