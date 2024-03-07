using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance {  get; private set; }
    private InventoryManager instance;
    public GameObject weaponObject;

    private void Awake()
    {
        if(instance == null)
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
        if(weaponObject == null)
        {
            
        }
    }
}
