using UnityEngine;
using TMPro;

public class GetVersion : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI vText;

    private string version;

    void Awake()
    {
        version = Application.version;

        if (version != "")
        {
            vText = GetComponent<TextMeshProUGUI>();
            if (vText != null)
            {
                SetText(version);
            }
        }
        else
        {
            UnityEngine.Debug.Log("Can not Get Application Version");
        }
    }

    void SetText(string str)
    {
        vText.text = str;
    }
}
