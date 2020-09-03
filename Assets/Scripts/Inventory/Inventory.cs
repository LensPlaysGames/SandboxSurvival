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

    public int selectedSlotIndex;
    public Slot selectedSlot;
    public GameObject slotSelector;

    public Slot[] slots;
    public Sprite[] sprites; // THIS HAS TO MATCH Tile.TileType INDEX PERFECTLY. IF LOADING WRONG IMAGE BUT CORRECT DATA IN INVENTORY, THIS IS THE CULPRIT

    public GameObject empty;

    public Slot[] slotsToSave;
    private int a;
    
    void Start()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogError("Multiple Inventories attempting to Initialize");
        }
        else { instance = this; }

        if (!inventoryLoaded)
        {
            foreach (Slot slot in slots)
            {
                a++;
                slot.slotParent = GameObject.Find("Slot (" + a + ")");
                slot.countText = GameObject.Find("Slot (" + a + ") Count");
                slot.empty = true;
                slot.count = 0;
                slot.isTile = false;
                slot.tileType = Tile.TileType.Air;
                slot.sprite = null;
            }
        }

        // Initialize sprites Array to Sprite based on list of Tile Types in Tile.cs
        for (int tileTypeIndex = 0; tileTypeIndex < Enum.GetNames(typeof(Tile.TileType)).Length; tileTypeIndex++)
        {
            sprites[tileTypeIndex] = Resources.Load<Sprite>(Enum.GetName(typeof(Tile.TileType), tileTypeIndex));
        }

        if (selectedSlot.slotParent == null)
        {
            selectedSlot = slots[0]; 
            slotSelector.transform.position = slots[0].slotParent.transform.position;
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { selectedSlot = slots[0]; slotSelector.transform.position = slots[0].slotParent.transform.position; selectedSlotIndex = 0; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { selectedSlot = slots[1]; slotSelector.transform.position = slots[1].slotParent.transform.position; selectedSlotIndex = 1; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { selectedSlot = slots[2]; slotSelector.transform.position = slots[2].slotParent.transform.position; selectedSlotIndex = 2; }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { selectedSlot = slots[3]; slotSelector.transform.position = slots[3].slotParent.transform.position; selectedSlotIndex = 3; }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { selectedSlot = slots[4]; slotSelector.transform.position = slots[4].slotParent.transform.position; selectedSlotIndex = 4; }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { selectedSlot = slots[5]; slotSelector.transform.position = slots[5].slotParent.transform.position; selectedSlotIndex = 5; }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { selectedSlot = slots[6]; slotSelector.transform.position = slots[6].slotParent.transform.position; selectedSlotIndex = 6; }
        if (Input.GetKeyDown(KeyCode.Alpha8)) { selectedSlot = slots[7]; slotSelector.transform.position = slots[7].slotParent.transform.position; selectedSlotIndex = 7; }
        if (Input.GetKeyDown(KeyCode.Alpha9)) { selectedSlot = slots[8]; slotSelector.transform.position = slots[8].slotParent.transform.position; selectedSlotIndex = 8; }
        if (Input.GetKeyDown(KeyCode.Alpha0)) { selectedSlot = slots[9]; slotSelector.transform.position = slots[9].slotParent.transform.position; selectedSlotIndex = 9; }
    }

    public void AddTileToSlot(Tile.TileType tileType)
    {
        bool itemDealt = false;

        // Notify Player
        GameObject.Find("UICanvas").GetComponent<UIHandler>().SendNotif("Added One " + tileType.ToString() + " To Inventory", Color.white);

        // Find Slot To Add To
        for (int s1 = 0; s1 < slots.Length; s1++)
        {
            for (int s = 0; s < slots.Length; s++)
            {
                if (tileType == slots[s].tileType)
                {
                    // STACK ITEMS
                    slots[s].count++;
                    slots[s].isTile = true;

                    itemDealt = true;

                    UpdateSlotUI(s);
                    break;
                }
            }
            if (itemDealt)
            {
                break;
            }
            else if (tileType != slots[s1].tileType)
            {
                // MAKE NEW STACK
                if (slots[s1].empty)
                {
                    slots[s1].tileType = tileType;
                    slots[s1].empty = false;
                    slots[s1].count = 0;
                    slots[s1].count++;
                    slots[s1].isTile = true;

                    if (sprites[(int)tileType] != null)
                    {
                        slots[s1].sprite = sprites[(int)tileType];
                    }
                    else { UnityEngine.Debug.LogError("Error when trying to AddItemToSlot: Sprite for " + tileType.ToString() + " not found in Inventory Sprites Array!"); }

                    itemDealt = true;

                    UpdateSlotUI(s1);
                    break;
                }
            }
        }
    }

    public void TakeFromSlot(Slot s)
    {
        s.count--;
        UpdateSlotUI(selectedSlotIndex);
    }

    void UpdateSlotUI(int slotNum) 
    {
        // If Image Exists In Slot, Destroy It, We're about to Update it
        if (slots[slotNum].slotParent.transform.Find("EmptyImagePrefab(Clone)") != null)
        {
            Destroy(slots[slotNum].slotParent.transform.Find("EmptyImagePrefab(Clone)").gameObject);
        }

        // If slot is populated, create image and update text
        if (slots[slotNum].count != 0)
        {
            GameObject item = Instantiate(empty, slots[slotNum].slotParent.transform);
            item.transform.SetSiblingIndex(item.transform.GetSiblingIndex() - 1);
            item.GetComponent<Image>().sprite = slots[slotNum].sprite;
            slots[slotNum].countText.GetComponent<TextMeshProUGUI>().text = slots[slotNum].count.ToString();
        }
        else // NO MORE ITEMS IN SLOT, REMOVE ATTRIBUTES
        {
            if (slots[slotNum].slotParent.transform.Find("EmptyImagePrefab(Clone)") != null)
            {
                Destroy(slots[slotNum].slotParent.transform.Find("EmptyImagePrefab(Clone)").gameObject);
            }

            slots[slotNum].tileType = Tile.TileType.Air;
            slots[slotNum].isTile = false;
            slots[slotNum].empty = true;
            slots[slotNum].count = 0;
            slots[slotNum].countText.GetComponent<TextMeshProUGUI>().text = "";
            slots[slotNum].sprite = null;
        }
    }



    public void SaveInventory(string saveName)
    {
        for (int slot = 0; slot < slots.Length; slot++)
        {
            slotsToSave[slot].isTile = slots[slot].isTile;
            slotsToSave[slot].empty = slots[slot].empty;
            slotsToSave[slot].count = slots[slot].count;

            slotsToSave[slot].tileType = slots[slot].tileType;
            slotsToSave[slot].slotParentName = slots[slot].slotParent.name;
            slotsToSave[slot].countTextName = slots[slot].countText.name;
            if (slots[slot].sprite != null)
            {
                slotsToSave[slot].spriteName = slots[slot].sprite.name;
            }
            else { slotsToSave[slot].spriteName = "Air"; }
        }

        SaveManager saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        saveManager.SaveInventoryDataToDisk(saveName, slotsToSave);
    }

    public void LoadInventory(string saveName)
    {
        SaveManager saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        saveManager.LoadInventoryDataFromDisk(saveName);

        for (int slot = 0; slot < slots.Length; slot++)
        {
            // Set Slot Data to Loaded Slot Data
            slots[slot].isTile = saveManager.loadedSlots[slot].isTile;
            slots[slot].empty = saveManager.loadedSlots[slot].empty;
            slots[slot].count = saveManager.loadedSlots[slot].count;

            slots[slot].tileType = saveManager.loadedSlots[slot].tileType;
            slots[slot].slotParent = GameObject.Find(saveManager.loadedSlots[slot].slotParentName);
            slots[slot].countText = GameObject.Find(saveManager.loadedSlots[slot].countTextName);
            slots[slot].sprite = Resources.Load<Sprite>(saveManager.loadedSlots[slot].spriteName);

            inventoryLoaded = true;

            // Update UI (Visual GameObject) to Represent New Data Loaded
            UpdateSlotUI(slot);
        }
    }
}
