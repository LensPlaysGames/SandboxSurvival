using System.Collections.Generic;
using UnityEngine;

namespace U_Grow
{
    public class Chest : MonoBehaviour, IInteracteable, ISlotContainer
    {
        public int numberOfSlots = 18;
        public int maxStackSize = 90;

        public int x, y;

        private List<Slot> slots;
        private List<Transform> slotParents;

        private bool loaded;
        private void Start()
        {
            #region Initialize slots Data

            if (!loaded)
            {
                slots = new List<Slot>();

                for (int s = 0; s < numberOfSlots; s++)
                {
                    Slot slot = new Slot
                    {
                        empty = true,
                        count = 0,
                        item = new Item
                        {
                            itemType = Item.ItemType.Tile,
                            tileType = Tile.TileType.Air
                        }
                    };

                    slots.Add(slot);
                }
            }

            #endregion

            x = (int)(transform.position.x / GameReferences.levelGenerator.GetLevelInstance().Scale);
            y = (int)(transform.position.y / GameReferences.levelGenerator.GetLevelInstance().Scale);
        }

        public void ClearSlot(int slotIndex)
        {
            slots[slotIndex].empty = true;
            slots[slotIndex].count = 0;
            slots[slotIndex].item.itemType = Item.ItemType.Tile;
            slots[slotIndex].item.tileType = Tile.TileType.Air;

            UpdateUI();
        }

        public void ModifySlotCount(int slotIndex, int amount)
        {
            if (slots[slotIndex].count + amount <= maxStackSize)
            {
                slots[slotIndex].count += amount;

                UpdateSlotUI(slotIndex);
            }
        }

        public void SetSlot(int slotIndex, Slot slot)
        {
            slots[slotIndex].empty = slot.empty;
            slots[slotIndex].count = slot.count;
            slots[slotIndex].item.itemType = slot.item.itemType;
            slots[slotIndex].item.tileType = slot.item.tileType;

            UpdateSlotUI(slotIndex);
        }

        public void TryAddToSlot(Slot slot)
        {
            bool itemDealt = false;

            for (int s = 0; s < slots.Count; s++)
            {
                for (int s1 = 0; s1 < slots.Count; s1++)
                {
                    if (slots[s].item.tileType == slot.item.tileType)
                    {
                        if (slots[s].count + slot.count < maxStackSize)
                        {
                            slots[s].count += slot.count;

                            itemDealt = true;

                            UpdateSlotUI(s);
                            break;
                        }
                    }
                }
                if (itemDealt) { return; }
                else if (slots[s].empty)
                {
                    slots[s].empty = false;
                    slots[s].count = slot.count;
                    slots[s].item = slot.item;

                    itemDealt = true;

                    UpdateSlotUI(s);

                    break;
                }
            }
            if (!itemDealt)
            {
                Debug.LogError("Error when Trying to add Item to Chest!");
            }
        }

        public void TryTakeFromSlot(int slotIndex)
        {
            if (slots[slotIndex].count > 0)
            {
                slots[slotIndex].count--;

                UpdateSlotUI(slotIndex);
            }
            else
            {
                ClearSlot(slotIndex);
            }
        }

        public void Use()
        {
            Debug.Log("Opening Chest!");

            GameReferences.uIHandler.inMenu = true;
            GameReferences.uIHandler.ExitMenu += ExitUI;

            GameReferences.chestUI.gameObject.SetActive(true);

            // Fill Chest UI with SlotPrefabs (Instantiate), Fill slotParents List from Newly Created Prefabs (.Add())


            // Update UI
        }

        public void UpdateUI()
        {
            for (int s = 0; s < slots.Count; s++)
            {
                UpdateSlotUI(s);
            }
        }
        public void UpdateSlotUI(int slotIndex)
        {
            Slot slotData = slots[slotIndex];

            // Need to figure out how I'm going to go about the UI at this point
        }

        public void ExitUI()
        {
            GameReferences.uIHandler.inMenu = false;
            GameReferences.chestUI.gameObject.SetActive(false);
        }

        public Slot[] GetSlotsToSave()
        {
            Slot[] slotsToSave = new Slot[slots.Count];

            for (int slot = 0; slot < slots.Count; slot++)
            {
                slotsToSave[slot] = new Slot
                {
                    count = slots[slot].count,
                    empty = slots[slot].empty,
                    item = new Item
                    {
                        itemType = slots[slot].item.itemType,
                        tileType = slots[slot].item.tileType
                    }
                };
            }

            return slotsToSave;
        }

        public void LoadChestFromSave(string saveName)
        {
            SaveManager saveManager = GlobalReferences.saveManager;
            saveManager.LoadAllDataFromDisk(saveName);

            // Find THIS chest in save data (oh god)
        }
    }

}