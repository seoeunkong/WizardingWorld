using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CountableData", menuName = "Scriptable Object/CountableData", order = int.MaxValue)]
public class CountableData : ObjectData
{
    public int MaxAmount { get { return _maxAmount; } }
    [SerializeField] private int _maxAmount = 99;
}
