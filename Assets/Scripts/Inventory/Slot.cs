using System;
using UnityEngine;

[Serializable]
public class Slot
{
    [NonSerialized]
    public GameObject slotParent;
    [NonSerialized]
    public GameObject countText;
    [NonSerialized]
    public Sprite sprite;

    public bool empty;
    public int count;

    public Item item;

    public string slotParentName;
    public string countTextName;
    public string spriteName;
}
