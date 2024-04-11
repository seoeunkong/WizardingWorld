using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalSphere : CountableObject
{
    public SphereData sphereData { get { return _sphereData; } }

    [Header("팰 스피어 정보"), Tooltip("해당 무기를 쥐었을 때의 local Transform 값 정보 등")]
    [SerializeField] protected SphereData _sphereData;

    private void Awake()
    {
        InitializeData(sphereData);
    }

    public override void InitializeData(ObjectData objData)
    {
        base.InitializeData(objData);
        _countableData = sphereData;
    }

}
