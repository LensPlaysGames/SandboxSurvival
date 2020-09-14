using System;

namespace U_Grow
{
    [Serializable]
    public class Tile
    {
        public enum TileType // Sprite Database Length is based on this, along with many other texture stuff... Hope that's okay and stuff
        {
            Air,
            Grass,
            Dirt,
            Stone,
            DarkStone,
            Log,
            Leaves,
            WoodBoards,
            Adobe,
            AdobeBricks,
            Chest,
            DevTile
        };

        TileType type = TileType.Air;
        int x;
        int y;

        public float tileDestroyTime;

        [NonSerialized]
        Action<Tile> tileTypeChangedCallback; // An Action is basically a list of functions under one name  THIS ONE IS GIVEN TO EACH TILE AND CALLED WHEN Type type ACCESSOR IS SET

        [NonSerialized]
        Level level; // Get Tile reference to Game World; Can't Serialize Due to creating new World instance upon load

        #region Accessors

        public TileType Type
        {
            get
            {
                return type;
            }
            set
            {
                TileType oldTileType = type;
                type = value;
                // Call the callback AKA let things know that the tile has updated IF tile type has changed and it is not already being called (lambdas are weird, I think this is right)
                if (tileTypeChangedCallback != null && oldTileType != type)
                {
                    UnityEngine.Debug.Log("Tile Type Changed from " + oldTileType + " to " + type);
                    tileTypeChangedCallback(this);
                }

            }
        }

        public int tileX
        {
            get
            {
                return x;
            }
        }

        public int tileY
        {
            get
            {
                return y;
            }
        }
        #endregion



        public Tile(Level _level, int _x, int _y)
        {
            this.level = _level;
            this.x = _x;
            this.y = _y;
        }

        public void SetTileTypeChangedCallback(Action<Tile> callback)
        {
            tileTypeChangedCallback += callback;
        }

        public void UnSetTileTypeChangedCallback(Action<Tile> callback)
        {
            tileTypeChangedCallback -= callback;
        }
    }
}