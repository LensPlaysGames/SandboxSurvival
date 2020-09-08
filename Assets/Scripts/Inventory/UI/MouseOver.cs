using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOver : MonoBehaviour
{
    public bool mouseOver;

    public void OnPointerEnter()
    {
        mouseOver = true;
    }

    public void OnPointerExit()
    {
        mouseOver = false;
    }
}
