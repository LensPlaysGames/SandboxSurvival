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
            if (Input.GetMouseButtonDown(0)) { DestroySelectedTile(); }
            if (Input.GetMouseButtonUp(1)) { BuildTile(); }
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
        if (Mathf.Abs(Cursor.transform.position.x - Player.transform.position.x) > 6) { canSelect = false; } // If Too Far from Player Sideways, Can Not Select
        else if (Cursor.transform.position.y - Player.transform.position.y > 8) { canSelect = false; } // If Above max y, Can Not Select
        else if (Cursor.transform.position.y - Player.transform.position.y < -4) { canSelect = false; } // below min y, Can Not Select
        else { canSelect = true; }

        if (canSelect) { Cursor.GetComponent<SpriteRenderer>().color = Color.white; }
        else { Cursor.GetComponent<SpriteRenderer>().color = Color.red; }
    }

    void DestroySelectedTile()
    {
        #region Play Sound based on Tile Type
        if (selectedTile.Type == Tile.TileType.Grass || selectedTile.Type == Tile.TileType.Dirt)
        {
            GameObject destroyParticles = Instantiate(particlesOnGrassDestroyed, Cursor.transform.position, Quaternion.identity);
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlaySound("dirtCrunch" + Random.Range(1, 4));
        }
        else if(selectedTile.Type == Tile.TileType.Stone)
        {
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlaySound("placedTile");
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlaySound("Hit");
        }
        else if (selectedTile.Type == Tile.TileType.Wood_Boards)
        {
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlaySound("placedTile");
        }

        #endregion

        if (selectedTile.Type != Tile.TileType.Air)
        {
            // Add The Damn Thing To Player Inventory
            GameObject.Find("Player").GetComponent<Inventory>().AddItemToSlot(selectedTile.Type);
            // Actually Build The Damn Thing (if it's breakable)
            selectedTile.Type = Tile.TileType.Air;

            // Save Change To World
            //SaveGame();
        }
    }

    void BuildTile()
    {
        if (selectedTile.Type == Tile.TileType.Air)
        {
            // Set Player Intended Build Tile to tile that is in the Selected Slot in Inventory
            buildTile = Player.GetComponent<Inventory>().selectedSlot.tileType;
            if (buildTile != Tile.TileType.Air)
            {
                // Play Placed Tile Sound (could be based on tile type in future)
                GameObject.Find("AudioManager").GetComponent<AudioManager>().PlaySound("placedTile");
                // Remove Tile Placed From Slot Selected
                Player.GetComponent<Inventory>().TakeFromSlot(Player.GetComponent<Inventory>().selectedSlot);
                // Set Tile To The Intended Build Tile
                selectedTile.Type = buildTile;

                //SaveGame();
            }

        }
    }

    void SaveGame()
    {
        WorldGenerator worldGenerator = GameObject.Find("WorldGenerator").GetComponent<WorldGenerator>();
        world = worldGenerator.GetWorldInstance();
        world.SaveTiles();
        //world.SaveTile(selectedTile.tileX, selectedTile.tileY);
    }
}
