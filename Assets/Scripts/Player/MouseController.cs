using System.Collections;
using UnityEngine;

namespace LensorRadii.U_Grow
{
    [RequireComponent(typeof(PlayerStats))]
    public class MouseController : MonoBehaviour
    {
        public GameObject Cursor;
        public GameObject Player;

        public PlayerStats stats;

        public bool lockedToGrid;

        public bool canSelect;

        public Vector3 mousePos;
        public Tile selectedTile;
        public Tile.TileType buildTile = Tile.TileType.DevTile;

        public float scale;
        private bool scaleSet;

        public GameObject particlesOnGrassDestroyed;

        private void Start()
        {
            Cursor = GameObject.Find("Cursor");

            Player = GameReferences.player;

            stats = Player.GetComponent<PlayerStats>();

            int locked = PlayerPrefs.GetInt("LockCursorPos", 0);
            if (locked != 0) { lockedToGrid = true; }
            else if (locked == 0) { lockedToGrid = false; }
        }

        private void Update()
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

        public void SetCursorPos()
        {
            // Set Cursor Position so it follows mouse and stays on grid
            Vector3 cursorPos = new Vector3();
            if (lockedToGrid) { cursorPos = new Vector3(selectedTile.tileX * scale, selectedTile.tileY * scale, 0); }
            else if (!lockedToGrid) { cursorPos = new Vector3(mousePos.x, mousePos.y, 0); }
            Cursor.transform.position = cursorPos;
        }

        public void SetCanSelect()
        {
            GameObject pauseMenu = GameReferences.uIHandler.pauseMenu;

            // If cursor is too far from player sideways, above, or below, player can not select
            if (Mathf.Abs(Cursor.transform.position.x - Player.transform.position.x) > 6
                || Cursor.transform.position.y - Player.transform.position.y > 8
                || Cursor.transform.position.y - Player.transform.position.y < -4.2
                || pauseMenu.activeInHierarchy)
            { canSelect = false; }
            else { canSelect = true; }

            Cursor.GetComponent<SpriteRenderer>().color = canSelect ? Color.white : Color.red;
        }

        #region Break Tile

        public void TryToDestroySelectedTile()
        {
            #region Based on Tile Type: Set Tile Break Time

            LevelGenerationParameters tileDestroyParams = GameObject.Find("DataDontDestroyOnLoad").GetComponent<LevelGenerationParameters>();



            switch (selectedTile.Type)
            {
                // FASTEST
                case Tile.TileType.Leaves:
                    selectedTile.tileDestroyTime = tileDestroyParams.fastestDestroyTime;
                    break;

                // FAST
                case Tile.TileType.Dirt:
                    selectedTile.tileDestroyTime = tileDestroyParams.fastDestroyTime;
                    break;

                // SLOW
                case Tile.TileType.Log:
                    selectedTile.tileDestroyTime = tileDestroyParams.slowDestroyTime;
                    break;

                // SLOWER
                case Tile.TileType.Stone:
                case Tile.TileType.DarkStone:
                case Tile.TileType.Adobe:
                    selectedTile.tileDestroyTime = tileDestroyParams.slowerDestroyTime;
                    break;

                // SLOWEST
                case Tile.TileType.DevTile:
                    selectedTile.tileDestroyTime = tileDestroyParams.slowestDestroyTime;
                    break;


                default:
                    selectedTile.tileDestroyTime = tileDestroyParams.defaultDestroyTime;
                    break;
            }

            #endregion

            if (selectedTile.Type != Tile.TileType.Air) // If tile isn't Air (aka a tile to break and collect) Then Actually try to destroy it
            {
                // Start Breaking Block Until tileDestroyTime <= 0
                StartCoroutine(BreakTileAfterX(selectedTile.tileDestroyTime * stats.tileDestroyTimeMultiplier));
            }
        }

        public IEnumerator BreakTileAfterX(float x)
        {
            Tile breakingTile = selectedTile;

            while (x >= 0)                          // REMOVE TIME SINCE LAST FRAME FROM DESTROY TIME EVERY FRAME IF PLAYER IS HOLDING BUTTON STILL
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

            if (selectedTile == breakingTile)       // Protection for using one coroutine to break anothers block (breaking blocks too fast while holding mouse button and dragging)
            {
                if (selectedTile.Type != Tile.TileType.Air)                         // Protection for player dragging off of block to break
                {
                    if (x <= 0f)                                                    // IF BLOCK SHOULD BE DESTROYED
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
                        else if (selectedTile.Type == Tile.TileType.Leaves)
                        {
                            GameReferences.audioManager.PlaySound("leaves" + Random.Range(1, 3));
                        }
                        else
                        {
                            GameReferences.audioManager.PlaySound("placedTile0");
                        }

                        #endregion


                        Slot slotFromTile = new Slot                                // Actually add the damned tile to player's inventory
                        {
                            count = 1,
                            empty = false,
                            item = new Item
                            {
                                itemType = Item.ItemType.Tile,
                                tileType = selectedTile.Type
                            }
                        };

                        GameReferences.playerInv.TryAddToSlot(slotFromTile);        // Add broken tile to player inventory
                        selectedTile.Type = Tile.TileType.Air;                      // Actually "break" the damn tile

                        yield break;
                    }
                }
            }


        }

        #endregion

        public void BuildTile()
        {
            if (selectedTile.Type == Tile.TileType.Air)
            {
                if (GameReferences.playerInv.selectedSlot.item.itemType == Item.ItemType.Tile)                      // Check if Selected Slot isTile, if so, place it
                {
                    buildTile = GameReferences.playerInv.selectedSlot.item.tileType;                                // Set Player Intended Build Tile to tile that is in the Selected Slot in Inventory
                    if (buildTile != Tile.TileType.Air)                                                             // If slot is not empty
                    {
                        GameReferences.audioManager.PlaySound("placedTile0");                                       // Play Placed Tile Sound (could be based on tile type in future)
                        GameReferences.playerInv.ModifySlotCount(GameReferences.playerInv.selectedSlotIndex, -1);   // Remove Tile Placed From Selected Inventory Slot 
                        selectedTile.Type = buildTile;                                                              // Set Tile To The Intended Build Tile
                    }
                }
            }
        }
    }
}