using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class BaseObject : MonoBehaviour
{
    public ObjectData _objData {  get; protected set; }

    public abstract void InitializeData(ObjectData itemData);

    //현재 아이템 개수
    //public int Amount { get; protected set; }

    ////하나의 슬롯이 가질 수 있는 최대 개수(기본 99)
    //public int MaxAmount { get { return _itemData.MaxAmount; } }

    ////수량이 가득 찼는지 여부 
    //public bool IsMax { get { return Amount >= _itemData.MaxAmount; } }

    ////개수가 없는지 여부
    //public bool IsEmpty { get { return Amount <= 0; } }

    ////개수 지정(범위 제한) 
    //public void SetAmount(int amount)
    //{
    //    Amount = Mathf.Clamp(amount, 0, MaxAmount);
    //}
}
