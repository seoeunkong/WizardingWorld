using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountableObject : BaseObject
{
    public CountableData countableData { get { return _countableData; } }

    [Header("수량이 존재하는 아이템"), Tooltip("개수 속성 정보")]
    [SerializeField] protected CountableData _countableData;

    private void Awake()
    {
        InitializeData(_countableData);
    }

    public override void InitializeData(ObjectData objData)
    {
        _objData = objData;
    }

    //현재 아이템 개수 
    public int Amount { get; protected set; }

    //하나의 슬롯이 가질 수 있는 최대 개수(기본 99)
    public int MaxAmount { get { return _countableData.MaxAmount; } }

    //수량이 가득 찼는지 여부 
    public bool IsMax { get { return Amount >= _countableData.MaxAmount; } }

    //개수가 없는지 여부
    public bool IsEmpty { get { return Amount <= 0; } }

    //개수 지정(범위 제한) 
    public void SetAmount(int amount)
    {
        Amount = Mathf.Clamp(amount, 0, MaxAmount);
    }

    // 개수 추가 및 최대치 초과량 반환(초과량 없을 경우 0)
    public int AddAmountAndGetExcess(int amount)
    {
        int nextAmount = Amount + amount;
        SetAmount(nextAmount);

        return (nextAmount > MaxAmount) ? (nextAmount - MaxAmount) : 0;
    }
}
