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

    void CreateInstance(BaseObject baseObject)
    {
        if (baseObject == null)
        {
            Player.Instance.weaponManager.UnSetWeapon();
            return;
        }


        //GameObject weapon = baseObject.gameObject;

        //Collider collider = weapon.GetComponent<Collider>();
        //if (collider != null) collider.enabled = false;

        //DropItem item = weapon.GetComponent<DropItem>();
        //if (item != null) item.enabled = false;

        //Rigidbody rb = weapon.GetComponent<Rigidbody>();
        //if (rb != null)
        //{
        //    rb.useGravity = false;
        //    rb.constraints = RigidbodyConstraints.FreezeAll;
        //}

        if(baseObject is BaseWeapon bs)
        {
            Player.Instance.weaponManager.RegisterWeapon(bs);
            Player.Instance.weaponManager.SetWeapon(bs);
        }
        else if(baseObject is PalSphere ps)
        {
            //Player.Instance.sphereManager.SetPalSphere(weapon);
            ps.SetPalSphere();
        }
    }

    public void SetSphere()
    {
        int index = FindSphere();
        if (index == -1) return;

        BaseObject sphere = _baseObjects[index];
        if(sphere is PalSphere ps)
        {
            int amount = ps.Amount;
            ps.SetAmount(amount-1);
            CreateInstance(_baseObjects[index]);
        }

        UpdateSlot(index);
    }

    //Weapon ���� ���Ⱑ �ִ� ĭ ��ȣ ��ȯ
    List<int> CheckWeapon()
    {
        List<int> list = new List<int>();

        for (int i = 0; i < _baseObjects.Length; i++)
        {
            if (isWeaponCol(i) && HasItem(i))
            {
                list.Add(i);
            }
        }
        return list;
    }

    void SetWeapon()
    {
        List<int> weaponCol = CheckWeapon();

        //���� �κ��丮�� ���Ⱑ ���� ��� 
        if (weaponCol.Count == 0)
        {
            _weaponIndex = 0;
            CreateInstance(null);
        }
        else
        {   //�÷��̾� ���Ⱑ ���� �κ��丮 ���� �������� �ʴ� ��� 
            if (!weaponCol.Contains(_weaponIndex))
            {
                _weaponIndex = weaponCol[0];
            }
            CreateInstance(_baseObjects[_weaponIndex]);
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

    //Weapon ���� �ִ� ���� ���� ��ȣ ��ȯ 
    BaseObject GetNextWeapon(int dir)
    {
        List<int> list = CheckWeapon();

        if (list.Count <= 1) return null;
       
        int index = list.FindIndex(x => x == _weaponIndex);
        if (index + dir < 0) index = list.Count - 1;
        else if (index + dir > list.Count - 1) index = 0;
        else index += dir;

        _weaponIndex = list[index];

        return _baseObjects[_weaponIndex];
    }

    public void SetNextWeapon(int dir)
    {
        BaseObject nextweapon = GetNextWeapon(dir);
        if (nextweapon == null) return;

        CreateInstance(nextweapon);
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

    //�κ��丮�� ã���� �ϴ� �������� ����Ǿ� �ִ��� �˻� 
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

    //�κ��丮�� ���Ǿ ����Ǿ� �ִ��� �˻� 
    int FindSphere()
    {
        foreach (var slot in _itemSlotUIs)
        {
            if (slot.HasItem)
            {
                int index = slot.Index;
                if (_baseObjects[index]._objData as SphereData)
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

        SetWeapon();
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
