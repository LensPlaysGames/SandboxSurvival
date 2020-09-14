using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace U_Grow
{
    public class SaveManager : MonoBehaviour
    {
        #region Singleton/Init

        public static SaveManager instance;

        void Awake()
        {
            if (instance != null)
            {
                UnityEngine.Debug.LogError("MULTIPLE SaveManagers IN SCENE. Destroying " + this.name);
                Destroy(this);
            }
            else
            {
                instance = this;
                GlobalReferences.saveManager = instance;
                DontDestroyOnLoad(instance);
            }
        }

        #endregion

        public AllData dataToSave;
        public AllData loadedData;

        public void SetLevelSaveData(string name, LevelSaveData SAVETHISLEVEL)
        {
            UnityEngine.Debug.Log("Setting World Data to Save");
            dataToSave.levelsSaved[SAVETHISLEVEL.levelIndex] = SAVETHISLEVEL;
            SaveAllDataToDisk(name, dataToSave);
        }
        public void SetPlayerDataSaveData(string name, PlayerSaveData playerData)
        {
            UnityEngine.Debug.Log("Saving All Player Data");
            dataToSave.playerData = playerData;
            SaveAllDataToDisk(name, dataToSave);
        }



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

            string savePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "world_" + name + ".dat";

            if (File.Exists(savePath))
            {
                UnityEngine.Debug.Log("Save File Exists! Attempting to Load From " + savePath);

                FileStream file = new FileStream(savePath, FileMode.Open);

                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    loadedData = (AllData)formatter.Deserialize(file);
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



        // Initialize All Save Files to Reference
        public string[] worldSaves;
        public void GetSaveFiles()
        {
            worldSaves = Directory.GetFiles(Application.persistentDataPath.ToString(), "*.dat");
            foreach (string worldSave in worldSaves)
            {
                UnityEngine.Debug.Log("World Save Found on Disk! Exists. Name: " + Path.GetFileName(worldSave));
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
        }
    }
}
