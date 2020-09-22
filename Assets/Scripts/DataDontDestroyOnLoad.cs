using System;
using System.IO;
using UnityEngine;

namespace LensorRadii.U_Grow
{
    public class DataDontDestroyOnLoad : MonoBehaviour
    {
        public static DataDontDestroyOnLoad instance;

        public string texturePack = "Textures";

        public Sprite[][] spritesDB;

        public Sprite[] spriteDB;
        private bool spriteDBLoaded;

        void Awake()
        {
            if (instance != null)
            {
                UnityEngine.Debug.LogError("Multiple DataDontDestroyOnLoad In Scene!!! Destroying " + this.name);
                Destroy(this);
            }
            else
            {
                instance = this;
                GlobalReferences.DDDOL = instance;
                DontDestroyOnLoad(this);
            }
        }

        void Start()
        {
            // Sprite Database Initilization
            if (!spriteDBLoaded)
            {
                spritesDB = new Sprite[Enum.GetNames(typeof(Tile.TileType)).Length][];

                for (int s = 0; s < Enum.GetNames(typeof(Tile.TileType)).Length; s++)
                {
                    spritesDB[s] = new Sprite[3];
                    spritesDB[s][0] = Resources.Load<Sprite>(texturePack + Path.DirectorySeparatorChar + Enum.GetName(typeof(Tile.TileType), s));
                    spritesDB[s][1] = Resources.Load<Sprite>(texturePack + Path.DirectorySeparatorChar + (Enum.GetName(typeof(Tile.TileType), s)) + "1");
                    spritesDB[s][2] = Resources.Load<Sprite>(texturePack + Path.DirectorySeparatorChar + (Enum.GetName(typeof(Tile.TileType), s)) + "2");
                }


                spriteDB = new Sprite[Enum.GetNames(typeof(Tile.TileType)).Length];

                for (int tile = 0; tile < Enum.GetNames(typeof(Tile.TileType)).Length; tile++)
                {
                    spriteDB[tile] = Resources.Load<Sprite>(texturePack + "/" + Enum.GetName(typeof(Tile.TileType), tile));
                }
                spriteDBLoaded = true;
            }
        }

        public bool newWorld;
        public bool playingMusic;

        public string saveName = ""; // Default World Save name if Player Names Empty World
    }

}