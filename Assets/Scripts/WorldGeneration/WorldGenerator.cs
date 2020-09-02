﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public static WorldGenerator instance { get; protected set; }
    public static SaveManager saveManager;

    public World world { get; protected set; }

    public Sprite fullGrassSprite, dirtSprite, stoneSprite, woodBoardsSprite, devTex;

    public World GetWorldInstance()
    {
        return world;
    }

    void Start()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogError("There Should NOT be more than one WorldGenerator");
        }
        instance = this;

        if (saveManager != null) { UnityEngine.Debug.LogError("There Should NOT be more than one SaveManager"); }
        saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();

        if (GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().newWorld) { CreateNewWorld(); }
        else 
        {
            LoadSavedWorld(GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName);
        }
    }

    public void CreateNewWorld()
    {
        UnityEngine.Debug.Log("Creating World");

        // Initialize World
        world = new World(270, 90);

        // Create GameObjects (Visual Layer) For Each Tile in World (Data Layer)
        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                GameObject tile = new GameObject();
                Tile tileData = world.GetTileAt(x, y);

                tile.name = "Tile." + x + "_" + y;
                tile.layer = 9;
                tile.transform.position = new Vector3(tileData.tileX * 2, tileData.tileY * 2, 0);
                tile.transform.SetParent(this.transform, true);

                tile.AddComponent<SpriteRenderer>();
                BoxCollider2D tile_Collider = tile.AddComponent<BoxCollider2D>();
                tile_Collider.size = new Vector3(2, 2);
                tile_Collider.enabled = false;

                tileData.SetTileTypeChangedCallback((Tile _tile) => { OnTileTypeChanged(_tile, tile); }); // this spooky syntax is a lambda, basically a void function with no name, with input of _tile, that runs the function OnTileTypeChanged
            }
        }
        world.GenerateRandomTiles(); // Set Each Tile to Random Tile Type

        // Make Sure New World is Saved As New and doesn't Overwrite Old World
        saveManager.GetSaveFiles();
        foreach (string s in saveManager.saves)
        {
            string saveFileName = Path.GetFileName(s);
            string saveName = saveFileName.Substring(saveFileName.IndexOf("_") + 1);
            int index = saveName.LastIndexOf(".");
            if (index > 0) { saveName = saveName.Substring(0, index); }

            if (GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName == saveName)
            {
                UnityEngine.Debug.Log("World " + saveName + " was found when trying to create new world, setting new world name");
                GameObject.Find(GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName += UnityEngine.Random.Range(0, 100000).ToString());
            }
        }
        world.SaveTiles(GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName);
    }

    public void LoadSavedWorld(string saveName)
    {
        world = new World(270, 90);

        world.LoadTiles(saveName);

        // LOAD INVENTORY DATA -- CURRENTLY I AM HAVING TROUBLE LOADING GAMEOBJECT REFERENCES THROUGH SERIALIZED DATA
        //Inventory inventory = GameObject.Find("Player").GetComponent<Inventory>();
        //inventory.LoadInventory(saveName);

        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                GameObject tile = new GameObject();
                Tile tileData = world.GetTileAt(x, y);;

                tile.name = "Tile." + x + "_" + y;
                tile.layer = 9;
                tile.transform.position = new Vector3(tileData.tileX * 2, tileData.tileY * 2, 0);
                tile.transform.SetParent(this.transform, true);

                tile.AddComponent<SpriteRenderer>();
                BoxCollider2D tile_Collider = tile.AddComponent<BoxCollider2D>();
                tile_Collider.size = new Vector3(2, 2);
                tile_Collider.enabled = false;

                tileData.SetTileTypeChangedCallback((_tile) => { OnTileTypeChanged(_tile, tile); });

                OnTileTypeChanged(tileData, tile); // CALL CALLBACK DIRECTLY BECAUSE BUGS AND THINGS
            }
        }
    }

    public void OnTileTypeChanged(Tile tileData, GameObject tile) // Callback for when Tile Changes so Tile Visuals are updated when Tile Data is updated
    {

        if (tileData.Type == Tile.TileType.Grass)
        {
            tile.GetComponent<SpriteRenderer>().sprite = fullGrassSprite;

            tile.GetComponent<BoxCollider2D>().enabled = true;
            tile.layer = 8;
        }
        else if (tileData.Type == Tile.TileType.Air)
        {
            tile.GetComponent<SpriteRenderer>().sprite = null;
            tile.GetComponent<BoxCollider2D>().enabled = false;
            tile.layer = 9;
        }
        else if(tileData.Type == Tile.TileType.DevTile)
        {
            tile.GetComponent<SpriteRenderer>().sprite = devTex;
            tile.GetComponent<BoxCollider2D>().enabled = true;
            tile.layer = 8;
        }
        else if(tileData.Type == Tile.TileType.Dirt)
        {
            tile.GetComponent<SpriteRenderer>().sprite = dirtSprite;
            tile.GetComponent<BoxCollider2D>().enabled = true;
            tile.layer = 8;
        }
        else if (tileData.Type == Tile.TileType.Stone)
        {
            tile.GetComponent<SpriteRenderer>().sprite = stoneSprite;
            tile.GetComponent<BoxCollider2D>().enabled = true;
            tile.layer = 8;
        }
        else if(tileData.Type == Tile.TileType.Wood_Boards)
        {
            tile.GetComponent<SpriteRenderer>().sprite = woodBoardsSprite;
            tile.GetComponent<BoxCollider2D>().enabled = true;
            tile.layer = 8;
        }
        else
        {
            UnityEngine.Debug.LogWarning("Unrecognized Tile Type, Can't Set Tile Data");
        }

    }

    public Tile GetTileAtWorldCoord(Vector3 coord)
    {
            int x = (int)Mathf.Round(coord.x / 2);
            int y = (int)Mathf.Round(coord.y / 2);

            return world.GetTileAt(x, y);
    }
}
