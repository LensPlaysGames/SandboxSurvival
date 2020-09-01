using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFirstRun : MonoBehaviour
{
    int firstRun = 0;
    void Start()
    {
        firstRun = PlayerPrefs.GetInt("cachedFirstRun");

        if (firstRun == 0) 
        {
            UnityEngine.Debug.Log("First Time Game has Ran on " + System.Environment.UserName);

            #region Insert Pioneer World to Save Location

            string pioneerWorldPath = Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "World_PioneerWorld.map";
            string saveFileLocation = Application.persistentDataPath + Path.DirectorySeparatorChar;

            File.Copy(pioneerWorldPath, saveFileLocation + Path.GetFileName(pioneerWorldPath), true);

            #endregion

            firstRun = 1;
            PlayerPrefs.SetInt("cachedFirstRun", firstRun);
        }
        else
        {
            firstRun++;
            PlayerPrefs.SetInt("cachedFirstRun", firstRun);
            UnityEngine.Debug.Log("This is the " + firstRun + " Time the Game Has Ran");
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Debug_Reset"))
        {
            PlayerPrefs.SetInt("cachedFirstRun", 0);
        }
    }
}
