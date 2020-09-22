using System.Collections;
using UnityEngine;

namespace LensorRadii.U_Grow
{
    public class AutoSave : MonoBehaviour
    {
        public int interval;
        private void Start()
        {
            StartCoroutine(Co_AutoSave(interval));
        }

        public IEnumerator Co_AutoSave(int saveInterval)
        {
            Debug.Log("Starting Auto-Save");

            while (enabled)
            {
                yield return new WaitForSeconds(saveInterval);
                Debug.Log("Auto-Saving");

                GameReferences.player.GetComponent<Player>().SaveAllPlayerData(GlobalReferences.DDDOL.saveName);
                GameReferences.levelGenerator.GetLevelInstance().SaveLevel(GlobalReferences.DDDOL.saveName);

                GameReferences.uIHandler.SendNotif("Auto-Save Complete", 5f, Color.green);
            }
        }
    }
}