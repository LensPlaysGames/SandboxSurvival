using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToMain : MonoBehaviour
{

    void Update()
    {
        if (Input.GetButtonDown("Cancel")) { GameObject.Find("Canvas").GetComponent<MenuHandler>().BackToMain(); }
    }
}
