using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDontDestroyOnLoad : MonoBehaviour
{
    #region Singleton

    public static DataDontDestroyOnLoad instance;
    
    void Start()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogError("Multiple DataDontDestroyOnLoad In Scene!!! Destroying " + this.name);
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

    }
    #endregion

    public bool newWorld;
    public bool playingMusic;

    public string saveName = "Lens";
}
