using UnityEngine;

namespace LensorRadii.U_Grow
{
    public class BackToMain : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetButtonDown("Cancel")) { GameObject.Find("Canvas").GetComponent<MenuHandler>().BackToMain(gameObject); }
        }
    }
}