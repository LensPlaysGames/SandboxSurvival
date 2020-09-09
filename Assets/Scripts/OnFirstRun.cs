using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFirstRun : MonoBehaviour
{
    public static OnFirstRun instance;

    public int firstRun = 0;
    public int runs;

    private InputManager inputManager;

    void Awake()
    {
        if (instance != null) 
        { 
            UnityEngine.Debug.LogError("Multiple ONFIRSTRUNs in Game. Destroying " + this.name); 
            Destroy(this.gameObject); 
        }
        else 
        { 
            instance = this;
            GlobalReferences.firstRun = instance;
            DontDestroyOnLoad(this); 
        }

        inputManager = new InputManager();
    }

    void OnEnable()
    {
        inputManager.Debug.Enable();
    }

    void OnDisable()
    {
        inputManager.Debug.Disable();
    }

    void Start()
    {
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
        if (inputManager.Debug.DebugReset.triggered)
        {
            UnityEngine.Debug.Log("Debug_Reset");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("FirstRun", 0);
            PlayerPrefs.SetInt("Runs", 0);
        }
    }
}
