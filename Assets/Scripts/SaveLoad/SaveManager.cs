using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace U_Grow
{
    public class SaveManager : MonoBehaviour
    {
        private static string saveDirectory;

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

            saveDirectory =
              Application.persistentDataPath +
              Path.DirectorySeparatorChar +
              "Saves";

            if (!Directory.Exists(saveDirectory)) { Directory.CreateDirectory(saveDirectory); }
        }

        #endregion

        public AllData dataToSave;
        public AllData loadedData;

        public void SetLevelSaveData(string name, LevelSaveData SAVETHISLEVEL)
        {
            Debug.Log("Setting World Data to Save");
            dataToSave.levelsSaved[SAVETHISLEVEL.levelIndex] = SAVETHISLEVEL;
            SaveAllDataToDisk(name, dataToSave);
        }
        public void SetPlayerDataSaveData(string name, PlayerSaveData playerData)
        {
            Debug.Log("Saving All Player Data");
            dataToSave.playerData = playerData;
            SaveAllDataToDisk(name, dataToSave);
        }



        public void SaveAllDataToDisk(string name, AllData data)
        {
            string saveName = "world_" + name + ".dat";
            string savePath = saveDirectory + Path.DirectorySeparatorChar + saveName;

            Debug.Log("Saving All Data in World: " + saveName);

            FileStream file = new FileStream(savePath, FileMode.OpenOrCreate);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(file, data);
                Debug.Log("Saved World Data!");
            }
            catch (SerializationException e)
            {
                Debug.LogError("Issue Serializing World Data: " + e.Message);
            }
            finally
            {
                file.Close();
            }
        }

        public void LoadAllDataFromDisk(string name)
        {
            Debug.Log("Loading World Data!");

            string saveName = "world_" + name + ".dat";
            string savePath = saveDirectory + Path.DirectorySeparatorChar + saveName;

            if (File.Exists(savePath))
            {
                Debug.Log($"Save File {saveName} Exists! \nAttempting to Load From {savePath}");

                FileStream file = new FileStream(savePath, FileMode.Open);

                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    loadedData = (AllData)formatter.Deserialize(file);
                    Debug.Log("Loaded World Data!");
                }
                catch (SerializationException e)
                {
                    Debug.LogError("Issue Deserializing World Data: " + e.Message);
                }
                finally
                {
                    file.Close();
                }
            }
            else
            {
                Debug.Log("Save File NULL at path: " + savePath);
                SceneManager.LoadScene("Menu");
            }
        }



        // Initialize All Save Files to Reference
        public string[] worldSaves;
        public void GetSaveFiles()
        {
            worldSaves = Directory.GetFiles(saveDirectory.ToString(), "*.dat");
            foreach (string worldSave in worldSaves)
            {
                Debug.Log("World Save Found on Disk! Exists. Name: " + Path.GetFileName(worldSave));
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
                    Debug.Log("DELETING WORLD SAVE AT " + s);
                    File.Delete(s);
                }
            }
        }
    }
}
