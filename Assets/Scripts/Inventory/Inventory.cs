using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    #region Singleton/Init

    public static Inventory instance;
    
    void Awake()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogError("Multiple Inventories attempting to Initialize");
        }
        instance = this;
        GameReferences.playerInv = instance;
    }

    #endregion

    public bool inventoryLoaded = false;
    public bool spritesLoaded = false;

    public int selectedSlotIndex;
    public Slot selectedSlot;

    public int maxStackSize = 90;

    public Slot[] slots;

    public Slot[] slotsToSave;
    private int a = 0;

    #region UI Event Declaration

    public delegate void UpdateUIEvent(int slotNum);
    public UpdateUIEvent updateSelectorUI;
    public UpdateUIEvent updateSlotCallback;

    public delegate void UpdateAllUIEvent();
    public UpdateAllUIEvent updateAllSlotsCallback;

    #endregion


    void Start()
    {
        // Initialize Inventory if Not Loaded
        if (!inventoryLoaded)
        {
            a = 0;
            foreach (Slot slot in slots)
            {
                a++;
                slot.item.itemType = Item.ItemType.Tile;
                slot.item.tileType = Tile.TileType.Air;
                slot.empty = true;
                slot.count = 0;
            }
        }

        // Set Selected Slot to 0 (First in Inv) if Null
        if (selectedSlot == null)
        {
            SetSelectedSlot(0);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { SetSelectedSlot(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { SetSelectedSlot(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { SetSelectedSlot(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { SetSelectedSlot(3); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { SetSelectedSlot(4); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { SetSelectedSlot(5); }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { SetSelectedSlot(6); }
        if (Input.GetKeyDown(KeyCode.Alpha8)) { SetSelectedSlot(7); }
        if (Input.GetKeyDown(KeyCode.Alpha9)) { SetSelectedSlot(8); }
        if (Input.GetKeyDown(KeyCode.Alpha0)) { SetSelectedSlot(9); }

        if (Input.mouseScrollDelta.y > 0 && selectedSlotIndex != 0) { SetSelectedSlot(selectedSlotIndex - 1); }
        else if (Input.mouseScrollDelta.y > 0 && selectedSlotIndex == 0) { SetSelectedSlot(9); }
        if (Input.mouseScrollDelta.y < 0 && selectedSlotIndex != 9) { SetSelectedSlot(selectedSlotIndex + 1); }
        else if (Input.mouseScrollDelta.y < 0 && selectedSlotIndex == 9) { SetSelectedSlot(0); }
    }

    public void SetSelectedSlot(int slotIndex)
    {
        selectedSlotIndex = slotIndex;
        selectedSlot = slots[slotIndex];
        updateSelectorUI?.Invoke(slotIndex);
    }

    public void AddItemToSlot(Item item)
    {
        bool itemDealt = false;

        // Find Slot To Add To
        for (int s1 = 0; s1 < slots.Length; s1++) 
        {
            for (int s = 0; s < slots.Length; s++) // For each slot: check if any other slot has a stackable slot BEFORE adding to current slot
            {
                // Not the best but check every slot per slot for a stack of tiles, if it's the same tile, stack, otherwise MAKE NEW STACK
                if (slots[s].item.tileType == item.tileType)
                {
                    if (slots[s].count < maxStackSize)
                    {
                        // STACK ITEMS
                        slots[s].count++;
                        slots[s].item = item;

                        itemDealt = true;

                        updateSlotCallback?.Invoke(s);
                        break;
                    }
                }
            }
            if (itemDealt) // Break out of the larger for loop if item was stacked somewhere
            {
                break;
            }
            else if (item != slots[s1].item) // No other slot with this item was found, lets see if this one is eligible
            {
                // MAKE NEW STACK
                if (slots[s1].empty)
                {
                    slots[s1].item = item;
                    slots[s1].empty = false;
                    slots[s1].count = 0;
                    slots[s1].count++;

                    updateSlotCallback?.Invoke(s1);

                    itemDealt = true;
                    
                    break;
                }
            }
        }
        if (!itemDealt) // Player Inventory Full
        { 
            UnityEngine.Debug.LogWarning("Player Inventory Full, Not sure what to do with destroyed Tile"); 
            // Do something like spawn entity for dropped tile or goowop beeboops events and such
        }
    }

    public void SetSlot(Slot slot, int slotIndex)
    {
        slots[slotIndex].empty = false;
        slots[slotIndex].count = slot.count;
        slots[slotIndex].item.itemType = slot.item.itemType;
        slots[slotIndex].item.tileType = slot.item.tileType;

        updateSlotCallback?.Invoke(slotIndex);
    }

    public void AddToSlot(int amount, int slotIndex)
    {
        slots[slotIndex].count += amount;
    }

    public void TakeFromSlot(Slot slot) // Take from Slot if Count > 0, Check if Count is 0 and Clear
    {
        if (slot.count > 0) // Take from Slot if Something There
        {
            slot.count--;

            updateSlotCallback?.Invoke(selectedSlotIndex);
        }

        if (slot.count <= 0) // Slot is Empty, Clear Slot
        {
            ClearSlot(slot);
        }
    }

    public void ClearSlot(Slot slot)
    {
        slot.empty = true;
        slot.count = 0;
        slot.item.itemType = Item.ItemType.Tile;
        slot.item.tileType = Tile.TileType.Air;
        updateAllSlotsCallback?.Invoke();
    }

    public void SetInventoryToSave(string saveName)
    {
        for (int slot = 0; slot < slots.Length; slot++)
        {
            slotsToSave[slot].item.itemType = slots[slot].item.itemType;
            slotsToSave[slot].item.tileType = slots[slot].item.tileType;
            slotsToSave[slot].empty = slots[slot].empty;
            slotsToSave[slot].count = slots[slot].count;
        }
    }

    public void LoadInventory(string saveName)
    {
        SaveManager saveManager = GlobalReferences.saveManager;
        saveManager.LoadAllDataFromDisk(saveName);

        for (int slot = 0; slot < slots.Length; slot++)
        {
            // Set Slot Data to Loaded Slot Data
            slots[slot].item.itemType = saveManager.loadedData.playerData.playerInv[slot].item.itemType;
            slots[slot].item.tileType = saveManager.loadedData.playerData.playerInv[slot].item.tileType;
            slots[slot].empty = saveManager.loadedData.playerData.playerInv[slot].empty;
            slots[slot].count = saveManager.loadedData.playerData.playerInv[slot].count;

            inventoryLoaded = true;

            // Update UI (Visual GameObject) to Represent New Data Loaded
            updateSlotCallback?.Invoke(slot);
        }
    }
}
