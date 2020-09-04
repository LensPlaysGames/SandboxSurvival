using System;
using UnityEngine;

[Serializable]
public class AllData
{
    // World Data
    public Tile[] tiles;
    public int worldWidth, worldHeight;
    public float worldScale;

    // Player Data
    public PlayerSaveData playerData;
    public Slot[] playerInv;
}
