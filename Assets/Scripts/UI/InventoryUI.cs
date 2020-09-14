using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace U_Grow
{
    public class InventoryUI : MonoBehaviour
    {
        #region Singleton/Init

        public static InventoryUI instance;

        void Awake()
        {
            if (instance != null) { UnityEngine.Debug.LogError("Multiple Inventory UIs!!! What the heck is going on???"); }
            instance = this;
            GameReferences.playerInvUI = instance;
        }

        #endregion

        [SerializeField]
        private Inventory inventory;
        [SerializeField]
        private GameObject selector;

        [SerializeField]
        private List<Transform> inventorySlotParents = new List<Transform>();
        [SerializeField]
        private List<Transform> inventorySlotCountTexts = new List<Transform>();

        private GameObject empty;

        private int a = 0;

        void Start()
        {
            // Prefab for UI image
            empty = Resources.Load<GameObject>("Prefabs/EmptyImagePrefab");

            inventory = GameReferences.playerInv;

            for (int slot = 0; slot < inventory.slots.Length; slot++) // Foreach Slot but FASTERRRR
            {
                a++; // So that Index is converted to my DUMBASS naming scheme that starts at "(1)"
                Transform slotParent = GameReferences.playerInvUI.transform.Find("InventoryBackground").transform.Find("Slot (" + a + ")");
                inventorySlotParents.Insert(slot, slotParent);
                inventorySlotCountTexts.Insert(slot, slotParent.transform.Find("CountTextPanel (" + a + ")").transform.Find("Slot (" + a + ") Count"));
            }

            // Assign Event Listeners from Inventory Script on Player
            inventory.updateSlotCallback += UpdateSlotUI;
            inventory.updateAllSlotsCallback += UpdateSlotsUI;
            inventory.updateSelectorUI += MoveSelector;
        }

        void MoveSelector(int slotIndex)
        {
            selector.transform.position = inventorySlotParents[slotIndex].position;
        }

        void UpdateSlotUI(int slotNum)
        {
            Slot slot = inventory.slots[slotNum];

            // If Image Exists In Slot, Destroy It, We're about to Update it
            if (inventorySlotParents[slotNum].Find("EmptyImagePrefab(Clone)") != null)
            {
                Destroy(inventorySlotParents[slotNum].Find("EmptyImagePrefab(Clone)").gameObject);
            }

            // If slot is populated, Update text
            if (slot.count != 0)
            {
                GameObject item = Instantiate(empty, inventorySlotParents[slotNum]);
                item.transform.SetSiblingIndex(0);
                item.GetComponent<Image>().sprite = GlobalReferences.DDDOL.spriteDB[(int)slot.item.tileType];
                inventorySlotCountTexts[slotNum].GetComponent<TextMeshProUGUI>().text = slot.count.ToString();
                inventorySlotCountTexts[slotNum].transform.localPosition = Vector3.zero;
            }
            else // NO MORE ITEMS IN SLOT, REMOVE ATTRIBUTES
            {
                inventorySlotCountTexts[slotNum].GetComponent<TextMeshProUGUI>().text = "";
            }
        }

        void UpdateSlotsUI()
        {
            for (int slotNum = 0; slotNum < inventory.slots.Length; slotNum++)
            {
                UpdateSlotUI(slotNum);
            }
        }
    }
}