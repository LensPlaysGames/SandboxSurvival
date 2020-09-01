using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Slot
{
    public GameObject slotParent;
    public GameObject countText;

    public Tile.TileType tileType;

    public bool empty;

    public int count;
    public Sprite sprite;
}
