using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereManager
{
    // ���Ǿ ��� ���� Ʈ������
    private Transform handPosition;

    public SphereManager(Transform hand)
    {
        handPosition = hand;
    }


    // ���Ǿ� �տ� ���� 
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
