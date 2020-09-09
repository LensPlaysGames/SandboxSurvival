using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GameReferences : MonoBehaviour
{
    public static LevelGenerator levelGenerator;

    public static GameObject player;
    public static Player playerScript;
    public static Inventory playerInv;

    public static UIHandler uIHandler;
    public static InventoryUI playerInvUI;

    public static AudioManager audioManager;

    public static DayNightCycle dayNightCycle;
    public static Light2D sunLight;
}
