using TMPro;
using UnityEngine;

namespace U_Grow
{
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
                Debug.Log("Can not Get Application Version");
            }
        }

        void SetText(string str)
        {
            vText.text = str;
        }
    }
}