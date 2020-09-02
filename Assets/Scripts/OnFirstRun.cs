using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFirstRun : MonoBehaviour
{
    public static OnFirstRun instance;

    public int firstRun = 0;
    void Start()
    {
        if (instance != null) { UnityEngine.Debug.LogError("Multiple ONFIRSTRUNs in Game. Destroying " + this.name); Destroy(this); }
        else { instance = this; DontDestroyOnLoad(this); }

        firstRun = PlayerPrefs.GetInt("cachedFirstRun");

        if (firstRun == 0) 
        {
            UnityEngine.Debug.Log("First Time Game has Ran on " + System.Environment.UserName);

            #region Insert Pioneer World to Save Location

            string pioneerWorldPath = Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "World_PioneerWorld.map";
            string pioneerInventoryPath = Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "inventory_PioneerWorld.dat";
            string saveFileLocation = Application.persistentDataPath + Path.DirectorySeparatorChar;

            File.Copy(pioneerWorldPath, saveFileLocation + Path.GetFileName(pioneerWorldPath), true);
            File.Copy(pioneerInventoryPath, saveFileLocation + Path.GetFileName(pioneerInventoryPath), true);

            #endregion

            firstRun = 1;
            PlayerPrefs.SetInt("cachedFirstRun", firstRun);
        }
        else
        {
            firstRun++;
            PlayerPrefs.SetInt("cachedFirstRun", firstRun);
            UnityEngine.Debug.Log("The Game Has Ran " + firstRun + " Times");
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Debug_Reset"))
        {
            UnityEngine.Debug.Log("Debug_Reset");
            PlayerPrefs.SetInt("cachedFirstRun", 0);
        }
    }
}
