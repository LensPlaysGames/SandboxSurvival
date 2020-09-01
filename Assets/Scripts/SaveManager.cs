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

    public static SaveManager instance;

    void Start()
    {
        if (instance != null) { UnityEngine.Debug.LogError("MULTIPLE SaveManagers IN SCENE. Destroying " + this.name); Destroy(this); }
        else { instance = this; DontDestroyOnLoad(instance); }
    }

    #endregion

    public World loadedWorld;

    public Tile[] loadedTiles;

    public void SaveWorldDataToDisk(string name, Tile[] SAVETHESETILES)
    {
        FileStream file = new FileStream(Application.persistentDataPath + Path.DirectorySeparatorChar + "world_" + name + ".map", FileMode.OpenOrCreate);

        UnityEngine.Debug.Log("Saving To World: " + "world_" + name + ".map");

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

    public void DeleteSaveFile(string name)
    {
        GetSaveFiles();
        foreach (string s in saves)
        {
            string saveFileName = Path.GetFileName(s);
            string saveName = saveFileName.Substring(saveFileName.IndexOf("_") + 1);
            int index = saveName.LastIndexOf(".");
            if (index > 0) { saveName = saveName.Substring(0, index); }

            if (saveName == name) // Found Save to Delete
            {
                File.Delete(s);
            }
        }
    }
}
