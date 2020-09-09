using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance { get; protected set; }
    public static SaveManager saveManager;

    public Level level { get; protected set; }

    public GameObject player;

    public GameObject loadScreen;

    public Level GetLevelInstance()
    {
        return level;
    }

    public bool worldCreated;

    [SerializeField]
    private Material tileMat;

    void Start()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogError("There Should NOT be more than one WorldGenerator");
        }
        instance = this;
        GameReferences.levelGenerator = instance;

        saveManager = GlobalReferences.saveManager;

        player = GameReferences.player;

        if (GlobalReferences.DDDOL.newWorld) { CreateNewWorld(0); }
        else 
        {
            LoadSavedWorld(GlobalReferences.DDDOL.saveName);
        }
    }

    public void CreateNewWorld(int levelIndex)
    {
        worldCreated = false;

        UnityEngine.Debug.Log("Creating World");

        // Get World Generation Characteristics
        LevelGenerationParameters levelGenParams = GlobalReferences.levelGenParams;

        // Initialize World
        level = new Level(levelGenParams.worldWidth, levelGenParams.worldHeight, levelGenParams.tileScale, levelIndex);

        // Create GameObjects (Visual Layer) For Each Tile in World (Data Layer)
        for (int x = 0; x < level.Width; x++)
        {
            for (int y = 0; y < level.Height; y++)
            {
                GameObject tile = new GameObject();
                Tile tileData = level.GetTileAt(x, y);

                tile.name = "Tile." + x + "_" + y;
                tile.layer = 9;
                tile.transform.position = 
                    new Vector3(
                        tileData.tileX * levelGenParams.tileScale, 
                        tileData.tileY * levelGenParams.tileScale, 
                        0);
                tile.transform.localScale = new Vector3(levelGenParams.tileScale, levelGenParams.tileScale);
                tile.transform.SetParent(this.transform, true);

                SpriteRenderer tile_SpriteRenderer = tile.AddComponent<SpriteRenderer>();
                tile_SpriteRenderer.material = tileMat;
                BoxCollider2D tile_Collider = tile.AddComponent<BoxCollider2D>();
                tile_Collider.size = new Vector3(1, 1);
                tile_Collider.enabled = false;

                tileData.SetTileTypeChangedCallback((Tile _tile) => { OnTileTypeChanged(_tile, tile); }); // this spooky syntax is a lambda, basically a void function with no name, with input of _tile, that runs the function OnTileTypeChanged
            }
        }
        level.GenerateLevelTiles(); // Set Each Tile to Random Tile Type

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
            if (GlobalReferences.DDDOL.saveName == saveName)
            {
                // 2.) Add Random Number Between 0 and 1000000 and append to intended save name
                // This is assuming that the player won't get unlucky AND name new worlds the same everytime... Should probably add exception handling eventually grumble grumble
                UnityEngine.Debug.Log("World " + saveName + " was found when trying to create new world, setting new world name");
                GlobalReferences.DDDOL.saveName += UnityEngine.Random.Range(0, 1000000).ToString();
            }
        }

        // Set Date and Time to ZERO
        level.day = 0;
        level.time = GameReferences.dayNightCycle.MorningTime;

        // SAVE WORLD DATA
        if (GlobalReferences.DDDOL.saveName == "") { GlobalReferences.DDDOL.saveName += UnityEngine.Random.Range(0, 1000000).ToString(); }
        level.SaveLevel(GlobalReferences.DDDOL.saveName);

        // SAVE PLAYER DATA
        StartCoroutine(SaveAllPlayerDataAfterX(1f));

        worldCreated = true;

        loadScreen = GameObject.Find("--LoadScreen--");
        loadScreen.transform.Find("Loading").gameObject.SetActive(false);
    }

    public IEnumerator SaveAllPlayerDataAfterX(float x) { yield return new WaitForSeconds(x); player.GetComponent<Player>().SaveAllPlayerData(GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName); }

    public void LoadSavedWorld(string saveName)
    {
        worldCreated = false;

        #region Load World

        // LOAD SAVE FROM DISK
        saveManager.LoadAllDataFromDisk(saveName);

        // LOAD LEVEL INDEX (Last Level Player Was In)
        int levelIndex = saveManager.loadedData.playerData.levelIndex;

        // LOAD LEVEL SIZE
        int width = saveManager.loadedData.levelsSaved[levelIndex].width;
        int height = saveManager.loadedData.levelsSaved[levelIndex].height;
        float scale = saveManager.loadedData.levelsSaved[levelIndex].scale;
        UnityEngine.Debug.Log("Loaded World Size! x: " + width + " y: " + height + "    World Scale: " + scale + "    World Level Index: " + levelIndex);

        // Create Level
        level = new Level(width, height, scale, levelIndex);

        // Load World that Player is/was in
        level.LoadLevel(saveName, levelIndex); // Loads Tiles, Scale, Width and Height, and More

        // LOAD DATE AND TIME
        GetComponent<DayNightCycle>().LoadDateAndTime();

        #endregion

        #region Create World GameObjects

        for (int x = 0; x < level.Width; x++)
        {
            for (int y = 0; y < level.Height; y++)
            {
                GameObject tile = new GameObject();
                Tile tileData = level.GetTileAt(x, y);;

                tile.name = "Tile." + x + "_" + y;
                tile.layer = 9;
                tile.transform.position = 
                    new Vector3(
                        tileData.tileX * scale, 
                        tileData.tileY * scale, 
                        0);
                tile.transform.localScale = new Vector3(scale, scale);
                tile.transform.SetParent(this.transform, true);

                SpriteRenderer tile_SpriteRenderer = tile.AddComponent<SpriteRenderer>();
                tile_SpriteRenderer.material = tileMat;
                BoxCollider2D tile_Collider = tile.AddComponent<BoxCollider2D>();
                tile_Collider.size = new Vector3(1, 1);
                tile_Collider.enabled = false;

                tileData.SetTileTypeChangedCallback((_tile) => { OnTileTypeChanged(_tile, tile); });

                OnTileTypeChanged(tileData, tile); // CALL CALLBACK DIRECTLY TO UPDATE VISUALS
            }
        }

        #endregion

        #region Load Player

        // Get Player To Load To
        GameObject player = GameObject.Find("Player");
        // LOAD ALL PLAYER DATA
        player.GetComponent<Player>().LoadAllPlayerData(saveName);

        #endregion

        worldCreated = true;

        loadScreen = GlobalReferences.loadScreen;
        loadScreen.transform.Find("Loading").gameObject.SetActive(false);
    }


    // Take in Tile.cs Data and Apply it to GameObject (physics, textures, lighting, etc)
    public void OnTileTypeChanged(Tile tileData, GameObject tile) // Callback for when Tile Changes so Tile Visuals are updated when Tile Data is updated
    {
        // false = Not Solid = Layer 9
        // true = Solid = Layer 8

        // Get Sprite Database
        DataDontDestroyOnLoad data = GlobalReferences.DDDOL;

        if (tileData.Type == Tile.TileType.Air)
        {
            tile.GetComponent<SpriteRenderer>().sprite = null;
            SetTileState(tile, false);
        }
        else if (tileData.Type == Tile.TileType.Grass)
        {
            tile.GetComponent<SpriteRenderer>().sprite = data.spritesDB[(int)Tile.TileType.Grass][UnityEngine.Random.Range(0, 2)];
            SetTileState(tile, true);
        }
        else if (tileData.Type == Tile.TileType.Dirt)
        {
            tile.GetComponent<SpriteRenderer>().sprite = data.spriteDB[(int)Tile.TileType.Dirt];
            SetTileState(tile, true);
        }
        else if (tileData.Type == Tile.TileType.Stone)
        {
            tile.GetComponent<SpriteRenderer>().sprite = data.spriteDB[(int)Tile.TileType.Stone];
            SetTileState(tile, true);
        }
        else if (tileData.Type == Tile.TileType.DarkStone)
        {
            tile.GetComponent<SpriteRenderer>().sprite = data.spriteDB[(int)Tile.TileType.DarkStone];
            SetTileState(tile, true);
        }
        else if (tileData.Type == Tile.TileType.Log)
        {
            tile.GetComponent<SpriteRenderer>().sprite = data.spriteDB[(int)Tile.TileType.Log];
            SetTileState(tile, true);
        }
        else if (tileData.Type == Tile.TileType.Leaves)
        {
            tile.GetComponent<SpriteRenderer>().sprite = data.spriteDB[(int)Tile.TileType.Leaves];
            SetTileState(tile, false);
        }
        else if (tileData.Type == Tile.TileType.WoodBoards)
        {
            tile.GetComponent<SpriteRenderer>().sprite = data.spriteDB[(int)Tile.TileType.WoodBoards];
            SetTileState(tile, true);
        }
        else if (tileData.Type == Tile.TileType.Adobe)
        {
            tile.GetComponent<SpriteRenderer>().sprite = data.spriteDB[(int)Tile.TileType.Adobe];
            SetTileState(tile, true);
        }
        else if (tileData.Type == Tile.TileType.AdobeBricks)
        {
            tile.GetComponent<SpriteRenderer>().sprite = data.spriteDB[(int)Tile.TileType.AdobeBricks];
            SetTileState(tile, true);
        }
        else if (tileData.Type == Tile.TileType.Chest)
        {
            tile.GetComponent<SpriteRenderer>().sprite = data.spriteDB[(int)Tile.TileType.Chest];
            SetTileState(tile, true);
            tile.AddComponent<Chest>();
        }
        else if (tileData.Type == Tile.TileType.DevTile)
        {
            tile.GetComponent<SpriteRenderer>().sprite = data.spriteDB[(int)Tile.TileType.DevTile];
            SetTileState(tile, true);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Unrecognized Tile Type: " + tileData.Type.ToString() + ", Can't Set Tile Data");
        }
    }

    public void SetTileState(GameObject tile, bool solid)
    {
        if (solid)
        {
            tile.GetComponent<BoxCollider2D>().enabled = true;
            tile.layer = 8;
        }
        if (!solid)
        {
            tile.GetComponent<BoxCollider2D>().enabled = false;
            tile.layer = 9;
        }
    }

    public GameObject GetTileGameObjectAtTileCoord(int x, int y)
    {
        return GameObject.Find("Tile." + x + "_" + y);
    }

    public Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = (int)Mathf.Round(coord.x / level.Scale);
        int y = (int)Mathf.Round(coord.y / level.Scale);

        return level.GetTileAt(x, y);
    }
}
