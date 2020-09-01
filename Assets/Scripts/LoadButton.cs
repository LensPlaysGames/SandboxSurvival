using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadButton : MonoBehaviour
{
    public GameObject canvas;

    void Start()
    {
        canvas = GameObject.Find("Canvas");
    }

    public void LoadSave()
    {
        canvas.GetComponent<MenuHandler>().LoadSavedGame(this.GetComponent<Button>());
    }

    public void DeleteSave()
    {
        UnityEngine.Debug.Log("Deleting Save " + this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
        SaveManager saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        saveManager.DeleteSaveFile(this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
        Destroy(this.gameObject);
    }
}
