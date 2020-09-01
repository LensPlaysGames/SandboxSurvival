using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public World world;

    public GameObject pauseMenu, player, inventoryUI, inventoryUIShowHideArrow;
    private Sprite inventoryHideUI, inventoryShowUI;

    public TextMeshProUGUI coordX, coordY;
    private Vector3 lastPlayerPos;

    private bool pauseMenuToggle;

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
        WorldGenerator worldGenerator = GameObject.Find("WorldGenerator").GetComponent<WorldGenerator>();
        world = worldGenerator.GetWorldInstance();
        world.SaveTiles();
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
        coordX.text = Mathf.Round(x).ToString();
        coordY.text = Mathf.Round(y).ToString();
    }

    #endregion
}
