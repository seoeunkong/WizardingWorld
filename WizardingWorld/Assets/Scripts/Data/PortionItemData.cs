using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// 소비 아이템 정보 
[CreateAssetMenu(fileName = "Item_Portion_", menuName = "Scriptable Object/Inventory System/Portion", order = 3)]
public class PortionItemData : CountableData
{
    // 효과량(회복량 등) 
    public float Value => _value;
    [SerializeField] private float _value;
  
}
