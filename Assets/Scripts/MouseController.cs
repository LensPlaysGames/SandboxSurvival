using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public GameObject Cursor;
    public GameObject Player;
    public World world;

    public bool canSelect;

    Vector3 mousePos;
    Tile selectedTile;
    Tile.TileType buildTile = Tile.TileType.Wood_Boards;

    public GameObject particlesOnGrassDestroyed;

    void Start()
    {
        Cursor = GameObject.Find("Cursor");
        Player = GameObject.Find("Player");
    }

    void Update()
    {
        // Get World Coordinates of Mouse Position
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // Get Currently Selected Tile
        selectedTile = WorldGenerator.instance.GetTileAtWorldCoord(mousePos);

        SetCursorPos();
        SetCanSelect();

        if (canSelect)
        {
            // Destroy Selected Tile, Add to Inventory
            if (Input.GetMouseButtonDown(0)) { TryToDestroySelectedTile(); }
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
        Vector3 cursorPos = new Vector3(selectedTile.tileX * 2, selectedTile.tileY * 2, 0);
        Cursor.transform.position = cursorPos;
    }

    void SetCanSelect()
    {
        GameObject pauseMenu = GameObject.Find("UICanvas").GetComponent<UIHandler>().pauseMenu;

        if (Mathf.Abs(Cursor.transform.position.x - Player.transform.position.x) > 6) { canSelect = false; } // If Too Far from Player Sideways, Can Not Select
        else if (Cursor.transform.position.y - Player.transform.position.y > 8) { canSelect = false; } // If Above max y, Can Not Select
        else if (Cursor.transform.position.y - Player.transform.position.y < -4) { canSelect = false; } // below min y, Can Not Select

        else if (pauseMenu.activeInHierarchy) { canSelect = false; } // If Player is In Pause Menu

        else { canSelect = true; }

        if (canSelect) { Cursor.GetComponent<SpriteRenderer>().color = Color.white; }
        else { Cursor.GetComponent<SpriteRenderer>().color = Color.red; }
    }

    #region Break Tile

    void TryToDestroySelectedTile()
    {
        #region Based on Tile Type: Set Tile Break Time

        if (selectedTile.Type == Tile.TileType.Grass || selectedTile.Type == Tile.TileType.Dirt)
        {
            selectedTile.tileDestroyTime = .25f;
        }
        else if (selectedTile.Type == Tile.TileType.Stone)
        {
            selectedTile.tileDestroyTime = .5f;
        }
        else if (selectedTile.Type == Tile.TileType.Wood_Boards || selectedTile.Type == Tile.TileType.DevTile)
        {
            selectedTile.tileDestroyTime = .4f;
        }

        #endregion

        if (selectedTile.Type != Tile.TileType.Air) // If tile isn't Air (aka a tile to break and collect) Then Actually try to destroy it
        {
            UnityEngine.Debug.Log("Attempting to BreakTileAfterX");
            // Start Breaking Block Until tileDestroyTime <= 0
            StartCoroutine(BreakTileAfterX(selectedTile.tileDestroyTime));
        }
    }

    IEnumerator BreakTileAfterX(float x) 
    {
        while (x >= 0) // REMOVE TIME SINCE LAST FRAME FROM DESTROY TIME EVERY FRAME IF PLAYER IS HOLDING BUTTON STILL
        {
            if (Input.GetMouseButton(0))
            {
                x -= Mathf.Round(Time.deltaTime * 100f) / 100f;
                yield return null;
            }
            else
            {
                yield break;
            }
        }

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

    void SaveGame()
    {
        WorldGenerator worldGenerator = GameObject.Find("WorldGenerator").GetComponent<WorldGenerator>();
        world = worldGenerator.GetWorldInstance();
        world.SaveTiles(GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName);
        //world.SaveTile(selectedTile.tileX, selectedTile.tileY);
    }
}
