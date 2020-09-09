using UnityEngine;

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
        if (myLog.Length > 5000)
        {
            myLog = myLog.Substring(0, 4000);
        }
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

    void Update()
    {
        if (inputManager.Debug.DebugLog.triggered)
        {
            toggle = !toggle;
        }
    }
    //#endif
}