using System.Collections;
using UnityEngine;

namespace U_Grow
{
    [RequireComponent(typeof(PlayerStats))]
    public class MouseController : MonoBehaviour
    {
        public GameObject Cursor;
        public GameObject Player;

        public PlayerStats stats;

        public bool lockedToGrid;

        public bool canSelect;

        Vector3 mousePos;
        Tile selectedTile;
        Tile.TileType buildTile = Tile.TileType.DevTile;

        public float scale;
        bool scaleSet;

        public GameObject particlesOnGrassDestroyed;

        void Start()
        {
            Cursor = GameObject.Find("Cursor");

            Player = GameReferences.player;

            stats = Player.GetComponent<PlayerStats>();

            int locked = PlayerPrefs.GetInt("LockCursorPos", 0);
            if (locked != 0) { lockedToGrid = true; }
            else if (locked == 0) { lockedToGrid = false; }
        }

        void Update()
        {
            if (LevelGenerator.instance.worldCreated && !scaleSet)
            {
                scale = LevelGenerator.instance.GetLevelInstance().Scale;
                scaleSet = true;
            }

            // Get World Coordinates of Mouse Position
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            // Get Currently Selected Tile
            if (scaleSet) { selectedTile = LevelGenerator.instance.GetTileAtWorldCoord(mousePos); }

            // If Tile is Selected, Set Cursor Pos
            if (selectedTile != null)
            {
                SetCursorPos();
            }

            SetCanSelect();

            if (canSelect)
            {
                // Destroy Selected Tile, Add to Inventory
                if (Input.GetMouseButton(0)) { TryToDestroySelectedTile(); }
                // Use Selected Slot Key
                if (Input.GetMouseButton(1))
                {
                    // Check if Item in Selected Slot is a Tile, if so, Build Tile
                    if (GameReferences.playerInv.selectedSlot.item.itemType == Item.ItemType.Tile) { BuildTile(); }
                }
            }
        }

        void SetCursorPos()
        {
            // Set Cursor Position so it follows mouse and stays on grid
            Vector3 cursorPos = new Vector3();
            if (lockedToGrid) { cursorPos = new Vector3(selectedTile.tileX * scale, selectedTile.tileY * scale, 0); }
            else if (!lockedToGrid) { cursorPos = new Vector3(mousePos.x, mousePos.y, 0); }
            Cursor.transform.position = cursorPos;
        }

        void SetCanSelect()
        {
            GameObject pauseMenu = GameReferences.uIHandler.pauseMenu;

            if (Mathf.Abs(Cursor.transform.position.x - Player.transform.position.x) > 6) { canSelect = false; } // If Too Far from Player Sideways, Can Not Select
            else if (Cursor.transform.position.y - Player.transform.position.y > 8) { canSelect = false; } // If Too Far Above Player, Can Not Select
            else if (Cursor.transform.position.y - Player.transform.position.y < -4.2) { canSelect = false; } // If Too Far below Player, Can Not Select

            else if (pauseMenu.activeInHierarchy) { canSelect = false; } // If Player is In Pause Menu, Can Not Select

            else { canSelect = true; }

            if (canSelect) { Cursor.GetComponent<SpriteRenderer>().color = Color.white; }
            else { Cursor.GetComponent<SpriteRenderer>().color = Color.red; }
        }

        #region Break Tile

