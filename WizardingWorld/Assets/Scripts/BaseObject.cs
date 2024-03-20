using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class BaseObject : MonoBehaviour
{
    public ItemData _itemData {  get; protected set; }

    public abstract void InitializeData(ItemData itemData);
}
