using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDontDestroyOnLoad : MonoBehaviour
{
    public DataDontDestroyOnLoad instance;

    public bool newWorld;
    public bool playingMusic;

    void Start()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogError("Multiple DataDontDestroyOnLoad In Scene!!!");
        }
        instance = this;

        DontDestroyOnLoad(this);
    }
}
