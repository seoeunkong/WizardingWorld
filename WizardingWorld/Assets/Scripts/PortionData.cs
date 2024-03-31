using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Portion_", menuName = "Scriptable Object/Inventory System/Portion", order = 3)]
public class PortionData : CountableData
{
    // 효과량(회복량 등)
    public float Value => _value;
    [SerializeField] private float _value;
}
