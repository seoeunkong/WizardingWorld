using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class BaseObject : MonoBehaviour
{
    public ObjectData _objData {  get; protected set; }

    public abstract void InitializeData(ObjectData itemData);

}
