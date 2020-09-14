using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace U_Grow
{
    public class CraftUI : MonoBehaviour
    {
        #region Singleton/Init

        public static CraftUI instance;

        void Awake()
        {
            if (instance != null)
            {
                UnityEngine.Debug.LogError("Multiple Crafting UIs, :|");
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
                GameReferences.craftUI = instance;
            }
        }

        #endregion

        private CraftSystem craftSystem;

        [SerializeField]
        private List<Transform> recipeSlots;
        [SerializeField]
        private List<TextMeshProUGUI> countTexts;

        [SerializeField]
        private Transform outputSlot;
        [SerializeField]
        private TextMeshProUGUI outputText;

        private GameObject empty;

        int a = 0;
        void Start()
        {
            empty = Resources.Load<GameObject>("Prefabs/EmptyImagePrefab");

            craftSystem = GameReferences.craftSystem;

            Transform BGContainer = transform.Find("CraftMenuBackground");
            for (int s = 0; s < CraftSystem.numberOfRecipeSlots; s++)
            {
                a++;
                Transform recipeSlotTransform = BGContainer.Find("Recipe Slot (" + (s + 1) + ")");
                recipeSlots.Insert(s, recipeSlotTransform);
                countTexts.Insert(s, recipeSlotTransform.Find("CountTextPanel").Find("Slot Count").GetComponent<TextMeshProUGUI>());
            }

            outputSlot = BGContainer.Find("Output Slot");
            outputText = outputSlot.Find("CountTextPanel").Find("Slot Count").GetComponent<TextMeshProUGUI>();

            craftSystem.updateRecipeSlotUI += UpdateRecipeSlot;
            craftSystem.updateAllRecipeSlots += UpdateAllRecipeSlots;
            craftSystem.updateOutputSlotUI += UpdateOutputSlot;
        }

        void UpdateRecipeSlot(int slotIndex)
        {

            if (recipeSlots[slotIndex] != null)
            {
                UnityEngine.Debug.Log("Updating Recipe Slot UI");

                Slot slotData = craftSystem.recipeSlots[slotIndex];

                if (recipeSlots[slotIndex].Find("EmptyImagePrefab(Clone)")?.gameObject != null)
                {
                    Destroy(recipeSlots[slotIndex].Find("EmptyImagePrefab(Clone)").gameObject);
                }

                if (slotData.count != 0) // Something in Slot, Update UI
                {
                    GameObject image = Instantiate(empty, recipeSlots[slotIndex]);
                    image.transform.SetSiblingIndex(0);
                    image.GetComponent<Image>().sprite = GlobalReferences.DDDOL.spriteDB[(int)slotData.item.tileType];
                    countTexts[slotIndex].text = slotData.count.ToString();
                    countTexts[slotIndex].transform.localPosition = Vector3.zero;
                }
                else // Slot is Empty, Destroy any Visual there
                {
                    countTexts[slotIndex].text = "";
                }
            }
        }

        void UpdateAllRecipeSlots()
        {
            for (int i = 0; i < recipeSlots.Count; i++)
            {
                UpdateRecipeSlot(i);
            }
        }

        void UpdateOutputSlot()
        {
            if (outputSlot != null)
            {
                Slot slotData = GameReferences.craftSystem.outputSlot;

                UnityEngine.Debug.Log("Updating Output Slot UI");

                if (outputSlot.Find("EmptyImagePrefab(Clone)")?.gameObject != null)
                {
                    Destroy(outputSlot.Find("EmptyImagePrefab(Clone)").gameObject);
                }

                if (slotData.count != 0)
                {
                    GameObject img = Instantiate(empty, outputSlot);
                    img.transform.SetSiblingIndex(0);
                    img.GetComponent<Image>().sprite = GlobalReferences.DDDOL.spriteDB[(int)slotData.item.tileType];
                    outputText.text = slotData.count.ToString();
                    outputText.transform.localPosition = Vector3.zero;
                }
                else
                {
                    outputText.text = "";
                }
            }
        }
    }
}