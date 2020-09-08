using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private GameObject selector;

    private GameObject empty;

    void Awake()
    {
        if (instance != null) { UnityEngine.Debug.LogError("Multiple Inventory UIs!!! What the heck is going on???"); }
        instance = this;
        GameReferences.playerInvUI = instance;
    }

    void Start()
    {
        // Prefab for UI image
        empty = Resources.Load<GameObject>("Prefabs/EmptyImagePrefab");

        inventory = GameReferences.playerInv;

        // Assign Event Listeners from Inventory Script on Player
        inventory.updateSlotCallback += UpdateSlotUI;
        inventory.updateAllSlotsCallback += UpdateSlotsUI;
        inventory.updateSelectorUI += MoveSelector;
    }

    void MoveSelector(int slotIndex)
    {
        selector.transform.position = inventory.slots[slotIndex].slotParent.transform.position;
    }

    void UpdateSlotUI(int slotNum)
    {
        // If Image Exists In Slot, Destroy It, We're about to Update it
        if (inventory.slots[slotNum].slotParent.transform.Find("EmptyImagePrefab(Clone)") != null)
        {
            Destroy(inventory.slots[slotNum].slotParent.transform.Find("EmptyImagePrefab(Clone)").gameObject);
        }

        // If slot is populated, Update text
        if (inventory.slots[slotNum].count != 0)
        {
            GameObject item = Instantiate(empty, inventory.slots[slotNum].slotParent.transform);
            item.transform.SetSiblingIndex(item.transform.GetSiblingIndex() - 1);
            item.GetComponent<Image>().sprite = inventory.slots[slotNum].sprite;
            inventory.slots[slotNum].countText.GetComponent<TextMeshProUGUI>().text = inventory.slots[slotNum].count.ToString();
        }
        else // NO MORE ITEMS IN SLOT, REMOVE ATTRIBUTES
        {
            if (inventory.slots[slotNum].slotParent.transform.Find("EmptyImagePrefab(Clone)") != null)
            {
                Destroy(inventory.slots[slotNum].slotParent.transform.Find("EmptyImagePrefab(Clone)").gameObject);
            }

            inventory.slots[slotNum].item.tileType = Tile.TileType.Air;
            inventory.slots[slotNum].item.itemType = Item.ItemType.Tile;
            inventory.slots[slotNum].empty = true;
            inventory.slots[slotNum].count = 0;
            inventory.slots[slotNum].countText.GetComponent<TextMeshProUGUI>().text = "";
            inventory.slots[slotNum].sprite = null;
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
