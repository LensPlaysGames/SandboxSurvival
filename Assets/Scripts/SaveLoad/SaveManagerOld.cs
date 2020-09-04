using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManagerOld : MonoBehaviour
{

    #region Singleton

    public static SaveManagerOld instance;

    void Start()
    {
        if (instance != null) { UnityEngine.Debug.LogError("MULTIPLE SaveManagers IN SCENE. Destroying " + this.name); Destroy(this); }
        else { instance = this; DontDestroyOnLoad(instance); }
    }

    #endregion

    public Tile[] loadedTiles;
    public World loadedWorld;
    public Slot[] loadedSlots;
    public PlayerSaveData loadedPlayerData;

    public void SaveAllDataToDisk(string name, AllData data)
    {
        FileStream file = new FileStream(Application.persistentDataPath + Path.DirectorySeparatorChar + "world_" + name + ".dat", FileMode.OpenOrCreate);

        UnityEngine.Debug.Log("Saving All Data in World: " + "world_" + name + ".dat");

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(file, data);
            UnityEngine.Debug.Log("Saved World Data!");
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

    public void LoadAllDataFromDisk(string name)
    {
        UnityEngine.Debug.Log("Loading World Data!");

        string savePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "world_" + name + ".map";

        if (File.Exists(savePath))
        {
            UnityEngine.Debug.Log("Save File Exists! Attempting to Load From " + savePath);

            FileStream file = new FileStream(savePath, FileMode.Open);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                loadedTiles = (Tile[])formatter.Deserialize(file);
                UnityEngine.Debug.Log("Loaded World Data!");
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
            UnityEngine.Debug.Log("Save File Exists! Attempting to Load From " + savePath);

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

    public void SaveInventoryDataToDisk(string name, Slot[] SAVETHESESLOTS)
    {
        FileStream file = new FileStream(Application.persistentDataPath + Path.DirectorySeparatorChar + "inventory_" + name + ".inv", FileMode.OpenOrCreate);

        UnityEngine.Debug.Log("Saving To Inventory " + "inventory_" + name + ".dat");

        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, SAVETHESESLOTS);
            UnityEngine.Debug.Log("Saved Inventory!");
        }
        catch (SerializationException e)
        {
            UnityEngine.Debug.LogError("Issue Serializing Inventory Data: " + e.Message);
        }
        finally
        {
            file.Close();
        }
    }

    public void LoadInventoryDataFromDisk(string name)
    {
        string savePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "inventory_" + name + ".inv";

        if (File.Exists(savePath))
        {
            UnityEngine.Debug.Log("Inventory Save Exists At " + savePath + "   ATTEMPTING TO LOAD   ");

            FileStream file = new FileStream(savePath, FileMode.Open);

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                loadedSlots = (Slot[])bf.Deserialize(file);
                UnityEngine.Debug.Log("Loaded Inventory!");
            }
            catch (SerializationException e)
            {
                UnityEngine.Debug.Log("Error Loading Inventory Data: " + e.Message);
            }
            finally
            {
                file.Close();
            }
        }
        
    }

    public void SavePlayerDataToDisk(string name, PlayerSaveData playerData)
    {
        FileStream file = new FileStream(Application.persistentDataPath + Path.DirectorySeparatorChar + "player_" + name + ".dat", FileMode.OpenOrCreate);

        UnityEngine.Debug.Log("Saving Player Data: " + "player_" + name + ".dat");

        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, playerData);
            UnityEngine.Debug.Log("Saved Player Data!");
        }
        catch (SerializationException e)
        {
            UnityEngine.Debug.LogError("Issue Serializing Player Data: " + e.Message);
        }
        finally
        {
            file.Close();
        }
    }

    public void LoadPlayerDataFromDisk(string name)
    {
        string savePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "player_" + name + ".dat";

        if (File.Exists(savePath))
        {
            UnityEngine.Debug.Log("Player Save Exists At " + savePath + "   ATTEMPTING TO LOAD   ");

            FileStream file = new FileStream(savePath, FileMode.Open);

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                loadedPlayerData = (PlayerSaveData)bf.Deserialize(file);
                UnityEngine.Debug.Log("Loaded Player Data!");
            }
            catch (SerializationException e)
            {
                UnityEngine.Debug.Log("Error Loading Player Data: " + e.Message);
            }
            finally
            {
                file.Close();
            }
        }

    }



    public string[] worldSaves, inventorySaves, playerDataSaves;

    // Initialize All Save Files to Reference
    public void GetSaveFiles()
    {
        worldSaves = Directory.GetFiles(Application.persistentDataPath.ToString(), "*.map");
        foreach (string worldSave in worldSaves)
        {         
            UnityEngine.Debug.Log("World Save Found on Disk! Exists. Name: " + Path.GetFileName(worldSave));
        }

        inventorySaves = Directory.GetFiles(Application.persistentDataPath.ToString(), "*.inv");
        foreach (string inventorySave in inventorySaves)
        {
            UnityEngine.Debug.Log("Inventory Save Found on Disk! Name: " + Path.GetFileName(inventorySave));
        }

        playerDataSaves = Directory.GetFiles(Application.persistentDataPath.ToString(), "*.dat");
        foreach (string playerDataSave in playerDataSaves)
        {
            UnityEngine.Debug.Log("Player Save Found on Disk! Name: " + Path.GetFileName(playerDataSave));
        }
    }

    public void DeleteSaveFile(string name)
    {
        GetSaveFiles();
        foreach (string s in worldSaves)
        {
            string saveFileName = Path.GetFileName(s);
            string saveName = saveFileName.Substring(saveFileName.IndexOf("_") + 1);
            int index = saveName.LastIndexOf(".");
            if (index > 0) { saveName = saveName.Substring(0, index); }

            if (saveName == name) // Found Save to Delete
            {
                UnityEngine.Debug.Log("DELETING WORLD SAVE AT " + s);
                File.Delete(s);
            }
        }

        foreach (string s in inventorySaves)
        {
            string saveFileName = Path.GetFileName(s);
            string saveName = saveFileName.Substring(saveFileName.IndexOf("_") + 1);
            int index = saveName.LastIndexOf(".");
            if (index > 0) { saveName = saveName.Substring(0, index); }

            if (saveName == name) // Found Save to Delete
            {
                UnityEngine.Debug.Log("DELETING INVENTORY SAVE AT " + s);
                File.Delete(s);
            }
        }

        foreach (string s in playerDataSaves)
        {
            string saveFileName = Path.GetFileName(s);
            string saveName = saveFileName.Substring(saveFileName.IndexOf("_") + 1);
            int index = saveName.LastIndexOf(".");
            if (index > 0) { saveName = saveName.Substring(0, index); }

            if (saveName == name) // Found Save to Delete
            {
                UnityEngine.Debug.Log("DELETING PLAYER DATA SAVE AT " + s);
                File.Delete(s);
            }
        }
    }
}
