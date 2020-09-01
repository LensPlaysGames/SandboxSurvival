using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
}
