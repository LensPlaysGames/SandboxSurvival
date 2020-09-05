using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class MenuHandler : MonoBehaviour
{
    public GameObject mainMenu, selectSaveMenu, newWorldOptions, optionsMenu;

    public WorldGenerationParameters worldGenParams;
    public Slider tileScale, width, height, masterVolume, musicVolume, sfxVolume;

    private bool moving, inMainMenu;

    void Start()
    {
        inMainMenu = true;
        mainMenu = GameObject.Find("--MainMenu--");
        selectSaveMenu = GameObject.Find("--SelectSaveToLoad--"); 
        if (selectSaveMenu != null)
        {
            selectSaveMenu.SetActive(false);
        }
        newWorldOptions = GameObject.Find("--NewWorldOptions--");
        if (newWorldOptions != null)
        {
            newWorldOptions.SetActive(false);
        }
        optionsMenu = GameObject.Find("--Options--");
        if (optionsMenu != null)
        {
            optionsMenu.SetActive(false);
        }

        #region Initialize Sliders for Advanced World Options

        worldGenParams = GameObject.Find("DataDontDestroyOnLoad").GetComponent<WorldGenerationParameters>();

        GameObject container = newWorldOptions.transform.Find("AdvancedWorldOptions").gameObject;
        GameObject containerT = container.transform.Find("--TileScale--").gameObject;
        tileScale = containerT.transform.Find("Slider").GetComponent<Slider>();

        GameObject containerW = container.transform.Find("--Width--").gameObject;
        width = containerW.transform.Find("WidthSlider").GetComponent<Slider>();

        GameObject containerH = container.transform.Find("--Height--").gameObject;
        height = containerH.transform.Find("HeightSlider").GetComponent<Slider>();

        tileScale.value = worldGenParams.defaultScale;
        tileScale.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = worldGenParams.defaultScale.ToString();
        width.value = worldGenParams.defaultWidth;
        width.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = worldGenParams.defaultWidth.ToString();
        height.value = worldGenParams.defaultHeight;
        height.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = worldGenParams.defaultHeight.ToString();

        #endregion

        #region Initialize Sliders for Options

        GameObject containerO = optionsMenu.transform.Find("OptionsBackground").gameObject;
        GameObject containerA = containerO.transform.Find("--Audio--").gameObject;

        masterVolume = containerA.transform.Find("MasterVolume").GetComponent<Slider>();
        musicVolume = containerA.transform.Find("MusicVolume").GetComponent<Slider>();
        sfxVolume = containerA.transform.Find("SfxVolume").GetComponent<Slider>();

        masterVolume.value = PlayerPrefs.GetFloat("Master Volume", -3);
        masterVolume.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = (Mathf.Round((Mathf.Pow(1 - (Mathf.Round(PlayerPrefs.GetFloat("Master Volume", -3) * -1) / 80), 5.807f)) * 100) / 100).ToString();
        musicVolume.value = PlayerPrefs.GetFloat("Music Volume", -3);
        musicVolume.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = (Mathf.Round((Mathf.Pow(1 - (Mathf.Round(PlayerPrefs.GetFloat("Music Volume", -3) * -1) / 80), 5.807f)) * 100) / 100).ToString();
        sfxVolume.value = PlayerPrefs.GetFloat("Sfx Volume", -3);
        sfxVolume.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = (Mathf.Round((Mathf.Pow(1 - (Mathf.Round(PlayerPrefs.GetFloat("Sfx Volume", -3) * -1) / 80), 5.807f)) * 100) / 100).ToString();

        #endregion

    }

    void Update()
    {
        if (!moving && inMainMenu)
        {
            if (Input.GetButtonDown("Cancel")) { UnityEngine.Debug.Log("Game Shutting Down. Good Night..."); Application.Quit(); }
        }
    }

    public IEnumerator DisableMenuAfterX(float x, GameObject menu) { 
        yield return new WaitForSeconds(x);

        if (menu == selectSaveMenu)
        {
            GameObject go = GameObject.Find("--SelectSaveToLoad--").transform.Find("WorldSavesBackground").gameObject;
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Destroy(go.transform.GetChild(i).gameObject);
            }
        }

        menu.SetActive(false); 
        moving = false; 
    }

    public void GoToEnterWorldName()
    {
        moving = true;
        inMainMenu = false;
        mainMenu.GetComponent<Animator>().Play("Base Layer.MainMenuHide");
        StartCoroutine(DisableMenuAfterX(.51f, mainMenu));
        newWorldOptions.SetActive(true);
    }

    public void SelectSaveFileMenu()
    {
        moving = true;
        inMainMenu = false;

        // (Update Visuals) Set Menu GameObjects Active State, Animations
        mainMenu.GetComponent<Animator>().Play("Base Layer.MainMenuHide");
        StartCoroutine(DisableMenuAfterX(.51f, mainMenu));
        selectSaveMenu.SetActive(true);

        // Get Reference to Load Save Button Prefab
        GameObject loadWorldButton = Resources.Load<GameObject>("LoadWorldButton");

        // Find all Save Files in Save Directory
        SaveManager saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        saveManager.GetSaveFiles();

        // For Every Save File found, Create Button and Give it Correct Name
        foreach (string s in saveManager.worldSaves)
        {
            // Create Button
            GameObject loadSaveButton = Instantiate(loadWorldButton, GameObject.Find("WorldSavesBackground").transform);

            // Get Save Name and Remove Prefix and Extension (World_ and .map, Respectively)
            string saveFileName = Path.GetFileName(s);
            string saveName = saveFileName.Substring(saveFileName.IndexOf("_") + 1);
            int index = saveName.LastIndexOf(".");
            if (index > 0) { saveName = saveName.Substring(0, index); }

            // Set Button Text to Name of Save
            loadSaveButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = saveName;
        }
    }

    public void GoToOptionsMenu()
    {
        moving = true;
        inMainMenu = false;
        mainMenu.GetComponent<Animator>().Play("Base Layer.MainMenuHide");
        StartCoroutine(DisableMenuAfterX(.51f, mainMenu));
        optionsMenu.SetActive(true);
    }

    public void SetMasterVol(float vol) 
    { 
        PlayerPrefs.SetFloat("Master Volume", Mathf.Round(vol * 100) / 100);
        masterVolume.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = (Mathf.Round((Mathf.Pow(1 - (Mathf.Round(vol * -1) / 80), 5.807f)) * 100) / 100).ToString();
        GameObject.Find("MusicManager").GetComponent<MusicManager>().UpdateMixerVolumes(); 
    }
    public void SetMusicVol(float vol) 
    { 
        PlayerPrefs.SetFloat("Music Volume", Mathf.Round(vol * 100) / 100);
        musicVolume.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = (Mathf.Round((Mathf.Pow(1 - (Mathf.Round(vol * -1) / 80), 5.807f)) * 100) / 100).ToString();
        GameObject.Find("MusicManager").GetComponent<MusicManager>().UpdateMixerVolumes(); 
    }
    public void SetSfxVol(float vol) 
    { 
        PlayerPrefs.SetFloat("Sfx Volume", Mathf.Round(vol * 100) / 100); 
        sfxVolume.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = (Mathf.Round((Mathf.Pow(1 - (Mathf.Round(vol * -1) / 80), 5.807f)) * 100) / 100).ToString();
        GameObject.Find("MusicManager").GetComponent<MusicManager>().UpdateMixerVolumes(); 
    }

    public void SetWorldNameFromInput()
    {
        // Check For Empty World Name, Set To Random Number
        if (newWorldOptions.transform.Find("WorldNameInput").GetComponent<TMP_InputField>().text == "") { GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName = UnityEngine.Random.Range(0, 1000000).ToString(); }
        else { GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName = newWorldOptions.transform.Find("WorldNameInput").GetComponent<TMP_InputField>().text; }
        
    }

    public void SetTileScaleFromInput()
    {
        GameObject.Find("DataDontDestroyOnLoad").GetComponent<WorldGenerationParameters>().tileScale = Mathf.Round(tileScale.value * 10) / 10;
        tileScale.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = (Mathf.Round(tileScale.value * 10) / 10).ToString();
    }

    public void SetLevelWidthFromInput()
    {
        GameObject.Find("DataDontDestroyOnLoad").GetComponent<WorldGenerationParameters>().worldWidth = (int)width.value;
        width.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = ((int)width.value).ToString();
    }

    public void SetLevelHeightFromInput()
    {
        GameObject.Find("DataDontDestroyOnLoad").GetComponent<WorldGenerationParameters>().worldHeight = (int)height.value;
        height.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = ((int)height.value).ToString();
    }

    public void StartNewGame()
    {
        GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().newWorld = true;
        SceneManager.LoadScene("SampleScene");
    }

    public void ShowAdvancedSettings()
    {
        // Play Animation to Show Advanced Settings
        newWorldOptions.GetComponent<Animator>().Play("Base Layer.AdvancedWorldSettingsShow");
    }
    public void HideAdvancedSettings()
    {
        // Play Animation to Show Advanced Settings
        newWorldOptions.GetComponent<Animator>().Play("Base Layer.AdvancedWorldSettingsHide");
    }

    public void BackToMain(GameObject currentMenu)
    {
        if (moving)
        {
            return;
        }
        moving = true;
        inMainMenu = true;

        mainMenu.SetActive(true);
        
        if (currentMenu == newWorldOptions) 
        {
            newWorldOptions.GetComponent<Animator>().Play("Base Layer.WorldSettingsExit");
            StartCoroutine(DisableMenuAfterX(.51f, currentMenu)); 
        }
        else if (currentMenu == selectSaveMenu) 
        { 
            currentMenu.GetComponent<Animator>().Play("Base Layer.HideLoadMenu"); 
            StartCoroutine(DisableMenuAfterX(.51f, currentMenu)); 
        }
        else if (currentMenu == optionsMenu)
        {
            currentMenu.GetComponent<Animator>().Play("Base Layer.OptionsMenuHide");
            StartCoroutine(DisableMenuAfterX(.51f, currentMenu));
        }
    }

    // Take in button, Set world to load Name to button Name (which is set from save file that it represents)
    public void LoadSavedGame(Button button)
    {
        GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().newWorld = false;
        SceneManager.LoadScene("SampleScene");
    }
    
    public void QuitGame() { UnityEngine.Debug.Log("Game Shutting Down. Good Night... "); Application.Quit(); }
}