        void TryToDestroySelectedTile()
        {
            #region Based on Tile Type: Set Tile Break Time

            LevelGenerationParameters tileDestroyParams = GameObject.Find("DataDontDestroyOnLoad").GetComponent<LevelGenerationParameters>();

            selectedTile.tileDestroyTime = tileDestroyParams.defaultDestroyTime;

            if (selectedTile.Type == Tile.TileType.Dirt)
            {
                selectedTile.tileDestroyTime = tileDestroyParams.fastDestroyTime;
            }
            else if (selectedTile.Type == Tile.TileType.Stone || selectedTile.Type == Tile.TileType.DarkStone || selectedTile.Type == Tile.TileType.Adobe)
            {
                selectedTile.tileDestroyTime = tileDestroyParams.slowerDestroyTime;
            }
            else if (selectedTile.Type == Tile.TileType.Log)
            {
                selectedTile.tileDestroyTime = tileDestroyParams.slowDestroyTime;
            }
            else if (selectedTile.Type == Tile.TileType.Leaves)
            {
                selectedTile.tileDestroyTime = tileDestroyParams.fastestDestroyTime;
            }
            else if (selectedTile.Type == Tile.TileType.DevTile)
            {
                selectedTile.tileDestroyTime = tileDestroyParams.slowestDestroyTime;
            }

            #endregion

            selectedTile.tileDestroyTime *= stats.tileDestroyTimeMultiplier;

            if (selectedTile.Type != Tile.TileType.Air) // If tile isn't Air (aka a tile to break and collect) Then Actually try to destroy it
            {
                // Start Breaking Block Until tileDestroyTime <= 0
                StartCoroutine(BreakTileAfterX(selectedTile.tileDestroyTime));
            }
        }

        IEnumerator BreakTileAfterX(float x)
        {
            Tile breakingTile = selectedTile;

            while (x >= 0) // REMOVE TIME SINCE LAST FRAME FROM DESTROY TIME EVERY FRAME IF PLAYER IS HOLDING BUTTON STILL
            {
                if (Input.GetMouseButton(0))
                {
                    x -= Time.deltaTime;
                    yield return null;
                }
                else
                {
                    yield break;
                }
            }

            if (selectedTile == breakingTile) // Protection for using one coroutine to break anothers block (breaking blocks too fast while holding mouse button and dragging)
            {
                if (selectedTile.Type != Tile.TileType.Air) // Protection for player dragging off of block to break
                {
                    if (x <= 0f) // IF BLOCK SHOULD BE DESTROYED
                    {

                        #region Based On Tile Type: Play Particles, Play Sound

                        if (selectedTile.Type == Tile.TileType.Grass || selectedTile.Type == Tile.TileType.Dirt)
                        {
                            GameObject destroyParticles = Instantiate(particlesOnGrassDestroyed, Cursor.transform.position, Quaternion.identity);
                            GameReferences.audioManager.PlaySound("dirtCrunch" + Random.Range(1, 4));
                        }
                        else if (selectedTile.Type == Tile.TileType.Stone)
                        {
                            GameReferences.audioManager.PlaySound("placedTile0");
                            GameReferences.audioManager.PlaySound("Hit");
                        }
                        else if (selectedTile.Type == Tile.TileType.Log || selectedTile.Type == Tile.TileType.WoodBoards)
                        {
                            GameReferences.audioManager.PlaySound("placedTile1");
                        }
                        else
                        {
                            GameReferences.audioManager.PlaySound("placedTile0");
                        }

                        #endregion

                        // Actually Add the Damned Tile to Inventory
                        Slot slotFromTile = new Slot
                        {
                            count = 1,
                            empty = false,
                            item = new Item
                            {
                                itemType = Item.ItemType.Tile,
                                tileType = selectedTile.Type
                            }
                        };

                        GameReferences.playerInv.TryAddToSlot(slotFromTile);

                        // Actually "Break" The Damn Tile
                        selectedTile.Type = Tile.TileType.Air;

                        yield break;
                    }
                }
            }


        }

        #endregion

        void BuildTile()
        {
            if (selectedTile.Type == Tile.TileType.Air)
            {
                // Check if Selected Slot isTile, if so, place it
                if (GameReferences.playerInv.selectedSlot.item.itemType == Item.ItemType.Tile)
                {
                    // Set Player Intended Build Tile to tile that is in the Selected Slot in Inventory
                    buildTile = GameReferences.playerInv.selectedSlot.item.tileType;
                    // If slot is not empty
                    if (buildTile != Tile.TileType.Air)
                    {
                        // Play Placed Tile Sound (could be based on tile type in future)
                        GameReferences.audioManager.PlaySound("placedTile0");
                        // Remove Tile Placed From Slot Selected
                        GameReferences.playerInv.TryTakeFromSlot(GameReferences.playerInv.selectedSlotIndex);
                        // Set Tile To The Intended Build Tile
                        selectedTile.Type = buildTile;
                    }
                }
            }
        }
    }
}