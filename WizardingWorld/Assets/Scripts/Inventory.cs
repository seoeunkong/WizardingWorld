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

    [Header("인벤토리 설정")]
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
            if (index == -1) //아이템이 인벤토리 내에 없는 경우
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

    //비어있는 슬롯 중 첫번째 슬롯 찾기
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
            _itemSlotUIs[index].RemoveItem();
            return;
        }

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
    }


}
