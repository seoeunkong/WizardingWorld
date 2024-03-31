using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [Header("�κ��丮 ����")]
    public GameObject inventoryContents; //�κ��丮 UI 
    private int _slotCount;

    private ItemSlotUI[] _itemSlotUIs; //�κ��丮 ���� UI 

    [SerializeField]
    private BaseObject[] _baseObjects;
    private int _weaponIndex;

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
        //if (weaponObject == null) return;

        //GameObject weapon = Instantiate(weaponObject);
        //Player.Instance.weaponManager.RegisterWeapon(weapon);
        //Player.Instance.weaponManager.SetWeapon(weapon);

        _itemSlotUIs = inventoryContents.GetComponentsInChildren<ItemSlotUI>();

        _slotCount = _itemSlotUIs.Length;

        _baseObjects = new BaseObject[_slotCount];

    }

    bool isWeaponCol(int index) => index % 6 == 5;

    bool HasItem(int index)
    {
        return index >= 0 && _itemSlotUIs[index] != null && _baseObjects[index] != null;
    }

    public bool isValidSwap(int from, int to)
    {
        if (!isWeaponCol(from) && !isWeaponCol(to)) return true;
        if (HasItem(from) && HasItem(to)) return isWeaponObj(from) && isWeaponObj(to);
        return isWeaponObj(from) || isWeaponObj(to);
    }

    public bool isCountableObj(int index)
    {
        return HasItem(index) && _baseObjects[index] is CountableObject;
    }

    public bool isWeaponObj(int index)
    {
        return HasItem(index) && _baseObjects[index] is BaseWeapon;
    }

    void InitWeaponInstance(BaseObject baseObject)
    {
        if(baseObject == null)
        {
            Player.Instance.weaponManager.SetWeapon(null);
            return;
        }

        GameObject weapon = Instantiate(baseObject.gameObject);

        Collider collider = weapon.GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        DropItem item = weapon.GetComponent<DropItem>();
        if (item != null) item.enabled = false;

        Rigidbody rb = weapon.GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb);

        Player.Instance.weaponManager.RegisterWeapon(weapon);
        Player.Instance.weaponManager.SetWeapon(weapon);
    }

    void CheckWeaponCol()
    {
        if(_weaponIndex == 0)
        {
            for (int i = 5; i < _baseObjects.Length; i += 6)
            {
                if (HasItem(i))
                {
                    _weaponIndex = i;
                    InitWeaponInstance(_baseObjects[i]);
                    return;
                }
            }
        }
        else
        {
            for (int i = 5; i < _baseObjects.Length; i += 6)
            {
                if (HasItem(i))
                {
                    return;
                }
            }
            _weaponIndex = 0;
            InitWeaponInstance(null);
        }
        


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

                if (index == -1) return;

                // ���ο� ������ ����
                 _baseObjects[index] = baseObject;

                CountableObject co = _baseObjects[index].gameObject.GetComponent<CountableObject>(); ;
                co.SetAmount(1);

            }
            else //�̹� �ش� �������� �κ��丮 ���� �����ϴ� ���
            {
                CountableObject co = _baseObjects[index].gameObject.GetComponent<CountableObject>(); ;
                int curamount = co.Amount;
                co.SetAmount(curamount + amount);
            }
        }
        else
        {
            index = FindEmptySlot();
            if (index >= 0 && baseObject != null) _baseObjects[index] = baseObject;
        }

        
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
            if (!slot.HasItem && !isWeaponCol(slot.Index))
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
            RemoveIcon();
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

        CheckWeaponCol();

    }


    public void Swap(int indexA, int indexB)
    {
        BaseObject itemA = _baseObjects[indexA];
        BaseObject itemB = _baseObjects[indexB];

        // 1. �� �� �ִ� �������̰�, ������ �������� ���
        //    indexA -> indexB�� ���� ��ġ��
        if (itemA != null && itemB != null &&
            itemA._objData.ID == itemB._objData.ID &&
            itemA is CountableObject ciA && itemB is CountableObject ciB)
        {
            int maxAmount = ciB.MaxAmount;
            int sum = ciA.Amount + ciB.Amount;

            if (sum <= maxAmount)
            {
                ciA.SetAmount(0);
                ciB.SetAmount(sum);
            }
            else
            {
                ciA.SetAmount(sum - maxAmount);
                ciB.SetAmount(maxAmount);
            }
        }
        // 2. �Ϲ����� ��� : ���� ��ü
        else
        {
            _baseObjects[indexA] = itemB;
            _baseObjects[indexB] = itemA;
        }

        // �� ���� ���� ����
        UpdateSlot(indexA);
        UpdateSlot(indexB);
        
    }


}
