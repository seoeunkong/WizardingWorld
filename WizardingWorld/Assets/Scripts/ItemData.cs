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
    [SerializeField] private string _name;    // 아이템 이름
    [Multiline]
    [SerializeField] private string _tooltip; // 아이템 설명
    [SerializeField] private Sprite _iconSprite; // 아이템 아이콘
    //[SerializeField] private GameObject _dropItemPrefab; // 바닥에 떨어질 때 생성할 프리팹
    [SerializeField] private int _maxAmount = 99;
}
