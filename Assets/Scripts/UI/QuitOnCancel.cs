using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitOnCancel : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButtonDown("Cancel")) { UnityEngine.Debug.Log("Game Shutting Down. Good Night..."); Application.Quit(); }
    }
}
