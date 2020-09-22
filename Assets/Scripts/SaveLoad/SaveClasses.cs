using System;

namespace LensorRadii.U_Grow
{
    public class SaveClasses { }

    [Serializable]
    public class AllData
    {
        // World Data
        public LevelSaveData[] levelsSaved;

        // Player Data
        public PlayerSaveData playerData;
    }

    #region Sub Classes of AllData

    [Serializable]
    public class LevelSaveData
    {
        public int levelIndex;
        public int width, height;
        public float scale;
        public Tile[] tiles;
        public ExtraTileData[] tileDatas;
        public int day, time;
    }

    [Serializable]
    public class PlayerSaveData
    {
        public int levelIndex;

        public float x;
        public float y;

        public Slot[] playerInv;
    }

    #endregion

}