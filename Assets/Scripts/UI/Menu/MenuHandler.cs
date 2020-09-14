using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace U_Grow
{
    public class MenuHandler : MonoBehaviour
    {
        public static MenuHandler instance;

        public GameObject mainMenu, selectSaveMenu, newWorldOptions, optionsMenu, loadScreen;

        public LevelGenerationParameters levelGenParams;
        public Slider tileScale, width, height, masterVolume, musicVolume, sfxVolume;
        public Toggle lockCursorPos, displayCoordinates;

        private bool moving, inMainMenu;

        void Awake()
        {
            if (instance != null)
            {
                UnityEngine.Debug.LogError("MULTIPLE MENUHANDLERS. ABORTING THIS ONE BECAUSE ITS A ROTTEN ROTTEN CHILD");
                Destroy(this);
            }
            else
            {
                instance = this;
                GlobalReferences.menuHandler = instance;
            }
        }

        void Start()
        {
            inMainMenu = true;
            mainMenu = transform.Find("--MainMenu--").gameObject;

            selectSaveMenu = transform.Find("--SelectSaveToLoad--").gameObject;
            if (selectSaveMenu != null)
            {
                selectSaveMenu.SetActive(false);
            }
            newWorldOptions = transform.Find("--NewWorldOptions--").gameObject;
            if (newWorldOptions != null)
            {
                newWorldOptions.SetActive(false);
            }
            optionsMenu = transform.Find("--Options--").gameObject;
            if (optionsMenu != null)
            {
                optionsMenu.SetActive(false);
            }

            loadScreen = GameObject.Find("--LoadScreen--");
            GlobalReferences.loadScreen = loadScreen;
            if (loadScreen != null)
            {
                loadScreen.transform.Find("Loading").gameObject.SetActive(false);
            }

            #region Initialize Sliders for Advanced World Options

            levelGenParams = GlobalReferences.levelGenParams;

            GameObject container = newWorldOptions.transform.Find("AdvancedWorldOptions").gameObject;
            GameObject containerT = container.transform.Find("--TileScale--").gameObject;
            tileScale = containerT.transform.Find("Slider").GetComponent<Slider>();

            GameObject containerW = container.transform.Find("--Width--").gameObject;
            width = containerW.transform.Find("WidthSlider").GetComponent<Slider>();

            GameObject containerH = container.transform.Find("--Height--").gameObject;
            height = containerH.transform.Find("HeightSlider").GetComponent<Slider>();

            tileScale.value = levelGenParams.defaultScale;
            tileScale.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = tileScale.value.ToString();
            width.value = levelGenParams.defaultWidth;
            width.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = width.value.ToString();
            height.value = levelGenParams.defaultHeight;
            height.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = height.value.ToString();

            levelGenParams.tileScale = levelGenParams.defaultScale;
            levelGenParams.worldWidth = levelGenParams.defaultWidth;
            levelGenParams.worldHeight = levelGenParams.defaultHeight;

            #endregion

            #region Initialize Options Values

            #region Audio

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

            GameObject containerTog = containerO.transform.Find("--Toggles--").gameObject;
            lockCursorPos = containerTog.transform.Find("LockCursorPos").GetComponent<Toggle>();
            int locked = PlayerPrefs.GetInt("LockCursorPos", 0);
            if (locked != 0) { lockCursorPos.isOn = true; }
            else if (locked == 0) { lockCursorPos.isOn = false; }

            displayCoordinates = containerTog.transform.Find("DisplayCoordinates").GetComponent<Toggle>();
            int displayCoords = PlayerPrefs.GetInt("Enabled Coordinates Display", 1);
            if (displayCoords != 0) { displayCoordinates.isOn = true; }
            else if (displayCoords == 0) { displayCoordinates.isOn = false; }

            #endregion

        }

        void Update()
        {
            if (!moving && inMainMenu)
            {
                if (Input.GetButtonDown("Cancel")) { UnityEngine.Debug.Log("Game Shutting Down. Good Night..."); Application.Quit(); }
            }
        }

        #region Generate World Menu

        public void GoToEnterWorldName()
        {
            moving = true;
            inMainMenu = false;
            mainMenu.GetComponent<Animator>().Play("Base Layer.MainMenuHide");
            StartCoroutine(DisableMenuAfterX(.51f, mainMenu));
            newWorldOptions.SetActive(true);
        }

        public void SetWorldNameFromInput()
        {
            // Check For Empty World Name, Set To Random Number
            if (newWorldOptions.transform.Find("WorldNameInput").GetComponent<TMP_InputField>().text == "") { DataDontDestroyOnLoad.instance.saveName = UnityEngine.Random.Range(0, 1000000).ToString(); }
            else { DataDontDestroyOnLoad.instance.saveName = newWorldOptions.transform.Find("WorldNameInput").GetComponent<TMP_InputField>().text; }

        }

        public void StartNewGame()
        {
            loadScreen.transform.Find("Loading").gameObject.SetActive(true);
            DataDontDestroyOnLoad.instance.newWorld = true;
            SceneManager.LoadScene("SampleScene");
        }

        #region Advanced World Creation Settings

        public void ShowAdvancedSettings()
        {
            // Play Animation to Show Advanced Settings
            newWorldOptions.GetComponent<Animator>().Play("Base Layer.AdvancedWorldSettingsShow");
        }

        public void SetTileScaleFromInput()
        {
            GlobalReferences.levelGenParams.tileScale = Mathf.Round(tileScale.value * 10) / 10;
            tileScale.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = (Mathf.Round(tileScale.value * 10) / 10).ToString();
        }

        public void SetLevelWidthFromInput()
        {
            GlobalReferences.levelGenParams.worldWidth = (int)width.value;
            width.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = ((int)width.value).ToString();
        }

        public void SetLevelHeightFromInput()
        {
            GlobalReferences.levelGenParams.worldHeight = (int)height.value;
            height.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = ((int)height.value).ToString();
        }

        public void HideAdvancedSettings()
        {
            // Play Animation to Show Advanced Settings
            newWorldOptions.GetComponent<Animator>().Play("Base Layer.AdvancedWorldSettingsHide");
        }

        #endregion

        #endregion

        #region Load World Menu

        public void SelectSaveFileMenu()
        {
            moving = true;
            inMainMenu = false;

            // (Update Visuals) Set Menu GameObjects Active State, Animations
            mainMenu.GetComponent<Animator>().Play("Base Layer.MainMenuHide");
            StartCoroutine(DisableMenuAfterX(.51f, mainMenu));
            selectSaveMenu.SetActive(true);

            // Get Reference to Load Save Button Prefab
            GameObject loadWorldButton = Resources.Load<GameObject>("Prefabs/LoadWorldButton");

            // Find all Save Files in Save Directory
            SaveManager saveManager = GlobalReferences.saveManager;
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

        public void LoadSavedGame(Button button)
        {
            loadScreen.transform.Find("Loading").gameObject.SetActive(true);
            DataDontDestroyOnLoad.instance.saveName = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            DataDontDestroyOnLoad.instance.newWorld = false;
            SceneManager.LoadScene("SampleScene");
        }

        #endregion

        #region Options Menu

        public void GoToOptionsMenu()
        {
            moving = true;
            inMainMenu = false;
            mainMenu.GetComponent<Animator>().Play("Base Layer.MainMenuHide");
            StartCoroutine(DisableMenuAfterX(.51f, mainMenu));
            optionsMenu.SetActive(true);
        }

        #region Set Options

        public void SetMasterVol(float vol)
        {
            PlayerPrefs.SetFloat("Master Volume", Mathf.Round(vol * 100) / 100);
            masterVolume.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = (Mathf.Round((Mathf.Pow(1 - (Mathf.Round(vol * -1) / 80), 5.807f)) * 100) / 100).ToString();
            GlobalReferences.musicManager.UpdateMixerVolumes();
        }
        public void SetMusicVol(float vol)
        {
            PlayerPrefs.SetFloat("Music Volume", Mathf.Round(vol * 100) / 100);
            musicVolume.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = (Mathf.Round((Mathf.Pow(1 - (Mathf.Round(vol * -1) / 80), 5.807f)) * 100) / 100).ToString();
            GlobalReferences.musicManager.UpdateMixerVolumes();
        }
        public void SetSfxVol(float vol)
        {
            PlayerPrefs.SetFloat("Sfx Volume", Mathf.Round(vol * 100) / 100);
            sfxVolume.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = (Mathf.Round((Mathf.Pow(1 - (Mathf.Round(vol * -1) / 80), 5.807f)) * 100) / 100).ToString();
            GlobalReferences.musicManager.UpdateMixerVolumes();
        }
        public void SetLockCursorPos(bool locked)
        {
            int lockedIndex = 1;
            if (locked) { lockedIndex = 1; }
            else if (!locked) { lockedIndex = 0; }
            PlayerPrefs.SetInt("LockCursorPos", lockedIndex);
            lockCursorPos.isOn = locked;
        }
        public void SetCoordinatesDisplay(bool enabled)
        {
            int enabledInt = 1;
            if (enabled) { enabledInt = 1; }
            else if (!enabled) { enabledInt = 0; }
            PlayerPrefs.SetInt("Enabled Coordinates Display", enabledInt);
        }

        #endregion

        #endregion

        public IEnumerator DisableMenuAfterX(float x, GameObject menu)
        {
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

        public void QuitGame() { UnityEngine.Debug.Log("Game Shutting Down. Good Night... "); Application.Quit(); }
    }
}