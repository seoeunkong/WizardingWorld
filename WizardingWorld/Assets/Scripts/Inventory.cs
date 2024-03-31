using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.VolumeComponent;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [Header("인벤토리 설정")]
    public GameObject inventoryContents; //인벤토리 UI 
    private int _slotCount;

    private ItemSlotUI[] _itemSlotUIs; //인벤토리 슬롯 UI 

    [SerializeField]
    private BaseObject[] _baseObjects;
    private int _weaponHandIndex;

    private List<BaseObject> _weaponList = new List<BaseObject>();

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

    bool HasItem(int index)
    {
        return index >= 0 && index < _slotCount && _itemSlotUIs[index] != null && _baseObjects[index] != null;
    }

    bool CheckWeaponIndex(int index)
    {
        return index % 6 == 5;
    }

    void InitWeaponInstance(BaseObject baseObject)
    {
        if (baseObject == null) return;

        GameObject weapon = null;

        if (!_weaponList.Contains(baseObject))
        {
            weapon = Instantiate(baseObject.gameObject);
            weapon.GetComponent<Collider>().enabled = false;
            weapon.GetComponent<DropItem>().enabled = false;

            Destroy(weapon.GetComponent<Rigidbody>());
        }

        Player.Instance.weaponManager.RegisterWeapon(weapon);
        Player.Instance.weaponManager.SetWeapon(weapon);
    }

    //Swap 가능한지 체크 
    public bool IsValidSwap(int from, int to)
    {
        if (!CheckWeaponIndex(from) && !CheckWeaponIndex(to))
        {
            return true;
        }

        if (HasItem(from) && HasItem(to))
        {
            return _baseObjects[from] is BaseWeapon && _baseObjects[to] is BaseWeapon;
        }

        if (HasItem(from) || HasItem(to))
        {
            
            int index = HasItem(from) ? from : to;
            return _baseObjects[index] is BaseWeapon;
        }

        return false;
    }

    public bool IsValidNextWeapon()
    {
        int nextWeapon = _weaponHandIndex + 6;
        if(HasItem(nextWeapon))
        {
            _weaponHandIndex = nextWeapon;
            return true;
        }
        return false;
    }

    public bool isCountableObj(int index)
    {
        return HasItem(index) && _baseObjects[index] is CountableObject;
    }

    public bool isWeaponObj(int index)
    {
        return HasItem(index) && _baseObjects[index] is BaseWeapon;
    }

    public void Add(BaseObject baseObject, int amount = 1)
    {
        int index = -1;

        ObjectData objData = baseObject._objData;

        if (objData is CountableData cd) //셀 수 있는 아이템인 경우
        {
            index = FindItem(objData);

            if (index == -1) //아이템이 인벤토리 내에 없는 경우
            {
                index = FindEmptySlot();
                if (index >= 0 && baseObject != null) _baseObjects[index] = baseObject;

                CountableObject co = baseObject.gameObject.GetComponent<CountableObject>(); ;
                co.SetAmount(1);
            }
            else //아이템이 인벤토리 내에 존재하는 경우 
            {
                CountableObject co = _baseObjects[index].gameObject.GetComponent<CountableObject>();
                int current = co.Amount;
                co.SetAmount(current + amount);
            }
        }
        else //무기인 경우
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

    //해당 슬롯의 아이템 정보 리턴 
    public ObjectData GetObjData(int index)
    {
        if (!HasItem(index)) return null;

        return _baseObjects[index]._objData;
    }

    /// <summary>
    /// 해당 슬롯의 현재 아이템 개수 리턴
    /// <para/> - 빈 슬롯 : -1 리턴
    /// <para/> - 셀 수 있는 아이템 : 1 리턴
    /// </summary>
    public int GetCurrentAmount(int index)
    {
        if (!HasItem(index)) return -1;

        CountableObject countableObject = _baseObjects[index] as CountableObject;
        if (countableObject == null)
            return 1;

        return countableObject.Amount;
    }

    //해당 슬롯의 아이템 이름 리턴
    public string GetObjName(int index)
    {
        if (!HasItem(index)) return "";

        return _baseObjects[index].name;
    }

    // Weapon 열에 무기가 세팅된 경우 -> 무기 인덱스 업데이트
    // Weapon 열에 무기가 없는 경우 -> 무기 인덱스 0으로 초기화 
    void CheckWeaponCol()
    {
        if(_weaponHandIndex == 0)
        {
            for(int i = 5; i < _slotCount; i+=6)
            {
                if (HasItem(i))
                {
                    _weaponHandIndex = i;
                    InitWeaponInstance(_baseObjects[_weaponHandIndex]);
                    return;
                }
            }
        }
        else
        {
            for (int i = 5; i < _slotCount; i += 6)
            {
                if (HasItem(i))
                {
                    return;
                }
            }
            _weaponHandIndex = 0;
        }
    }

    //비어있는 슬롯 중 첫번째 슬롯 찾기
    int FindEmptySlot()
    {
        foreach (var slot in _itemSlotUIs)
        {
            if (!slot.HasItem && !CheckWeaponIndex(slot.Index))
            {
                return slot.Index;
            }
        }
        return -1;
    }

    //인벤토리에 저장되어 있는지 검사 
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


    //해당하는 인덱스의 슬롯 상태 및 UI 갱신
    public void UpdateSlot(int index)
    {
        if (index < 0) return;

        BaseObject obj = _baseObjects[index];

        if (obj == null)
        {
            RemoveIcon();
            return;
        }

        //foreach (var slot in _baseObjects)
        //{
        //    Debug.Log(slot);
        //}

        // 아이콘 등록
        _itemSlotUIs[index].SetItemImg(obj._objData.IconSprite);

        // 1. 셀 수 있는 아이템
        if (obj is CountableObject co)
        {
            // 1-1. 수량이 0인 경우, 아이템 제거
            if (co.IsEmpty)
            {
                _baseObjects[index] = null;
                RemoveIcon();
                return;
            }
            // 1-2. 수량 텍스트 표시
            else
            {
                _itemSlotUIs[index].SetItemAmountTxt(co.Amount);
            }
        }
        // 2. 셀 수 없는 아이템인 경우 수량 텍스트 제거
        else
        {
            _itemSlotUIs[index].HideItemAmountText();
        }

        // 아이콘 제거하기
        void RemoveIcon()
        {
            _itemSlotUIs[index].RemoveItem();
            _itemSlotUIs[index].HideItemAmountText(); // 수량 텍스트 숨기기
        }

        CheckWeaponCol();
      
    }


    public void Swap(int indexA, int indexB)
    {
        BaseObject itemA = _baseObjects[indexA];
        BaseObject itemB = _baseObjects[indexB];

        // 1. 셀 수 있는 아이템이고, 동일한 아이템일 경우
        //    indexA -> indexB로 개수 합치기
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
        // 2. 일반적인 경우 : 슬롯 교체
        else
        {
            _baseObjects[indexA] = itemB;
            _baseObjects[indexB] = itemA;
        }

        // 두 슬롯 정보 갱신
        UpdateSlot(indexA);
        UpdateSlot(indexB);
        
    }


}
