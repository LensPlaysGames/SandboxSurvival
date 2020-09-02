using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public World world;

    public GameObject pauseMenu, player, inventoryUI, inventoryUIShowHideArrow, notificationBG, notification;
    private Sprite inventoryHideUI, inventoryShowUI;

    public TextMeshProUGUI coordX, coordY;
    private Vector3 lastPlayerPos;

    private bool pauseMenuToggle;

    public float notificationTime = 5f;

    void Start()
    {
        pauseMenu = GameObject.Find("--PauseUI--");
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        inventoryHideUI = Resources.Load<Sprite>("InventoryUIArrowHide");
        inventoryShowUI = Resources.Load<Sprite>("InventoryUIArrowShow");

        inventoryUI = GameObject.Find("--InventoryUI--");
        inventoryUIShowHideArrow = GameObject.Find("HideArrow");

        player = GameObject.Find("Player");
        coordX = GameObject.Find("coordX").GetComponent<TextMeshProUGUI>();
        coordY = GameObject.Find("coordY").GetComponent<TextMeshProUGUI>();

        notificationBG = GameObject.Find("--NotificationUI--");
        notification = Resources.Load<GameObject>("Notification");
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            TogglePauseMenu();
        }

        if (player.transform.position - lastPlayerPos != Vector3.zero)
        {
            SetCoordinateUI(player.transform.position.x, player.transform.position.y);
        }
        lastPlayerPos = player.transform.position;
    }

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
        string saveName = GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName;

        // Save World
        WorldGenerator worldGenerator = GameObject.Find("WorldGenerator").GetComponent<WorldGenerator>();
        world = worldGenerator.GetWorldInstance();
        world.SaveTiles(saveName);

        // Save Inventory
        player.GetComponent<Inventory>().SaveInventory(saveName);

        // Notify Player Game Saved
        SendNotif('\"' + saveName + '\"' + " Saved", Color.green);
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
        coordX.text = Mathf.Round(x / 2).ToString();
        coordY.text = Mathf.Round(y / 2).ToString();
    }

    #endregion

    #region Notifications

    public void SendNotif(string notifText, Color notifColor) { StartCoroutine(SendNotification(notifText, notifColor)); }

    public IEnumerator SendNotification(string notifText, Color notifColor)
    {
        GameObject notif = Instantiate(notification, notificationBG.transform);
        notif.GetComponentInChildren<TextMeshProUGUI>().text = notifText;
        notif.GetComponentInChildren<TextMeshProUGUI>().color = notifColor;
        notif.GetComponent<Animator>().SetFloat("ShowNotif", 1f);
        yield return new WaitForSeconds(notificationTime);
        Destroy(notif);
    }

    #endregion
}
