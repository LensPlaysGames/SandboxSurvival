using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteract : MonoBehaviour, IInteracteable
{
    public void Use()
    {
        UnityEngine.Debug.Log("Whoopie, you did it, fat ass!");
    }
}
