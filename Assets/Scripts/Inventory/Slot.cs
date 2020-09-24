using System;

namespace LensorRadii.U_Grow
{
    [Serializable]
    public class Slot
    {
        public bool empty;
        public int count;

        public Item item;

        public Slot()
        {
            empty = true;
            count = 0;

            item = new Item
            {
                itemType = Item.ItemType.Tile,
                tileType = Tile.TileType.Air,
            };
        }
    }
}