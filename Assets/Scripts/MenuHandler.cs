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
    public GameObject mainMenu, selectSaveMenu;

    void Start()
    {
        mainMenu = GameObject.Find("--MainMenu--");
        selectSaveMenu = GameObject.Find("--SelectSaveToLoad--"); 
        if (selectSaveMenu != null)
        {
            selectSaveMenu.SetActive(false);
        }
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
        foreach (string s in saveManager.saves)
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

    public void BackToMain()
    {
        // Get Rid Of Load Game Buttons That Are Instantiated in SelectSaveFileMenu (Garbage Cleanup)
        GameObject go = GameObject.Find("WorldSavesBackground");
        for (int i = 0; i < go.transform.childCount; i++)
        {
            Destroy(go.transform.GetChild(i).gameObject);
        }

        mainMenu.SetActive(true);
        selectSaveMenu.SetActive(false);
    }

    // Take in button, Set World To Load Name to button name
    public void LoadSavedGame(Button button)
    {
        SetSaveNameToButtonName(button);
        GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().newWorld = false;
        SceneManager.LoadScene("SampleScene");
    }

    // Take in Load World Button and Return World Name from Text
    public void SetSaveNameToButtonName(Button button)
    {
        GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().saveName = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
    }
}
