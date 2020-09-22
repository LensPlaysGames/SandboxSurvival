using UnityEngine;

namespace LensorRadii.U_Grow
{
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

        public ExtraTileData[,] tileDatas;
        public ExtraTileData[] tileDatasToSave;

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
            levelIndex = _levelIndex;

            width = _width;
            height = _height;
            scale = _scale;

            tiles = new Tile[width, height];

            tileDatas = new ExtraTileData[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y] = new Tile(this, x, y);
                }
            }

            Debug.Log("World Created. Size: " + width * height + " Width: " + width + " Height: " + height);
        }

        #endregion

        #region World Generation

        public void GenerateLevelTiles() // All this does is goes through each tile in world and sets tile type to something
        {
            // Initialize World Generation Characteristics from Where they Are Set In Inspector (DataDontDestroyOnLoad.WorldGenerationParameters.cs)
            DataDontDestroyOnLoad DDDOL = GlobalReferences.DDDOL;

            // Get Seed
            int seed = DDDOL.saveName.GetHashCode();
            UnityEngine.Random.InitState(seed);
            Debug.Log("Generated Level Seed: " + seed);

            // Get Level Params (Height for Surface, Underground)
            LevelGenerationParameters levelGenParams = GlobalReferences.levelGenParams;
            surfaceHeightMultiplier = levelGenParams.surfaceHeightPercentage;
            undergroundHeightMultiplier = levelGenParams.undergroundHeightPercentage;

            for (int x = 0; x < width; x++)
            {
                #region Random Height Gen

                surfaceHeightMultiplier *= UnityEngine.Random.Range(.99f, 1.01f);
                surfaceHeightMultiplier *= UnityEngine.Random.Range(.999f, 1.001f);

                if (UnityEngine.Random.Range(0, 101) <= 50) { surfaceHeightMultiplier *= UnityEngine.Random.Range(.99f, 1.01f); }
                if (UnityEngine.Random.Range(0, 101) <= 1) { surfaceHeightMultiplier *= UnityEngine.Random.Range(.98f, 1.02f); }

                undergroundHeightMultiplier *= UnityEngine.Random.Range(.99f, 1.01f);
                undergroundHeightMultiplier *= UnityEngine.Random.Range(.999f, 1.001f);

                if (UnityEngine.Random.Range(0, 101) <= 50) { undergroundHeightMultiplier *= UnityEngine.Random.Range(.99f, 1.01f); }
                if (UnityEngine.Random.Range(0, 101) <= 1) { undergroundHeightMultiplier *= UnityEngine.Random.Range(.95f, 1.05f); }

                // Keep Underground At bay
                if ((undergroundHeightMultiplier / surfaceHeightMultiplier) > .99f)
                {
                    undergroundHeightMultiplier *= .98f;
                }
                else if ((undergroundHeightMultiplier / surfaceHeightMultiplier) < .9f)
                {
                    undergroundHeightMultiplier *= 1.02f;
                }

                #endregion

                for (int y = 0; y < height; y++)
                {
                    // Generete Bottom of World
                    if (y == 0) { tiles[x, y].Type = Tile.TileType.DevTile; }

                    else if (y < height * undergroundHeightMultiplier)
                    {
                        // Below Underground Surface, Generate Stone
                        tiles[x, y].Type = Tile.TileType.Stone;

                        // Generate Underground Resources
                        int randInt = UnityEngine.Random.Range(0, 101);

                        if (randInt <= 6) { tiles[x, y].Type = Tile.TileType.DarkStone; }
                        else if (randInt <= 9) { tiles[x, y].Type = Tile.TileType.Adobe; }
                    }
                    else if (y < height * surfaceHeightMultiplier)
                    {
                        // Below Surface, Generate Dirt
                        tiles[x, y].Type = Tile.TileType.Dirt;
                    }
                    else if (y > height * surfaceHeightMultiplier)
                    {
                        // Above Surface, Generate Air
                        tiles[x, y].Type = Tile.TileType.Air;

                        // If At Bottom of Surface, Generate Grass
                        if (y - 1 < height * surfaceHeightMultiplier) { tiles[x, y].Type = Tile.TileType.Grass; }
                    }
                }
            }

            FindGrassMakeTrees();
        }

        public void FindGrassMakeTrees()
        {
            // Initialize Tree Generation Characteristics from Where they Are Set In Inspector (DataDontDestroyOnLoad.WorldGenerationParameters.cs)
            LevelGenerationParameters levelGenParams = GlobalReferences.levelGenParams;
            treeChance = levelGenParams.treeSpawnChance;
            leafOnTreeHeightMultiplier = levelGenParams.leafHeightOnTree;
            minTreeHeight = levelGenParams.minTreeHeight;
            maxTreeHeight = levelGenParams.maxTreeHeight;

            // Loop through all possible tree spawn locations (Every 3 tiles starting 2 tiles in from sides, 10 tiles below top of world)
            for (int x = 2; x < width - 2; x += 3)
            {
                for (int y = 10; y < height - 10; y++)
                {
                    // Find If Tile Type is Grass
                    if (tiles[x, y].Type == Tile.TileType.Grass)
                    {
                        int randTreeInt = UnityEngine.Random.Range(0, 100);

                        if (randTreeInt < treeChance * 100)
                        {
                            // Generate Tree
                            Debug.Log("Generating Tree at x: " + x + " y: " + y);

                            int treeHeight = UnityEngine.Random.Range(minTreeHeight, maxTreeHeight);

                            for (int t = 0; t <= treeHeight; t++)
                            {
                                // Place Dirt Below Tree
                                if (t == 0) { tiles[x, y].Type = Tile.TileType.Dirt; }
                                // Place Wood Until Tree Height is Reached
                                else { tiles[x, y + t].Type = Tile.TileType.Log; }

                                // Leaf Generation
                                leafOnTreeHeightMultiplier = UnityEngine.Random.Range(0.45f, 0.72f);
                                if (t > treeHeight * leafOnTreeHeightMultiplier)
                                {
                                    tiles[x - 1, y + t].Type = Tile.TileType.Leaves;
                                    tiles[x + 1, y + t].Type = Tile.TileType.Leaves;

                                    if (UnityEngine.Random.Range(0, 100) > 50)
                                    {
                                        tiles[x - 2, y + t].Type = Tile.TileType.Leaves;
                                        tiles[x + 2, y + t].Type = Tile.TileType.Leaves;
                                    }
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
            levelToSave.scale = scale;

            #region Save Tiles (Set tilesToSave)

            tilesToSave = new Tile[tiles.Length];

            int index = 0;
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

            levelToSave.day = day;
            levelToSave.time = time;

            #region Save Extra Tile Data

            tileDatasToSave = new ExtraTileData[tiles.Length];

            int indexprime = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tileDatasToSave[indexprime] = tileDatas[x, y];
                    indexprime++;
                }
            }

            levelToSave.tileDatas = tileDatasToSave;

            #endregion

            SaveManager saveManager = GlobalReferences.saveManager;
            saveManager.SetLevelSaveData(saveName, levelToSave);
        }

        public void LoadLevel(string saveName, int levelIndex)
        {
            SaveManager saveManager = GlobalReferences.saveManager;
            saveManager.LoadAllDataFromDisk(saveName);

            LevelSaveData levelSave = saveManager.loadedData.levelsSaved[levelIndex];

            this.levelIndex = levelSave.levelIndex;

            width = levelSave.width;
            height = levelSave.height;
            scale = levelSave.scale;

            int index = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y] = levelSave.tiles[index];
                    index++;
                }
            }

            int indexprime = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tileDatas[x, y] = levelSave.tileDatas[indexprime];
                    indexprime++;
                }
            }

            day = levelSave.day;
            time = levelSave.time;
        }


        #endregion

        public Tile GetTileAt(int _x, int _y) // Get Tile Data At Certain X and Y in (Data) 2D tiles Array
        {
            if (_x > width || _x < 0 || _y > height || _y < 0)
            {
                Debug.LogError("Tile (" + _x + "," + _y + ") is out of World");
                return null;
            }
            return tiles[_x, _y];
        }

        public ExtraTileData GetTileDataAt(int _x, int _y) // Get Tile Data At Certain X and Y in (Data) 2D tiles Array
        {
            if (_x > width || _x < 0 || _y > height || _y < 0)
            {
                Debug.LogError("Tile (" + _x + "," + _y + ") is out of World");
                return null;
            }
            return tileDatas[_x, _y];
        }
    }
}