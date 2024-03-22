using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class BaseObject : MonoBehaviour
{
    public ObjectData _objData {  get; protected set; }

    public abstract void InitializeData(ObjectData itemData);

    //���� ������ ����
    //public int Amount { get; protected set; }

    ////�ϳ��� ������ ���� �� �ִ� �ִ� ����(�⺻ 99)
    //public int MaxAmount { get { return _itemData.MaxAmount; } }

    ////������ ���� á���� ���� 
    //public bool IsMax { get { return Amount >= _itemData.MaxAmount; } }

    ////������ ������ ����
    //public bool IsEmpty { get { return Amount <= 0; } }

    ////���� ����(���� ����) 
    //public void SetAmount(int amount)
    //{
    //    Amount = Mathf.Clamp(amount, 0, MaxAmount);
    //}
}
