using System;
using System.IO;
using UnityEngine;

namespace U_Grow
{
    public class debugtest : MonoBehaviour
    {
        //#if !UNITY_EDITOR
        static string myLog = "";
        private string output;
        private string stack;

        private bool toggle;

        private InputManager inputManager;

        void Awake()
        {
            inputManager = new InputManager();
        }

        void OnEnable()
        {
            Application.logMessageReceived += Log;

            inputManager.Debug.Enable();
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;

            inputManager.Debug.Disable();
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            myLog = output + "\n" + myLog;
            if (myLog.Length > 10000)
            {
                myLog = myLog.Substring(0, 9000);
            }

            SaveString(myLog);
        }

        void OnGUI()
        {
            //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
            {
                if (toggle)
                {
                    myLog = GUI.TextArea(new Rect(10, 10, Screen.width - 10, Screen.height - 480), myLog);
                }
            }
        }

        void SaveString(string str)
        {
            string directory = Application.persistentDataPath + Path.DirectorySeparatorChar + "Logs";
            if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

            str = "\n" + 
                "Grow! Debug Log" + "\n" + 
                "Version: " + Application.version + "\n" + 
                "Date" + DateTime.Now + "\n\n" +
                str;

            string path = directory + Path.DirectorySeparatorChar + $"Grow_debuglog_{Application.version}.txt";
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);

            try
            {
                StreamWriter streamWriter = new StreamWriter(stream);
                streamWriter.Write(str);
            }
            catch (Exception e)
            {
                Debug.LogError("Error While Saving Debug Log: " + e.Message);
            }
            finally
            {
                stream.Close();
            }
        }
        void Update()
        {
            if (inputManager.Debug.DebugLog.triggered)
            {
                toggle = !toggle;
            }
        }
        //#endif
    }
}