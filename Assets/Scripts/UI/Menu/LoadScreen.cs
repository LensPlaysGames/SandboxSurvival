using UnityEngine;

namespace LensorRadii.U_Grow
{
    public class LoadScreen : MonoBehaviour
    {
        public static LoadScreen instance;

        void Start()
        {
            if (instance != null)
            {
                UnityEngine.Debug.Log("Multiple Loading Scenes, Destroying Extra");
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                GlobalReferences.loadScreen = instance.gameObject;
                DontDestroyOnLoad(gameObject);
            }
        }
    }

}