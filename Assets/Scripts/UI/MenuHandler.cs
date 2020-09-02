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
    public GameObject mainMenu, selectSaveMenu, newWorldOptions;

    void Start()
    {
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

    }

    public void GoToEnterWorldName()
    {
        mainMenu.SetActive(false);
        newWorldOptions.SetActive(true);
    }

    public void SetWorldNameFromInput()
    {
        GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName = newWorldOptions.transform.Find("WorldNameInput").GetComponent<TMP_InputField>().text;
    }

    public void StartNewGame()
    {
        GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().newWorld = true;
        SceneManager.LoadScene("SampleScene");
    }

    public void SelectSaveFileMenu()
    {
        // Get Reference to Load Save Button Prefab
        GameObject loadWorldButton = Resources.Load<GameObject>("LoadWorldButton");

        // Find all Save Files in Save Directory
        SaveManager saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        saveManager.GetSaveFiles();

        // (Update Visuals) Set Menu GameObjects Active State
        mainMenu.SetActive(false);
        selectSaveMenu.SetActive(true);

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

    public void BackToMain(GameObject currentMenu)
    {
        mainMenu.SetActive(true);
        currentMenu.SetActive(false);
    }

    // Take in button, Set world to load Name to button Name
    public void LoadSavedGame(Button button)
    {
        GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().newWorld = false;
        SceneManager.LoadScene("SampleScene");
    }
    
    public void QuitGame() { UnityEngine.Debug.Log("Game Shutting Down. Good Night... "); Application.Quit(); }
}
