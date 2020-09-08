using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;
    public static Inventory inventory;

    public Slot mousedOver;

    private GameObject empty;

    void Start()
    {
        inventory = GameObject.Find("Player").GetComponent<Inventory>();
        // inventory = Inventory.instance; // THIS DOESN"T FUCKING WORK BUT IT SHOULD
        inventory.updateSlotCallback += UpdateSlotUI;
        inventory.updateAllSlotsCallback += UpdateSlotsUI;

        empty = Resources.Load<GameObject>("Prefabs/EmptyImagePrefab");
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
