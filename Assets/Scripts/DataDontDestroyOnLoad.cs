using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDontDestroyOnLoad : MonoBehaviour
{
    public static DataDontDestroyOnLoad instance;

    public string texturePack = "Textures";

    public Sprite[] spriteDB;
    private bool spriteDBLoaded;

    void Start()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogError("Multiple DataDontDestroyOnLoad In Scene!!! Destroying " + this.name);
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        // Sprite Database Initilization
        if (!spriteDBLoaded)
        {
            spriteDB = new Sprite[Enum.GetNames(typeof(Tile.TileType)).Length];
            for (int tile = 0; tile < Enum.GetNames(typeof(Tile.TileType)).Length; tile++)
            {
                spriteDB[tile] = Resources.Load<Sprite>(texturePack + "/" + Enum.GetName(typeof(Tile.TileType), tile));
            }
            spriteDBLoaded = true;
        }
    }

    public bool newWorld;
    public bool playingMusic;

    public string saveName = ""; // Default World Save name if Player Names Empty World
}
