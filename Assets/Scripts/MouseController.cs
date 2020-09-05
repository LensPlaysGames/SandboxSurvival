using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public GameObject Cursor;
    public GameObject Player;
    public Level level;

    public bool canSelect;

    Vector3 mousePos;
    Tile selectedTile;
    Tile.TileType buildTile = Tile.TileType.WoodBoards;

    public float scale;
    bool scaleSet;

    public GameObject particlesOnGrassDestroyed;

    void Start()
    {
        Cursor = GameObject.Find("Cursor");
        Player = GameObject.Find("Player");
    }

    void Update()
    {
        if (GameObject.Find("WorldGenerator").GetComponent<WorldGenerator>().worldCreated && !scaleSet)
        {
            scale = GameObject.Find("WorldGenerator").GetComponent<WorldGenerator>().GetLevelInstance().Scale;
            scaleSet = true;
        }

        // Get World Coordinates of Mouse Position
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // Get Currently Selected Tile
        if (scale != 0) { selectedTile = WorldGenerator.instance.GetTileAtWorldCoord(mousePos); }
        

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
            if (Input.GetMouseButtonUp(1)) 
            {
                // Check if Item in Selected Slot is a Tile, if so, Build Tile
                if (Player.GetComponent<Inventory>().selectedSlot.isTile == true) { BuildTile(); }
            }
        }
    }

    void SetCursorPos()
    {
        // Set Cursor Position so it follows mouse and stays on grid
        Vector3 cursorPos = new Vector3(selectedTile.tileX * scale, selectedTile.tileY * scale, 0);
        Cursor.transform.position = cursorPos;
    }

    void SetCanSelect()
    {
        GameObject pauseMenu = GameObject.Find("UICanvas").GetComponent<UIHandler>().pauseMenu;

        if (Mathf.Abs(Cursor.transform.position.x - Player.transform.position.x) > 6) { canSelect = false; } // If Too Far from Player Sideways, Can Not Select
        else if (Cursor.transform.position.y - Player.transform.position.y > 8) { canSelect = false; } // If Too Far Above Player, Can Not Select
        else if (Cursor.transform.position.y - Player.transform.position.y < -4.2) { canSelect = false; } // If Too Far below Player, Can Not Select

        else if (pauseMenu.activeInHierarchy) { canSelect = false; } // If Player is In Pause Menu

        else { canSelect = true; }

        if (canSelect) { Cursor.GetComponent<SpriteRenderer>().color = Color.white; }
        else { Cursor.GetComponent<SpriteRenderer>().color = Color.red; }
    }

    #region Break Tile

    void TryToDestroySelectedTile()
    {
        #region Based on Tile Type: Set Tile Break Time

        WorldGenerationParameters tileDestroyParams = GameObject.Find("DataDontDestroyOnLoad").GetComponent<WorldGenerationParameters>();

        if (selectedTile.Type == Tile.TileType.Dirt)
        {
            selectedTile.tileDestroyTime = tileDestroyParams.dirtDestroyTime;
        }
        else if (selectedTile.Type == Tile.TileType.Grass)
        {
            selectedTile.tileDestroyTime = tileDestroyParams.grassDestroyTime;
        }
        else if (selectedTile.Type == Tile.TileType.Stone || selectedTile.Type == Tile.TileType.DarkStone || selectedTile.Type == Tile.TileType.Adobe)
        {
            selectedTile.tileDestroyTime = tileDestroyParams.stoneDestroyTime;
        }
        else if (selectedTile.Type == Tile.TileType.Log)
        {
            selectedTile.tileDestroyTime = tileDestroyParams.logDestroyTime;
        }
        else if (selectedTile.Type == Tile.TileType.Leaves)
        {
            selectedTile.tileDestroyTime = tileDestroyParams.leavesDestroyTime;
        }
        else if (selectedTile.Type == Tile.TileType.WoodBoards)
        {
            selectedTile.tileDestroyTime = tileDestroyParams.woodBoardsDestroyTime;
        }
        else if (selectedTile.Type == Tile.TileType.DevTile)
        {
            selectedTile.tileDestroyTime = tileDestroyParams.devTileDestroyTime;
        }
        else { selectedTile.tileDestroyTime = .02f; }

        #endregion

        if (selectedTile.Type != Tile.TileType.Air) // If tile isn't Air (aka a tile to break and collect) Then Actually try to destroy it
        {
            // UnityEngine.Debug.Log("Attempting to BreakTileAfterX");
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
                        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlaySound("dirtCrunch" + Random.Range(1, 4));
                    }
                    else if (selectedTile.Type == Tile.TileType.Stone)
                    {
                        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlaySound("placedTile");
                        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlaySound("Hit");
                    }
                    else
                    {
                        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlaySound("placedTile");
                    }

                    #endregion

                    // Actually add the damned thing to Inventory
                    Player.GetComponent<Inventory>().AddTileToSlot(selectedTile.Type);
                    // Actually Break The Damn Tile
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
            if (Player.GetComponent<Inventory>().selectedSlot.isTile == true)
            {
                // Set Player Intended Build Tile to tile that is in the Selected Slot in Inventory
                buildTile = Player.GetComponent<Inventory>().selectedSlot.tileType;
                // If slot is not empty
                if (buildTile != Tile.TileType.Air)
                {
                    // Play Placed Tile Sound (could be based on tile type in future)
                    GameObject.Find("AudioManager").GetComponent<AudioManager>().PlaySound("placedTile");
                    // Remove Tile Placed From Slot Selected
                    Player.GetComponent<Inventory>().TakeFromSlot(Player.GetComponent<Inventory>().selectedSlot);
                    // Set Tile To The Intended Build Tile
                    selectedTile.Type = buildTile;
                }
            }
        }
    }
}
