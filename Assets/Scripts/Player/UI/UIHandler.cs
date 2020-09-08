using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public Level level;

    public bool coordsEnabled;

    public GameObject pauseMenu, player, inventoryUI, inventoryUIShowHideArrow, craftMenu, notificationBG, notification;
    private Sprite inventoryHideUI, inventoryShowUI;

    public TextMeshProUGUI coordX, coordY;
    private Vector3 lastPlayerPos;

    private bool pauseMenuToggle, craftMenuToggle;

    public float scale;

    void Start()
    {
        pauseMenu = transform.Find("--PauseUI--").gameObject;
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        inventoryHideUI = Resources.Load<Sprite>("InventoryUIArrowHide");
        inventoryShowUI = Resources.Load<Sprite>("InventoryUIArrowShow");

        inventoryUI = transform.Find("--InventoryUI--").gameObject;
        inventoryUIShowHideArrow = GameObject.Find("HideArrow");

        player = GameObject.Find("Player");
        coordX = GameObject.Find("coordX").GetComponent<TextMeshProUGUI>();
        coordY = GameObject.Find("coordY").GetComponent<TextMeshProUGUI>();

        notificationBG = transform.Find("--NotificationUI--").gameObject;
        notification = Resources.Load<GameObject>("Prefabs/Notification");

        craftMenu = transform.Find("--CraftMenu--").gameObject;
        if (craftMenu != null)
        {
            craftMenu.GetComponent<Animator>().SetInteger("CraftMenu", 0);
        }

        if (PlayerPrefs.GetInt("Enabled Coordinates Display", 1) == 1) 
        { 
            coordsEnabled = true;
            transform.Find("--Coordinates--").gameObject.SetActive(true);
        }
        else 
        { 
            coordsEnabled = false;
            transform.Find("--Coordinates--").gameObject.SetActive(false);
        }

        SendNotif("You Awake From A Deep Slumber", Color.white, 20f);
    }

    void Update()
    {
        if (scale == 0) { scale = GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>().GetLevelInstance().Scale; }

        if (Input.GetButtonDown("Cancel"))
        {
            TogglePauseMenu();
        }

        if (player.transform.position - lastPlayerPos != Vector3.zero && coordsEnabled)
        {
            SetCoordinateUI(player.transform.position.x, player.transform.position.y);
        }
        lastPlayerPos = player.transform.position;
    }

    #region Craft Menu

    public void CraftMenu()
    {
        if (!craftMenuToggle)
        {
            craftMenu.GetComponent<Animator>().SetInteger("CraftMenu", 1);
            craftMenuToggle = true;
        }
        else if (craftMenuToggle)
        {
            craftMenu.GetComponent<Animator>().SetInteger("CraftMenu", 0);
            craftMenuToggle = false;
        }
    }

    #endregion

    #region Pause Menu

    void TogglePauseMenu()
    {
        if (!pauseMenuToggle)
        {
            pauseMenu.SetActive(true);
            pauseMenuToggle = true;
        }
        else
        {
            pauseMenu.SetActive(false);
            pauseMenuToggle = false;
        }

    }

    #region Button Functions

    public void SaveGame()
    {
        GameObject.Find("--LoadScreen--").transform.Find("Loading").gameObject.SetActive(true);

        string saveName = DataDontDestroyOnLoad.instance.saveName;

        // Save World
        GameObject levelGenerator = GameObject.Find("LevelGenerator");
        level = levelGenerator.GetComponent<LevelGenerator>().GetLevelInstance();

        level.day = levelGenerator.GetComponent<DayNightCycle>().GetDate();
        level.time = levelGenerator.GetComponent<DayNightCycle>().GetTime();

        level.SaveLevel(saveName);

        // Save All Player Data
        player.GetComponent<Player>().SaveAllPlayerData(saveName);

        GameObject.Find("--LoadScreen--").transform.Find("Loading").gameObject.SetActive(false);

        // Notify Player Game Saved
        SendNotif('\"' + saveName + '\"' + " Saved", Color.green, 10f);
    }
    public void ExitGame()
    {
        SaveGame();
        SceneManager.LoadScene("Menu");
    }
    public void ExitGameWITHOUTSAVING()
    {
        Application.Quit();
    }

    #endregion

    #endregion

    #region Inventory UI Show And Hide

    bool toggle;

    public void PressShowHideUIButton()
    {
        Animator anim = inventoryUI.GetComponent<Animator>();
        Image inventoryUIShowHideArrow = this.inventoryUIShowHideArrow.GetComponent<Image>();

        if (!toggle)
        {
            // Move Inventory With Animation
            anim.SetBool("InventoryUIShown", false);
            // Switch Hide/Show Arrow Around
            inventoryUIShowHideArrow.sprite = inventoryShowUI;
            toggle = true;
        }
        else
        {
            // Move Inventory With Animation
            anim.SetBool("InventoryUIShown", true);
            // Switch Hide/Show Arrow Around
            inventoryUIShowHideArrow.sprite = inventoryHideUI;
            toggle = false;
        }
    }

    #endregion

    #region Coordinate UI

    void SetCoordinateUI(float x, float y)
    {
        coordX.text = Mathf.Round(x / scale).ToString();
        coordY.text = Mathf.Round(y / scale).ToString();
    }

    #endregion

    #region Notifications

    public void SendNotif(string notifText, Color notifColor, float notifTime) { StartCoroutine(SendNotification(notifText, notifColor, notifTime)); }

    public IEnumerator SendNotification(string notifText, Color notifColor, float notifTime)
    {
        GameObject notif = Instantiate(notification, notificationBG.transform);
        notif.GetComponentInChildren<TextMeshProUGUI>().text = notifText;
        notif.GetComponentInChildren<TextMeshProUGUI>().color = notifColor;
        notif.GetComponent<Animator>().SetFloat("ShowNotif", 1f);
        yield return new WaitForSeconds(notifTime);
        Destroy(notif);
    }

    #endregion
}
