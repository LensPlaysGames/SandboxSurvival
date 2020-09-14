using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace U_Grow
{
    public class GameReferences : MonoBehaviour
    {
        public static LevelGenerator levelGenerator;

        public static GameObject player;
        public static Player playerScript;
        public static Inventory playerInv;

        public static UIMouseManager uIMouseManager;
        public static UIHandler uIHandler;
        public static InventoryUI playerInvUI;
        public static CraftUI craftUI;

        public static AudioManager audioManager;

        public static DayNightCycle dayNightCycle;
        public static Light2D sunLight;

        public static CraftSystem craftSystem;
        public static ListOfRecipes listOfRecipes;

        void Awake()
        {
            craftSystem = new CraftSystem(this);
        }
    }
}