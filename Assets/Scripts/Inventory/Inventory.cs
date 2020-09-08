using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public bool inventoryLoaded = false;
    public bool spritesLoaded = false;

    public int selectedSlotIndex;
    public Slot selectedSlot;

    public int maxStackSize = 90;

    public Slot[] slots;
    public Sprite[] tileSprites;

    public Slot[] slotsToSave;
    private int a;

    public delegate void UpdateSelector(int slotNum);
    public UpdateSelector updateSelectorUI;

    public delegate void UpdateSlot(int slotIndex);
    public UpdateSlot updateSlotCallback;

    public delegate void UpdateAllSlots();
    public UpdateAllSlots updateAllSlotsCallback;

    void Awake()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogError("Multiple Inventories attempting to Initialize");
        }
        instance = this;
        GameReferences.playerInv = instance;
    }

    void Start()
    {
        // Initialize Inventory if Not Loaded from Save
        if (!inventoryLoaded)
        {
            foreach (Slot slot in slots)
            {
                a++;
                slot.slotParent = GameReferences.playerInvUI.transform.Find("InventoryBackground").transform.Find("Slot (" + a + ")").gameObject;
                slot.countText = GameObject.Find("Slot (" + a + ") Count");
                slot.empty = true;
                slot.count = 0;
                slot.item.itemType = Item.ItemType.Tile;
                slot.item.tileType = Tile.TileType.Air;
                slot.sprite = null;
            }
        }

        // Initialize sprites Array from Sprite Database
        tileSprites = DataDontDestroyOnLoad.instance.spriteDB;

        // Set Selected Slot to 0 if Null
        if (selectedSlot.slotParent == null)
        {
            selectedSlot = slots[0];
            updateSelectorUI?.Invoke(0);
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

        if (Input.mouseScrollDelta.y > 0 && selectedSlotIndex != 9) { SetSelectedSlot(selectedSlotIndex + 1); }
        else if (Input.mouseScrollDelta.y > 0 && selectedSlotIndex == 9) { SetSelectedSlot(0); }
        if (Input.mouseScrollDelta.y < 0 && selectedSlotIndex != 0) { SetSelectedSlot(selectedSlotIndex - 1); }
        else if (Input.mouseScrollDelta.y < 0 && selectedSlotIndex == 0) { SetSelectedSlot(9); }
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
            for (int s = 0; s < slots.Length; s++)
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
            if (itemDealt)
            {
                break;
            }
            else if (item != slots[s1].item)
            {
                // MAKE NEW STACK
                if (slots[s1].empty)
                {
                    slots[s1].item = item;
                    slots[s1].empty = false;
                    slots[s1].count = 0;
                    slots[s1].count++;

                    if (item.itemType == Item.ItemType.Tile)
                    {
                        if (tileSprites[(int)item.tileType] != null)
                        {
                            slots[s1].sprite = tileSprites[(int)item.tileType];
                        }
                        else 
                        { 
                            UnityEngine.Debug.LogError("Error when trying to AddItemToSlot: Sprite for " + item.tileType.ToString() + " not found in Inventory Sprites Array!"); 
                        }
                    }
                    else if (item.itemType == Item.ItemType.Tool)
                    {
                        // Get Appropriate Tool Sprite in Database (has to equal enums or I'll do it another way)

                    }
                    else if (item.itemType == Item.ItemType.Weapon)
                    {
                        // Get Appropriate Weapon Sprite in Database (has to equal enums or I'll do it another way)

                    }

                    itemDealt = true;

                    updateSlotCallback?.Invoke(s1);
                    break;
                }
            }
        }
        if (!itemDealt) { UnityEngine.Debug.LogWarning("Player Inventory Full, Not sure what to do with destroyed Tile"); }
    }

    public void TakeFromSlot(Slot slot)
    {
        slot.count--;
        updateSlotCallback?.Invoke(selectedSlotIndex);
    }

    public void ClearSlot(Slot slot)
    {
        slot.empty = true;
        slot.count = 0;
        slot.item.itemType = Item.ItemType.Tile;
        slot.item.tileType = Tile.TileType.Air;
        slot.sprite = null;
        updateAllSlotsCallback?.Invoke();
    }



    public void SetInventoryToSave(string saveName)
    {
        for (int slot = 0; slot < slots.Length; slot++)
        {
            slotsToSave[slot].item.itemType = slots[slot].item.itemType;
            slotsToSave[slot].empty = slots[slot].empty;
            slotsToSave[slot].count = slots[slot].count;

            slotsToSave[slot].item.tileType = slots[slot].item.tileType;
            slotsToSave[slot].slotParentName = slots[slot].slotParent.name;
            slotsToSave[slot].countTextName = slots[slot].countText.name;
            if (slots[slot].sprite != null)
            {
                slotsToSave[slot].spriteName = slots[slot].sprite.name;
            }
            else { slotsToSave[slot].spriteName = "Air"; }
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
            slots[slot].empty = saveManager.loadedData.playerData.playerInv[slot].empty;
            slots[slot].count = saveManager.loadedData.playerData.playerInv[slot].count;

            slots[slot].item.tileType = saveManager.loadedData.playerData.playerInv[slot].item.tileType;
            slots[slot].slotParent = GameObject.Find(saveManager.loadedData.playerData.playerInv[slot].slotParentName);
            slots[slot].countText = GameObject.Find(saveManager.loadedData.playerData.playerInv[slot].countTextName);
            slots[slot].sprite = DataDontDestroyOnLoad.instance.spriteDB[(int)Enum.Parse(typeof(Tile.TileType), saveManager.loadedData.playerData.playerInv[slot].spriteName)];

            inventoryLoaded = true;

            // Update UI (Visual GameObject) to Represent New Data Loaded
            updateSlotCallback?.Invoke(slot);
        }
    }
}
