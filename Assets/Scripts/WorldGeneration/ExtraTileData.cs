using System;
using System.Collections.Generic;

namespace LensorRadii.U_Grow
{
    [Serializable]
    public class ExtraTileData
    {
        [NonSerialized]
        Level level;

        public List<Slot> inventorySlots = new List<Slot>();

        public int x;
        public int y;

        public ExtraTileData(Level _level, int _x, int _y)
        {
            level = _level;
            x = _x;
            y = _y;
        }

        public void SetSlots(List<Slot> slots)
        {
            inventorySlots = slots;
        }
    }
}