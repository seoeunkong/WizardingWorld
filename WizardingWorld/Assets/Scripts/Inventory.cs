using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
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
        this.GetComponent<UIManager>().ShowInventory();

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

    bool HasItem(int index)
    {
        return index >= 0 && _itemSlotUIs[index] != null && _baseObjects[index] != null;
    }

    public bool isCountableObj(int index)
    {
        return HasItem(index) && _baseObjects[index] is CountableObject;
    }

    public void Add(BaseObject baseObject, int amount = 1)
    {
        int index = -1;

        ObjectData objData = baseObject._objData;
        if (objData is CountableData cd)
        {
            index = FindItem(objData);
            if (index == -1) //�������� �κ��丮 ���� ���� ���
            {
                index = FindEmptySlot();
            }
        }
        else
        {
            index = FindEmptySlot();

        }

        if(index >= 0 && baseObject != null) _baseObjects[index] = baseObject;
        UpdateSlot(index);
    }

    public void RemoveItem(GameObject item)
    {
        //int index = FindItem(item);
        //if (index == -1) return;

        //BaseObject removeItem = item.GetComponent<BaseObject>();
        //if(!removeItem.IsEmpty)
        //{
        //    UpdateItemSlot(removeItem, index, removeItem.Amount - 1);
        //}
        //else
        //{
        //    _baseObjects[index] = null;
        //    _itemSlotUIs[index].RemoveItem();
        //}
    }

    //�ش� ������ ������ ���� ���� 
    public ObjectData GetObjData(int index)
    {
        if (!HasItem(index)) return null;

        return _baseObjects[index]._objData;
    }

    /// <summary>
    /// �ش� ������ ���� ������ ���� ����
    /// <para/> - �� ���� : -1 ����
    /// <para/> - �� �� �ִ� ������ : 1 ����
    /// </summary>
    public int GetCurrentAmount(int index)
    {
        if (!HasItem(index)) return -1;

        CountableObject countableObject = _baseObjects[index] as CountableObject;
        if (countableObject == null)
            return 1;

        return countableObject.Amount;
    }

    //�ش� ������ ������ �̸� ����
    public string GetObjName(int index)
    {
        if (!HasItem(index)) return "";

        return _baseObjects[index].name;
    }

    //����ִ� ���� �� ù��° ���� ã��
    int FindEmptySlot()
    {
        foreach (var slot in _itemSlotUIs)
        {
            if (!slot.HasItem)
            {
                return slot.Index;
            }
        }
        return -1;
    }

    //�κ��丮�� ����Ǿ� �ִ��� �˻� 
    int FindItem(ObjectData objectData)
    {
        foreach (var slot in _itemSlotUIs)
        {
            if (slot.HasItem)
            {
                int index = slot.Index;
                if (_baseObjects[index]._objData.ID == objectData.ID)
                {
                    return index;
                }
            }
        }
        return -1;
    }


    //�ش��ϴ� �ε����� ���� ���� �� UI ����
    public void UpdateSlot(int index)
    {
        if (index < 0) return;

        BaseObject obj = _baseObjects[index];

        if (obj == null)
        {
            _itemSlotUIs[index].RemoveItem();
            return;
        }

        // ������ ���
        _itemSlotUIs[index].SetItemImg(obj._objData.IconSprite);

        // 1. �� �� �ִ� ������
        if (obj is CountableObject co)
        {
            // 1-1. ������ 0�� ���, ������ ����
            if (co.IsEmpty)
            {
                _baseObjects[index] = null;
                RemoveIcon();
                return;
            }
            // 1-2. ���� �ؽ�Ʈ ǥ��
            else
            {
                _itemSlotUIs[index].SetItemAmountTxt(co.Amount);
            }
        }
        // 2. �� �� ���� �������� ��� ���� �ؽ�Ʈ ����
        else
        {
            _itemSlotUIs[index].HideItemAmountText();
        }

        // ������ �����ϱ�
        void RemoveIcon()
        {
            _itemSlotUIs[index].RemoveItem();
            _itemSlotUIs[index].HideItemAmountText(); // ���� �ؽ�Ʈ �����
        }
    }


}
