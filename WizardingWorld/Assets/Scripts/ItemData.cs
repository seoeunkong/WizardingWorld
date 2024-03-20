using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemData : ScriptableObject
{
    public int ID { get { return _id; } }
    public string Name { get { return _name; } }
    public int MaxAmount { get { return _maxAmount; } }
    public Sprite IconSprite { get { return _iconSprite; } }

    [SerializeField] private int _id;
    [SerializeField] private string _name;    // ������ �̸�
    [Multiline]
    [SerializeField] private string _tooltip; // ������ ����
    [SerializeField] private Sprite _iconSprite; // ������ ������
    //[SerializeField] private GameObject _dropItemPrefab; // �ٴڿ� ������ �� ������ ������
    [SerializeField] private int _maxAmount = 99;
}
