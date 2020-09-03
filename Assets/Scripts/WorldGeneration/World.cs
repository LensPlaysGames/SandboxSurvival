using System;
using UnityEngine;

[Serializable]
public class World
{
    #region World Generation Characteristics

    private float surfaceHeightMultiplier = .81f, undergroundHeightMultiplier = .69f;

    private float treeChance = .1f;
    private float leafOnTreeHeightMultiplier = 0.5f;
    private int minTreeHeight = 4, maxTreeHeight = 9;

    #endregion

    #region Singletons

    public static World instance;
    public static SaveManager saveManager;

    void Start()
    {
        if (instance != null) { UnityEngine.Debug.LogError("MULTIPLE WORLDS TRYING TO CREATE"); }
        instance = this;

        if (saveManager != null) { UnityEngine.Debug.LogError("There Should NOT be more than one SaveManager"); }
        saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        UnityEngine.Debug.Log("Save Manager cached in World.cs as saveManager SaveManager");
    }

    #endregion

    public Tile[,] tiles;
    public Tile[] tilesToSave;

    private int width;
    private int height;

    #region Accessors

    public int Width
    {
        get
        {
            return width;
        }
    }
    public int Height
    {
        get
        {
            return height;
        }
    }

    #endregion

    #region Constructs

    public World() : this(270, 90) { } // Default Initilializer for serializtion something something grumble grumble

    public World(int _width, int _height) // Creates tile at each point within world width and height
    {
        this.width = _width;
        this.height = _height;

        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
            }
        }

        UnityEngine.Debug.Log("World Created. Size: " + (width * height) + " Width: " + width + " Height: " + height);
    }

    #endregion

    #region World Generation

    public void GenerateRandomTiles() // All this does is goes through each tile in world and sets tile type to something
    {
        // Initialize World Generation Characteristics from Where they Are Set In Inspector (DataDontDestroyOnLoad.WorldGenerationParameters.cs)
        WorldGenerationParameters worldGenParams = GameObject.Find("DataDontDestroyOnLoad").GetComponent<WorldGenerationParameters>();
        surfaceHeightMultiplier = worldGenParams.surfaceHeightPercentage;
        undergroundHeightMultiplier = worldGenParams.undergroundHeightPercentage;

        for (int x = 0; x < width; x++)
        {
            surfaceHeightMultiplier *= UnityEngine.Random.Range(.99f, 1.01f);
            undergroundHeightMultiplier *= UnityEngine.Random.Range(.99f, 1.01f);

            surfaceHeightMultiplier = Mathf.Clamp(surfaceHeightMultiplier, undergroundHeightMultiplier, .99f);
            undergroundHeightMultiplier = Mathf.Clamp(undergroundHeightMultiplier, 0.4f, worldGenParams.undergroundHeightPercentage);

            for (int y = 0; y < height; y++)
            {
                if (y > (height * surfaceHeightMultiplier))
                {
                    // Generate Sky Area Above Ground
                    tiles[x, y].Type = Tile.TileType.Air;
                    // Generate Grass at bottom of Sky and top of Floor
                    if ((y - 1) < (height * surfaceHeightMultiplier)) { tiles[x, y].Type = Tile.TileType.Grass; }
                }
                else if (y < (height * undergroundHeightMultiplier))
                {
                    // Generate Underground
                    int randInt = UnityEngine.Random.Range(0, 11);
                    if (randInt == 0) { tiles[x, y].Type = Tile.TileType.Air; }

                    // INSERT UNDERGROUND RESOURCES HERE

                    else { tiles[x, y].Type = Tile.TileType.Stone; } // Default Underground Resource Type
                }
                else if (y == 0) { tiles[x, y].Type = Tile.TileType.DevTile; }
                else
                {
                    // Tile is Dirt
                    tiles[x, y].Type = Tile.TileType.Dirt;
                }
            }
        }

        FindGrassMakeTrees();

    }

    public void FindGrassMakeTrees()
    {
        // Initialize Tree Generation Characteristics from Where they Are Set In Inspector (DataDontDestroyOnLoad.WorldGenerationParameters.cs)
        WorldGenerationParameters worldGenParams = GameObject.Find("DataDontDestroyOnLoad").GetComponent<WorldGenerationParameters>();
        treeChance = worldGenParams.treeSpawnChance;
        leafOnTreeHeightMultiplier = worldGenParams.leafHeightOnTree;
        minTreeHeight = worldGenParams.minTreeHeight;
        maxTreeHeight = worldGenParams.maxTreeHeight;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Tree Chance
                int randTreeInt = UnityEngine.Random.Range(0, 100);

                if ((randTreeInt) < (treeChance * 100))
                {
                    // Find If Tile Type is Grass
                    if (tiles[x, y].Type == Tile.TileType.Grass)
                    {
                        UnityEngine.Debug.Log("Generating Tree at x: " + x + " y: " + y);
                        // Generate Tree

                        int treeHeight = UnityEngine.Random.Range(minTreeHeight, maxTreeHeight);
                        for (int t = 0; t <= treeHeight; t++)
                        {
                            // Place Dirt Below Tree
                            if (t == 0) { tiles[x, (y)].Type = Tile.TileType.Dirt; }
                            // Place Wood Until Tree Height is Reached
                            else { tiles[x, (y + t)].Type = Tile.TileType.Log; }

                            // Leaf Generation
                            leafOnTreeHeightMultiplier = UnityEngine.Random.Range(0.45f, 0.72f);
                            if (t > (treeHeight * leafOnTreeHeightMultiplier))
                            {
                                tiles[x - 1, y + t].Type = Tile.TileType.Leaves;
                                tiles[x + 1, y + t].Type = Tile.TileType.Leaves;
                            }
                            if (t == treeHeight)
                            {
                                tiles[x, y + t + 1].Type = Tile.TileType.Leaves;
                            }
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region Save and Load Tiles

    public void SaveTiles(string saveName) // Loop Through All Tiles in World and Save 2D Tile[,] tiles Array to 1D tilesToSave Array; Get Reference to Save Manager And Save Data To File on Hard Disk
    {
        int index = 0;

        tilesToSave = new Tile[tiles.Length];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++) 
            {
                tilesToSave[index] = tiles[x, y];
                index++;
            }
        }

        saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        saveManager.SaveWorldDataToDisk(saveName, tilesToSave);

    }

    public void LoadTiles(string saveName) // Reset (Data) 2D tiles Array; Get Reference to Save Manager and Load Data From Hard Disk; Loop through all tiles and set Tile (Data) to loadedTile (Data)
    {
        tiles = new Tile[width, height];

        saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        saveManager.LoadWorldDataFromDisk(saveName);

        int index = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = saveManager.loadedTiles[index];
                index++;
            }
        }

    }

    #endregion

    public Tile GetTileAt(int _x, int _y) // Get Tile Data At Certain X and Y in (Data) 2D tiles Array
    {
        if (_x > width || _x < 0 || _y > width || _y < 0)
        {
            UnityEngine.Debug.LogError("Tile (" + _x + "," + _y + ") is out of World");
            return null;
        }
        return tiles[_x, _y];
    }
}

