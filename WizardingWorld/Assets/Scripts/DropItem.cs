using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private Material _OutlineMat;
    private Renderer _renderer;
    //private Material _material;
    private Material[] _materials;
    private int _length;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        //_material = _renderer.material;

        _materials = _renderer.materials;
        _length = _renderer.materials.Length;
    }


    public void AddOutlineMat(bool detectDropItem)
    {
        var materials = _renderer.materials;

        if (detectDropItem)
        {
            //if (materials.Length == _length) //아웃라인이 추가되지 않았다면 
            //{
            //    Material[] mat = new Material[_length + 1];
            //    for(int i = 0; i < _length; i++)
            //    {
            //        mat[i] = materials[i];
            //    }
            //    mat[_length] = _OutlineMat;

            //    _renderer.materials = mat;
            //}


            Material[] mat = new Material[_length];
            for (int i = 0; i < _length; i++)
             {
                mat[i] = _OutlineMat;
             }
            _renderer.materials = mat;
        }
        else
        {
            //if(materials.Length > _length)  //아웃라인이 추가되었다면 
            //{
            //    _renderer.materials = _materials;
            //}

            _renderer.materials = _materials;
        }
    }

}
