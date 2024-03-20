using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private Material _OutlineMat;

    private Material _material;
    public BaseObject _Object { get; private set; }

    void Start()
    {
        _material = this.GetComponent<Renderer>().material;
        _Object = GetComponent<BaseObject>();
    }

    public void AddOutlineMat(bool detectDropItem)
    {
        if(detectDropItem) 
        {
            this.GetComponent<Renderer>().materials = new Material[2] { _material, _OutlineMat };
        }
        else this.GetComponent<Renderer>().materials = new Material[1] { _material};
    }
}
