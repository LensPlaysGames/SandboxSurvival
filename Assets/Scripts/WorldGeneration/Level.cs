using System;
using UnityEngine;

[Serializable]
public class Level
{
    #region World Generation Characteristics

    private float surfaceHeightMultiplier = .81f, undergroundHeightMultiplier = .69f;

    private float treeChance = .1f;
    private float leafOnTreeHeightMultiplier = 0.5f;
    private int minTreeHeight = 4, maxTreeHeight = 9;

    #endregion

    public int levelIndex;

    public int day, time;

    public Tile[,] tiles;
    public Tile[] tilesToSave;

    private int width;
    private int height;
    private float scale;

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
    public float Scale
    {
        get
        {
            return scale;
        }
    }

    #endregion

    #region Constructs

    // Default Initilializer for serializtion something something grumble grumble
    public Level() : this(270, 135, 1.5f, 0) { } 

    // Actual Construct that is called
    public Level(int _width, int _height, float _scale, int _levelIndex) // Creates tile at each point within world width and height
    {
        this.levelIndex = _levelIndex;

        this.width = _width;
        this.height = _height;
        this.scale = _scale;

        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                this.tiles[x, y] = new Tile(this, x, y);
            }
        }

        UnityEngine.Debug.Log("World Created. Size: " + (width * height) + " Width: " + width + " Height: " + height);
    }

    #endregion

    #region World Generation

    public void GenerateLevelTiles() // All this does is goes through each tile in world and sets tile type to something
    {
        // Initialize World Generation Characteristics from Where they Are Set In Inspector (DataDontDestroyOnLoad.WorldGenerationParameters.cs)
        LevelGenerationParameters levelGenParams = GameObject.Find("DataDontDestroyOnLoad").GetComponent<LevelGenerationParameters>();
        surfaceHeightMultiplier = levelGenParams.surfaceHeightPercentage;
        undergroundHeightMultiplier = levelGenParams.undergroundHeightPercentage;

        for (int x = 0; x < width; x++)
        {
            surfaceHeightMultiplier *= UnityEngine.Random.Range(.99f, 1.01f);
            undergroundHeightMultiplier *= UnityEngine.Random.Range(.99f, 1.01f);

            surfaceHeightMultiplier = Mathf.Clamp(surfaceHeightMultiplier, undergroundHeightMultiplier, .99f);
            undergroundHeightMultiplier = Mathf.Clamp(undergroundHeightMultiplier, 0.4f, levelGenParams.undergroundHeightPercentage);

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
                    int randInt = UnityEngine.Random.Range(0, 100);
                    if (randInt <= 10) { tiles[x, y].Type = Tile.TileType.DarkStone; }

                    else if (randInt <= 15) { tiles[x, y].Type = Tile.TileType.Adobe; }

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
        LevelGenerationParameters levelGenParams = GameObject.Find("DataDontDestroyOnLoad").GetComponent<LevelGenerationParameters>();
        treeChance = levelGenParams.treeSpawnChance;
        leafOnTreeHeightMultiplier = levelGenParams.leafHeightOnTree;
        minTreeHeight = levelGenParams.minTreeHeight;
        maxTreeHeight = levelGenParams.maxTreeHeight;

        // Loop through all possible tree spawn locations (Every 3 tiles starting 2 tiles in from sides, 10 tiles below top of world)
        for (int x = 2; x < (width - 2); x += 3)
        {
            for (int y = 0; y < (height - 10); y++)
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

    #region Save and Load World

    public void SaveLevel(string saveName)
    {
        LevelSaveData levelToSave = new LevelSaveData();

        levelToSave.levelIndex = levelIndex;

        levelToSave.width = width;
        levelToSave.height = height;

        #region Save Tiles (Set tilesToSave)

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

        levelToSave.tiles = tilesToSave;

        #endregion

        levelToSave.scale = scale;

        levelToSave.day = day;
        levelToSave.time = time;

        SaveManager saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        saveManager.SetWorldSaveData(saveName, levelToSave);
    }

    public void LoadLevel(string saveName, int levelIndex)
    {
        SaveManager saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        saveManager.LoadAllDataFromDisk(saveName);

        LevelSaveData levelSave = saveManager.loadedData.levelsSaved[levelIndex];

        this.levelIndex = levelSave.levelIndex;

        this.width = levelSave.width;
        this.height = levelSave.height;

        int index = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = levelSave.tiles[index];
                index++;
            }
        }

        this.scale = levelSave.scale;

        this.day = levelSave.day;
        this.time = levelSave.time;
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

