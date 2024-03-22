using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private Material _OutlineMat;
    private Renderer _renderer;
    private Material _material;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material;
    }


    public void AddOutlineMat(bool detectDropItem)
    {
        var materials = _renderer.materials;

        if (detectDropItem)
        {
            if (materials.Length < 2) //�ƿ������� �߰����� �ʾҴٸ� 
            {
                _renderer.materials = new Material[2] { _material, _OutlineMat };
            }
        }
        else
        {
            if(materials.Length > 1)  //�ƿ������� �߰��Ǿ��ٸ� 
            {
                _renderer.materials = new Material[1] { _material };
            }
        }
    }

}
