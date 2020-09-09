﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class InventoryUI : MonoBehaviour // I need to add a reference to each slot (basically what I had in Inventory Start() but instead of slots it's transforms and stuff likea dat) AND THEN ACTUALLY USE THEM
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
    [SerializeField]
    private Sprite[] tileSprites;

    private GameObject empty;

    private int a = 0;

    void Start()
    {
        // Prefab for UI image
        empty = Resources.Load<GameObject>("Prefabs/EmptyImagePrefab");

        tileSprites = GlobalReferences.DDDOL.spriteDB;

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
        selector.transform.position = inventorySlotParents[slotIndex].transform.position;
    }

    void UpdateSlotUI(int slotNum)
    {
        Slot slot = inventory.slots[slotNum];

        // If Image Exists In Slot, Destroy It, We're about to Update it
        if (inventorySlotParents[slotNum].transform.Find("EmptyImagePrefab(Clone)") != null)
        {
            Destroy(inventorySlotParents[slotNum].transform.Find("EmptyImagePrefab(Clone)").gameObject);
        }

        // If slot is populated, Update text
        if (slot.count != 0)
        {
            GameObject item = Instantiate(empty, inventorySlotParents[slotNum].transform);
            item.transform.SetSiblingIndex(0);
            item.GetComponent<Image>().sprite = tileSprites[(int)slot.item.tileType];
            inventorySlotCountTexts[slotNum].GetComponent<TextMeshProUGUI>().text = slot.count.ToString();
            inventorySlotCountTexts[slotNum].transform.localPosition = Vector3.zero;
        }
        else // NO MORE ITEMS IN SLOT, DESTROY IMAGE, REMOVE ATTRIBUTES
        {
            if (inventorySlotParents[slotNum].transform.Find("EmptyImagePrefab(Clone)") != null)
            {
                Destroy(inventorySlotParents[slotNum].transform.Find("EmptyImagePrefab(Clone)").gameObject);
            }

            slot.item.tileType = Tile.TileType.Air;
            slot.item.itemType = Item.ItemType.Tile;
            slot.empty = true;
            slot.count = 0;
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