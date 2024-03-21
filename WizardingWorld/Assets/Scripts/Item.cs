using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public 
    abstract class Item 
{
    public ItemData Data { get; private set; }
    public Item(ItemData data)
    {
        this.Data = data;
        Amount = 0;
    }

    //���� ������ ����
    public int Amount { get; protected set; }

    //�ϳ��� ������ ���� �� �ִ� �ִ� ����(�⺻ 99)
    public int MaxAmount { get {  return Data.MaxAmount; } }

    //������ ���� á���� ���� 
    public bool IsMax { get { return Amount >= Data.MaxAmount; } }

    //������ ������ ����
    public bool IsEmpty { get { return Amount <= 0; } }

    //���� ����(���� ����) 
    public void SetAmount(int amount)
    {
        Amount = Mathf.Clamp(amount, 0, MaxAmount);
    }
}
