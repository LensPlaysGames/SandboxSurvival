﻿using UnityEngine;

[System.Serializable]
public class Item
{
    public enum ItemType
    {
        Tile,
        Tool,
        Weapon,
    };

    public ItemType itemType;

    public Tile.TileType tileType;
}