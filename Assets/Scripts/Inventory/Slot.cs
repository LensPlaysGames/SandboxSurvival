using System;
using UnityEngine;
using UnityEngine.UI;

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

    public bool isTile;

    public Tile.TileType tileType;

    public string slotParentName;
    public string countTextName;
    public string spriteName;
}
