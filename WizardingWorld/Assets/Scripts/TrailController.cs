using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailController : MonoBehaviour
{
    private GameObject _body;
    private GameObject _trail;

    public void bodyActiveFalse() => _body.SetActive(false);
    public void bodyActiveTrue() => _body.SetActive(true);
    public void trailActiveTrue() => _trail.SetActive(true);
    public void trailActiveFalse() => _trail.SetActive(false);

    void Start()
    {
        Transform parentTransform = transform; 

        foreach (Transform child in parentTransform)
        {
            if(_body == null) _body = child.gameObject;
            else _trail = child.gameObject;
        }

        _trail.SetActive(false);
    }

    
}
