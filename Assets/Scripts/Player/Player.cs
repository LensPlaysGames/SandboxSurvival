using UnityEngine;

namespace U_Grow
{
    [RequireComponent(typeof(Inventory), typeof(PlayerMovement), typeof(PlayerStats))]
    public class Player : MonoBehaviour
    {
        public static Player instance;
        public PlayerStats stats;

        public int level;

        private bool playerDataLoaded;

        void Awake()
        {
            if (instance != null) { Debug.LogError("Multiple PLAYERS in scene... WHAT THE FUCK!?"); }
            else
            {
                instance = this;
                GameReferences.playerScript = instance;
                GameReferences.player = instance.gameObject;
            }

            stats = GetComponent<PlayerStats>();
        }

        void Start()
        {
            if (!playerDataLoaded)
            {
                Level l = GameReferences.levelGenerator.GetLevelInstance();
                Vector3 middleTopofWorld = new Vector3((l.Width / 2) * l.Scale, l.Height * l.Scale);
                Debug.Log("Player Data Not Loaded! Setting Position to: " + middleTopofWorld);
                transform.position = middleTopofWorld;
                playerDataLoaded = true;
            }
        }

        void FixedUpdate()
        {
            // Shitty Respawn to Middle of World If Below Certain Y Value or above certain X... yuck!
            Level l = GameReferences.levelGenerator.GetLevelInstance();
            if (transform.position.y < -10
              || transform.position.x < 0
              || transform.position.x > l.Width * l.Scale
              || transform.position.y > (l.Height * l.Scale) + 10)
            {
                Vector3 middleTopofWorld = new Vector3((l.Width / 2) * l.Scale, l.Height * l.Scale);
                Debug.Log("Player Fell Below Level! Setting Position to: " + middleTopofWorld);
                transform.position = middleTopofWorld;
            }
        }  

        #region Player Data Save/Load

        public void SaveAllPlayerData(string saveName)
        {
            PlayerSaveData playerDataToSave = new PlayerSaveData();

            // Current World Level Player is In
            playerDataToSave.levelIndex = level;

            // Player Pos
            playerDataToSave.x = Mathf.Round(transform.position.x * 100) / 100;
            playerDataToSave.y = Mathf.Round(Mathf.Ceil(transform.position.y) * 100) / 100;

            // Player Inventory
            playerDataToSave.playerInv = GetComponent<Inventory>().GetInventoryToSave();

            SaveManager saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
            saveManager.SetPlayerDataSaveData(saveName, playerDataToSave);
        }

        public void LoadAllPlayerData(string saveName)
        {
            SaveManager saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
            saveManager.LoadAllDataFromDisk(saveName);

            level = saveManager.loadedData.playerData.levelIndex;

            Vector3 loadedPos = new Vector3(saveManager.loadedData.playerData.x, saveManager.loadedData.playerData.y);
            transform.position = loadedPos;

            GetComponent<Inventory>().LoadInventory(saveName);

            playerDataLoaded = true;
        }

        #endregion
    }
}