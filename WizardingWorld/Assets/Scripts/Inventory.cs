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

    [Header("인벤토리 설정")]
    public GameObject inventoryContents; //인벤토리 UI 
    public GameObject monsterContents; //몬스터 인벤토리 UI 
    private int _slotCount;
    private int _monsterCount;

    private ItemSlotUI[] _itemSlotUIs; //인벤토리 슬롯 UI 

    private GameObject _sphereObject;

    [SerializeField] private BaseObject[] _baseObjects;
    private int _weaponIndex;
    private int _monsterIndex = -1;

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
        ItemSlotUI[] items = inventoryContents.GetComponentsInChildren<ItemSlotUI>();
        ItemSlotUI[] mons = monsterContents.GetComponentsInChildren<ItemSlotUI>();

        _slotCount = items.Length;
        _monsterCount = mons.Length;
        _itemSlotUIs = new ItemSlotUI[_slotCount + _monsterCount];

        for (int i = 0; i < _itemSlotUIs.Length; i++)
        {
            if (i < _slotCount) _itemSlotUIs[i] = items[i];
            else _itemSlotUIs[i] = mons[i - _slotCount];
        }
        _baseObjects = new BaseObject[_slotCount + _monsterCount];
    }

    bool isWeaponCol(int index) => index % 6 == 5;
    bool isMonsterInventory(int index) => index >= _slotCount;

    bool HasItem(int index)
    {
        return index >= 0 && _itemSlotUIs[index] != null && _baseObjects[index] != null;
    }

    public bool isValidSwap(int from, int to)
    {
        if (isMonsterInventory(from) && isMonsterInventory(to)) return true;
        if (isMonsterInventory(from) || isMonsterInventory(to)) return false;
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

    public BaseObject GetCurrentPal() => HasItem(_monsterIndex)? _baseObjects[_monsterIndex] : null;

    void CreateInstance(BaseObject baseObject)
    {
        if (baseObject == null) return;

        if (baseObject is BaseWeapon bs)
        {
            Player.Instance.weaponManager.RegisterWeapon(bs);
            Player.Instance.weaponManager.SetWeapon(bs);
        }
        else if (baseObject is PalSphere ps)
        {
            ps.SetPalSphere();
        }
    }

    public void SetSphere(bool toCaptureMonster)
    {
        if(toCaptureMonster) //스피어 투척을 위해 
        {
            int index = FindItemOfType<SphereData>();
            if (index == -1) return;

            BaseObject sphere = _baseObjects[index];
            if(_sphereObject == null)
            {
                _sphereObject = Instantiate(sphere.gameObject);
                _sphereObject.GetComponent<PalSphere>().ChangeSphereMode();
            }

            if (sphere is PalSphere ps)
            {
                //플레이어가 무기를 들고 있는 경우 -> 스피어로 교체 
                if (Player.Instance.weaponManager.hasWeapon()) Player.Instance.weaponManager.UnSetWeapon();
                CreateInstance(_baseObjects[index]);
            }
        }
        else // 펠 투척을 위해
        {
            BaseObject sphere = _sphereObject.GetComponent<BaseObject>();
            if(sphere != null) CreateInstance(sphere);
        }
    }

    //Weapon 열에 무기가 있는 칸 번호 반환
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

    //팰로 등록시킨 몬스터 칸 번호 반환
    List<int> CheckPal()
    {
        List<int> list = new List<int>();

        for (int i = _baseObjects.Length - 5; i < _baseObjects.Length; i++)
        {
            if (HasItem(i)) list.Add(i);
        }
        return list;
    }

    void SetWeapon()
    {
        List<int> weaponCol = CheckWeapon();

        //무기 인벤토리에 무기가 없는 경우 
        if (weaponCol.Count == 0)
        {
            _weaponIndex = 0;
            //CreateInstance(null);
            Player.Instance.weaponManager.UnSetWeapon();
        }
        else
        {   //플레이어 무기가 무기 인벤토리 내에 존재하지 않는 경우 
            if (!weaponCol.Contains(_weaponIndex))
            {
                _weaponIndex = weaponCol[0];
            }

            //플레이어가 스피어를 들고 있는 경우 -> 무기로 교체 
            if (Player.Instance.hasSphere()) Player.Instance.UnSetSphere();

            CreateInstance(_baseObjects[_weaponIndex]);
        }
    }

    void SetPal()
    {
        List<int> pals = CheckPal();
        if (pals.Count == 0) 
        {
            if(_monsterIndex != -1) //기존 팰을 인벤토리에 다시 넣는 경우 
            {
                Player.Instance.UnSetPal = true;
            }
            _monsterIndex = -1;
        }
        else
        {
            if(!pals.Contains(_monsterIndex))
            {
                if(_monsterIndex != -1) Player.Instance.UnSetPal = true; //기존 팰을 인벤토리에 다시 넣는 경우 
                _monsterIndex = pals[0];  //팰 인덱스 업데이트 
            }
        }

        //팰 이미지 새롭게 등록 
        ObjectData ob = GetObjData(_monsterIndex);
        UIManager uIManager = GetComponent<UIManager>();

        if(ob == null) //등록한 팰이 없다면
        {
            uIManager.SetPalImg(null);
        } 
        else
        {
            uIManager.SetPalImg(ob.IconSprite);
            Player.Instance.currentPal = _baseObjects[_monsterIndex] as Monster;
        }
    }

    public BaseObject SetNextPal(int dir)
    {
        int index = _monsterIndex + dir;
        if (index < _itemSlotUIs.Length - 5) index = _itemSlotUIs.Length - 1;
        else if (index >= _itemSlotUIs.Length) index = _itemSlotUIs.Length - 5;

        return HasItem(index)? _baseObjects[index] : null;
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

                if (index == -1) return;

                // 새로운 아이템 생성
                _baseObjects[index] = baseObject;

                CountableObject co = _baseObjects[index].gameObject.GetComponent<CountableObject>(); ;
                co.SetAmount(1);

            }
            else //이미 해당 아이템이 인벤토리 내에 존재하는 경우
            {
                CountableObject co = _baseObjects[index].gameObject.GetComponent<CountableObject>(); ;
                int curamount = co.Amount;
                co.SetAmount(curamount + amount);
            }
        }
        else
        {
            index = FindEmptySlot();
            if(objData as MonsterData) index = FindEmptyMonsterSlot();

            if (index >= 0 && baseObject != null) _baseObjects[index] = baseObject;
        }


        UpdateSlot(index);
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

    //Weapon 열에 있는 다음 무기 번호 반환 
    BaseObject GetNextWeapon(int dir)
    {
        List<int> list = CheckWeapon();

        if (list.Count == 0) return null;

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

    //비어있는 슬롯 중 첫번째 슬롯 찾기
    int FindEmptySlot()
    {
        for (int i = 0; i < _slotCount; i++)
        {
            ItemSlotUI slot = _itemSlotUIs[i];
            if (!slot.HasItem && !isWeaponCol(slot.Index))
            {
                return slot.Index;
            }
        }
        return -1;
    }

    int FindEmptyMonsterSlot()
    {
        for (int i = _slotCount; i < _itemSlotUIs.Length - 5; i++)
        {
            ItemSlotUI slot = _itemSlotUIs[i];
            if (!slot.HasItem)
            {
                return slot.Index;
            }
        }
        return -1;
    }

    //인벤토리에 특정 아이템이 저장되어 있는지 검사 
    public int FindItem(ObjectData objectData)
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

    //인벤토리에 아이템들이 저장되어 있는지 유무 검사 
    public int FindItemOfType<T>() where T : ObjectData
    {
        foreach (var slot in _itemSlotUIs)
        {
            if (slot.HasItem)
            {
                int index = slot.Index;
                if (_baseObjects[index]._objData as T)
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

        SetWeapon();
        SetPal();
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
