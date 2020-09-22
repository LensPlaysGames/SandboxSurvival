using UnityEngine;

namespace LensorRadii.U_Grow
{
    public class ChestUI : MonoBehaviour
    {
        public static ChestUI instance;

        private void Awake()
        {
            if (instance != null) { Debug.LogError("MULTIPLE CHEST UIs IN SCENE"); Destroy(this); }
            else
            {
                instance = this;
                GameReferences.chestUI = instance;
            }

            if (GameReferences.chestUI != null)
            {
                gameObject.SetActive(false);
            }
        }
    }
}