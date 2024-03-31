using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountableObject : BaseObject
{
    public CountableData countableData { get { return _countableData; } }

    [Header("������ �����ϴ� ������"), Tooltip("���� �Ӽ� ����")]
    [SerializeField] protected CountableData _countableData;

    private void Awake()
    {
        InitializeData(_countableData);
    }

    public override void InitializeData(ObjectData objData)
    {
        _objData = objData;
    }

    //���� ������ ���� 
    public int Amount { get; protected set; }

    //�ϳ��� ������ ���� �� �ִ� �ִ� ����(�⺻ 99)
    public int MaxAmount { get { return _countableData.MaxAmount; } }

    //������ ���� á���� ���� 
    public bool IsMax { get { return Amount >= _countableData.MaxAmount; } }

    //������ ������ ����
    public bool IsEmpty { get { return Amount <= 0; } }

    //���� ����(���� ����) 
    public void SetAmount(int amount)
    {
        Amount = Mathf.Clamp(amount, 0, MaxAmount);
    }

    // ���� �߰� �� �ִ�ġ �ʰ��� ��ȯ(�ʰ��� ���� ��� 0)
    public int AddAmountAndGetExcess(int amount)
    {
        int nextAmount = Amount + amount;
        SetAmount(nextAmount);

        return (nextAmount > MaxAmount) ? (nextAmount - MaxAmount) : 0;
    }
}
