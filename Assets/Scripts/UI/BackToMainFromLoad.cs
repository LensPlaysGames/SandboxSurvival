using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToMainFromLoad : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButtonDown("Cancel")) 
        {
            GameObject go = transform.Find("WorldSavesBackground").gameObject;
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Destroy(go.transform.GetChild(i).gameObject);
            }
            GameObject.Find("Canvas").GetComponent<MenuHandler>().BackToMain(this.gameObject); 
        }
    }
}
