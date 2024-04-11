using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Portion_", menuName = "Scriptable Object/Inventory System/Portion", order = 3)]
public class PortionData : CountableData
{
    // ȿ����(ȸ���� ��)
    public float Value => _value;
    [SerializeField] private float _value;
}
