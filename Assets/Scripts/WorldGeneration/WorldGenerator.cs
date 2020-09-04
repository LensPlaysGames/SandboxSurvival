using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public static WorldGenerator instance { get; protected set; }
    public static SaveManager saveManager;

    public World world { get; protected set; }

    public Sprite fullGrassSprite, dirtSprite, stoneSprite, logSprite, leavesSprite, woodBoardsSprite, devTex;

    public GameObject player;

    public World GetWorldInstance()
    {
        return world;
    }

    public bool worldCreated;

    void Start()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogError("There Should NOT be more than one WorldGenerator");
        }
        instance = this;

        if (saveManager != null) { UnityEngine.Debug.LogError("There Should NOT be more than one SaveManager"); }
        saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();

        if (player != null) { UnityEngine.Debug.LogError("There Should NOT be more than one Player"); }
        player = GameObject.Find("Player");

        if (GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().newWorld) { CreateNewWorld(); }
        else 
        {
            LoadSavedWorld(GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName);
        }
    }

    public void CreateNewWorld()
    {
        worldCreated = false;

        UnityEngine.Debug.Log("Creating World");

        // Get World Generation Characteristics
        WorldGenerationParameters worldGenParams = GameObject.Find("DataDontDestroyOnLoad").GetComponent<WorldGenerationParameters>();

        // Initialize World
        world = new World(worldGenParams.worldWidth, worldGenParams.worldHeight);

        // Get World Scale
        float tileScale = worldGenParams.tileScale;
        world.scale = tileScale;

        // Create GameObjects (Visual Layer) For Each Tile in World (Data Layer)
        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                GameObject tile = new GameObject();
                Tile tileData = world.GetTileAt(x, y);

                tile.name = "Tile." + x + "_" + y;
                tile.layer = 9;
                tile.transform.position = 
                    new Vector3(
                        tileData.tileX * tileScale, 
                        tileData.tileY * tileScale, 
                        0);
                tile.transform.localScale = new Vector3(tileScale, tileScale);
                tile.transform.SetParent(this.transform, true);

                tile.AddComponent<SpriteRenderer>();
                BoxCollider2D tile_Collider = tile.AddComponent<BoxCollider2D>();
                tile_Collider.size = new Vector3(1, 1);
                tile_Collider.enabled = false;

                tileData.SetTileTypeChangedCallback((Tile _tile) => { OnTileTypeChanged(_tile, tile); }); // this spooky syntax is a lambda, basically a void function with no name, with input of _tile, that runs the function OnTileTypeChanged
            }
        }
        world.GenerateRandomTiles(); // Set Each Tile to Random Tile Type

        // Make Sure New World is Saved As New and doesn't Overwrite Old World
        saveManager.GetSaveFiles();
        foreach (string s in saveManager.worldSaves)
        {
            // Get Save Name from Path Name
            string saveFileName = Path.GetFileName(s);
            string saveName = saveFileName.Substring(saveFileName.IndexOf("_") + 1);
            int index = saveName.LastIndexOf(".");
            if (index > 0) { saveName = saveName.Substring(0, index); }

            // 1.) If that save name exists on the disk already, 
            if (GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName == saveName)
            {
                // 2.) Add Random Number Between 0 and 1000000 and append to intended save name
                // This is assuming that the player won't get unlucky AND name new worlds the same everytime... Should probably add exception handling eventually grumble grumble
                UnityEngine.Debug.Log("World " + saveName + " was found when trying to create new world, setting new world name");
                GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName += UnityEngine.Random.Range(0, 1000000).ToString();
            }
        }

        // Save Newly Created World Tiles to Disk
        world.SaveTiles(GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName);

        // Save World Width and Height
        saveManager.SetWorldWidthHeightSaveData(GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName, world.Width, world.Height);

        // Save World Scale
        saveManager.SetWorldScaleSaveData(GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName, tileScale);

        // Save Inventory (once player loads) So that Game Doesn't BAZINGA when loading if player alt-f4s a new world without saving
        StartCoroutine(SavePlayerInventoryAfterX(1f));
        StartCoroutine(SavePlayerDataAfterX(1f));

        worldCreated = true;
    }

    public IEnumerator SavePlayerInventoryAfterX(float x) { yield return new WaitForSeconds(x); player.GetComponent<Inventory>().SaveInventory(GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName); }

    public IEnumerator SavePlayerDataAfterX(float x) { yield return new WaitForSeconds(x); player.GetComponent<Player>().SavePlayerData(GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName); }

    public void LoadSavedWorld(string saveName)
    {
        worldCreated = false;

        // LOAD SAVE FROM DISK
        saveManager.LoadAllDataFromDisk(saveName);

        // LOAD WORLD SIZE
        int worldWidth = saveManager.loadedData.worldWidth;
        int worldHeight = saveManager.loadedData.worldHeight;
        UnityEngine.Debug.Log("Loaded World Size! x: " + worldWidth + " y: " + worldHeight);

        world = new World(worldWidth, worldHeight);

        world.LoadTiles(saveName);

        // LOAD INVENTORY DATA
        GameObject player = GameObject.Find("Player");
        player.GetComponent<Inventory>().LoadInventory(saveName);

        // LOAD WORLD SCALE
        float worldScale = saveManager.loadedData.worldScale;
        world.scale = worldScale;

        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                GameObject tile = new GameObject();
                Tile tileData = world.GetTileAt(x, y);;

                tile.name = "Tile." + x + "_" + y;
                tile.layer = 9;
                tile.transform.position = 
                    new Vector3(
                        tileData.tileX * worldScale, 
                        tileData.tileY * worldScale, 
                        0);
                tile.transform.localScale = new Vector3(worldScale, worldScale);
                tile.transform.SetParent(this.transform, true);

                tile.AddComponent<SpriteRenderer>();
                BoxCollider2D tile_Collider = tile.AddComponent<BoxCollider2D>();
                tile_Collider.size = new Vector3(1, 1);
                tile_Collider.enabled = false;
                tileData.SetTileTypeChangedCallback((_tile) => { OnTileTypeChanged(_tile, tile); });

                OnTileTypeChanged(tileData, tile); // CALL CALLBACK DIRECTLY BECAUSE BUGS AND THINGS
            }
        }

        // LOAD PLAYER DATA (position)
        player.GetComponent<Player>().LoadPlayerData(saveName);

        worldCreated = true;
    }

    public void OnTileTypeChanged(Tile tileData, GameObject tile) // Callback for when Tile Changes so Tile Visuals are updated when Tile Data is updated
    {
        // Layer 9 = Not Solid
        // Layer 8 = Solid

        if (tileData.Type == Tile.TileType.Air)
        {
            tile.GetComponent<SpriteRenderer>().sprite = null;
            tile.GetComponent<BoxCollider2D>().enabled = false;
            tile.layer = 9;
        }
        else if(tileData.Type == Tile.TileType.Grass)
        {
            tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Grass");
            tile.GetComponent<BoxCollider2D>().enabled = true;
            tile.layer = 8;
        }
        else if(tileData.Type == Tile.TileType.Dirt)
        {
            tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Dirt");
            tile.GetComponent<BoxCollider2D>().enabled = true;
            tile.layer = 8;
        }
        else if (tileData.Type == Tile.TileType.Stone)
        {
            tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Stone");
            tile.GetComponent<BoxCollider2D>().enabled = true;
            tile.layer = 8;
        }
        else if (tileData.Type == Tile.TileType.DarkStone)
        {
            tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("DarkStone");
            tile.GetComponent<BoxCollider2D>().enabled = true;
            tile.layer = 8;
        }
        else if (tileData.Type == Tile.TileType.Log)
        {
            tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Log");
            tile.GetComponent<BoxCollider2D>().enabled = true;
            tile.layer = 8;
        }
        else if (tileData.Type == Tile.TileType.Leaves)
        {
            tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Leaves");
            tile.GetComponent<BoxCollider2D>().enabled = false;
            tile.layer = 9;
        }
        else if(tileData.Type == Tile.TileType.WoodBoards)
        {
            tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("WoodBoards");
            tile.GetComponent<BoxCollider2D>().enabled = true;
            tile.layer = 8;
        }
        else if (tileData.Type == Tile.TileType.DevTile)
        {
            tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("DevTex"); ;
            tile.GetComponent<BoxCollider2D>().enabled = true;
            tile.layer = 8;
        }
        else
        {
            UnityEngine.Debug.LogWarning("Unrecognized Tile Type: " + tileData.Type.ToString() + ", Can't Set Tile Data");
        }

    }

    public GameObject GetTileGameObjectAtWorldCoord(Vector3 coord)
    {
        int x = (int)Mathf.Round(coord.x / world.scale);
        int y = (int)Mathf.Round(coord.y / world.scale);

        return GameObject.Find("Tile." + x + "_" + y);
    }

    public Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = (int)Mathf.Round(coord.x / world.scale);
        int y = (int)Mathf.Round(coord.y / world.scale);

        return world.GetTileAt(x, y);
    }
}
