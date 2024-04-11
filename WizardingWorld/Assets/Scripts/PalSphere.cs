using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalSphere : CountableObject
{
    public SphereData sphereData { get { return _sphereData; } }

    [Header("�� ���Ǿ� ����"), Tooltip("�ش� ���⸦ ����� ���� local Transform �� ���� ��")]
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
