using UnityEngine;

namespace U_Grow
{
    public class GlobalReferences : MonoBehaviour
    {
        public static GlobalReferences instance;

        void Awake()
        {
            if (instance != null) { UnityEngine.Debug.LogError("Multiple GlobalReferences In Scene, Destroying"); Destroy(this.gameObject); }
            else 
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
        }

        public static MenuHandler menuHandler;

        public static SaveManager saveManager;
        public static DataDontDestroyOnLoad DDDOL;
        public static LevelGenerationParameters levelGenParams;

        public static MusicManager musicManager;

        public static GameObject loadScreen;

        public static OnFirstRun firstRun;
        public static debugtest debugLog;
    }
}