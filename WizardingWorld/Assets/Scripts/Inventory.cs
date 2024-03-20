using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    private Inventory instance;

    public GameObject weaponObject;

    public GameObject inventoryContents;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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

        //테스트 코드 
        ItemSlotUI[] itemSlots = inventoryContents.GetComponentsInChildren<ItemSlotUI>();
        Sprite weaponSprite = weapon.GetComponent<BaseWeapon>().handleData.IconSprite;
        itemSlots[0].SetItem(weaponSprite);
    }

    void AddItem()
    {
        
    }


}
