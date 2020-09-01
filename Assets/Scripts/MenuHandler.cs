using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public void StartNewGame()
    {
        GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().newWorld = true;
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadSavedGame()
    {
        GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>().newWorld = false;
        SceneManager.LoadScene("SampleScene");

    }
}
