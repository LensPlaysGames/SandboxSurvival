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
        Slot slot = inventory.slots[slotNum];

        // If Image Exists In Slot, Destroy It, We're about to Update it
        if (slot.slotParent.transform.Find("EmptyImagePrefab(Clone)") != null)
        {
            Destroy(slot.slotParent.transform.Find("EmptyImagePrefab(Clone)").gameObject);
        }

        // If slot is populated, Update text
        if (slot.count != 0)
        {
            GameObject item = Instantiate(empty, slot.slotParent.transform);
            item.transform.SetSiblingIndex(0);
            item.GetComponent<Image>().sprite = slot.sprite;
            slot.countText.GetComponent<TextMeshProUGUI>().text = slot.count.ToString();
            slot.countText.transform.localPosition = Vector3.zero;

            UnityEngine.Debug.Log("Slot Count:  " + slot.count);
            UnityEngine.Debug.Log("Slot Count From Text:  " + slot.countText.GetComponent<TextMeshProUGUI>().text);
            UnityEngine.Debug.Log("Text Position:  " + "x:  " + slot.countText.GetComponent<TextMeshProUGUI>().transform.position.x + "    y:    " + slot.countText.GetComponent<TextMeshProUGUI>().transform.position.y);
        }
        else // NO MORE ITEMS IN SLOT, REMOVE ATTRIBUTES
        {
            if (slot.slotParent.transform.Find("EmptyImagePrefab(Clone)") != null)
            {
                Destroy(slot.slotParent.transform.Find("EmptyImagePrefab(Clone)").gameObject);
            }

            slot.item.tileType = Tile.TileType.Air;
            slot.item.itemType = Item.ItemType.Tile;
            slot.empty = true;
            slot.count = 0;
            slot.countText.GetComponent<TextMeshProUGUI>().text = "";
            slot.sprite = null;
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
