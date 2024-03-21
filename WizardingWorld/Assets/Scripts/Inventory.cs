using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public GameObject weaponObject; //test

    [Header("�κ��丮 ����")]
    public GameObject inventoryContents;
    private int _slotCount;

    private ItemSlotUI[] _itemSlotUIs;

    [SerializeField]
    private BaseObject[] _baseObjects;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }  
        DestroyImmediate(gameObject);
        
    }

    void Start()
    {
        Init();
    }

    private void Init()
    {
        if (weaponObject == null) return;

        GameObject weapon = Instantiate(weaponObject);
        Player.Instance.weaponManager.RegisterWeapon(weapon);
        Player.Instance.weaponManager.SetWeapon(weapon);

        _itemSlotUIs = inventoryContents.GetComponentsInChildren<ItemSlotUI>();

        _slotCount = _itemSlotUIs.Length;

        _baseObjects = new BaseObject[_slotCount];
       
    }

    public void AddItemToInv(GameObject item)
    {
        int index = FindItem(item);
        if(index == -1) index = FindEmptySlot(); //�������� �κ��丮 ���� ���� ��� 

        AddItemSlot(item.GetComponent<BaseObject>(), index);
       
    }

    void AddItemSlot(BaseObject item, int index)
    {
        if (index == -1) return; //����ִ� ������ ���� ���

        _baseObjects[index] = item;
        item.SetAmount(item.Amount+1);

        Sprite weaponSprite = item._itemData.IconSprite;
        _itemSlotUIs[index].SetItem(weaponSprite);
        _itemSlotUIs[index].SetItemAmount(item.Amount);
    }


    //����ִ� ���� �� ù��° ���� ã��
    int FindEmptySlot()
    {
        foreach (var slot in _itemSlotUIs)
        {
            if(!slot.HasItem)
            {
                return slot.Index;
            }
        }
        return -1;
    }

    //�κ��丮�� ����Ǿ� �ִ��� �˻� 
    int FindItem(GameObject item)
    {
        foreach (var slot in _itemSlotUIs)
        {
            if (slot.HasItem)
            {
                int index = slot.Index;
                if (_baseObjects[index]._itemData.ID == item.GetComponent<BaseObject>()._itemData.ID)
                {
                    return index;
                }
            }
        }
        return -1;
    }


}
