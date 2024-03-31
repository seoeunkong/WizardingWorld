using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// �Һ� ������ ���� 
[CreateAssetMenu(fileName = "Item_Portion_", menuName = "Scriptable Object/Inventory System/Portion", order = 3)]
public class PortionItemData : CountableData
{
    // ȿ����(ȸ���� ��) 
    public float Value => _value;
    [SerializeField] private float _value;
  
}
