using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "CountableData", menuName = "Scriptable Object/CountableData", order = int.MaxValue)]
public abstract class CountableData : ObjectData
{
    public int MaxAmount { get { return _maxAmount; } }
    [SerializeField] private int _maxAmount = 99;

}
