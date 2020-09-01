using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{

    #region Singleton

    public SaveManager instance;

    void Awake()
    {
        if (instance != null) { UnityEngine.Debug.LogError("MULTIPLE SaveManagers IN SCENE"); }
        instance = this;

        DontDestroyOnLoad(instance);
    }

    #endregion

    public World loadedWorld;

    public Tile[] loadedTiles;

    public void SaveWorldDataToDisk(string name, Tile[] SAVETHESETILES)
    {
        UnityEngine.Debug.Log("Saving World!");

        FileStream file = new FileStream(Application.persistentDataPath + Path.DirectorySeparatorChar + "world_" + name + ".map", FileMode.OpenOrCreate);

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(file, SAVETHESETILES);
            UnityEngine.Debug.Log("Saved World!");
        }
        catch (SerializationException e)
        {
            UnityEngine.Debug.LogError("Issue Serializing World Data: " + e.Message);
        }
        finally
        {
            file.Close();
        }
    }

    public void LoadWorldDataFromDisk(string name)
    {
        UnityEngine.Debug.Log("Loading World!");

        string savePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "world_" + name + ".map";

        if (File.Exists(savePath)) 
        {
            UnityEngine.Debug.Log("Save File Exists! Attempting to Load...");
            UnityEngine.Debug.Log("Attempting To Load From Save File Path: " + savePath);

            FileStream file = new FileStream(savePath, FileMode.Open);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                loadedTiles = (Tile[])formatter.Deserialize(file);
                UnityEngine.Debug.Log("Loaded World!");
            }
            catch (SerializationException e)
            {
                UnityEngine.Debug.LogError("Issue Deserializing World Data: " + e.Message);
            }
            finally
            {
                file.Close();
            }
        }
        else
        {
            UnityEngine.Debug.Log("Save File NULL at path: " + savePath);
            SceneManager.LoadScene("Menu");
        }
    }

    public string[] saves;

    public void GetSaveFiles()
    {
        saves = Directory.GetFiles(Application.persistentDataPath.ToString(), "*.map");
        foreach (string save in saves)
        {
            // Initialize All Save Files so Load Save Game UI can Update Correctly
            UnityEngine.Debug.Log("Save File Exists. Name: " + Path.GetFileName(save));
        }
    }
}
