using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFirstRun : MonoBehaviour
{
    public static OnFirstRun instance;

    public int firstRun = 0;
    public int runs;

    void Start()
    {
        if (instance != null) { UnityEngine.Debug.LogError("Multiple ONFIRSTRUNs in Game. Destroying " + this.name); Destroy(this); }
        else { instance = this; DontDestroyOnLoad(this); }

        firstRun = PlayerPrefs.GetInt("FirstRun");
        runs = PlayerPrefs.GetInt("Runs");

        if (firstRun == 0) 
        {
            UnityEngine.Debug.Log("First Time Game has Ran on " + System.Environment.UserName);

            firstRun = 1;
            PlayerPrefs.SetInt("FirstRun", firstRun);

            runs++;
        }
        else
        {
            runs++;
            PlayerPrefs.SetInt("Runs", runs);
            UnityEngine.Debug.Log("The Game Has Ran " + runs + " Times");
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Debug_Reset"))
        {
            UnityEngine.Debug.Log("Debug_Reset");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("FirstRun", 0);
            PlayerPrefs.SetInt("Runs", 0);
        }
    }
}
