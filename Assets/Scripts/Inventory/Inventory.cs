using UnityEngine;

namespace LensorRadii.U_Grow
{
    public class Inventory : MonoBehaviour, ISlotContainer
    {
        #region Singleton/Init

        public static Inventory instance;
        public InputManager inputManager;

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("Multiple Inventories attempting to Initialize");
            }
            else
            {
                instance = this;
                GameReferences.playerInv = instance;
            }

            inputManager = new InputManager();
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

        void OnEnable()
        {
            inputManager.Enable();
        }
        void OnDisable()
        {
            inputManager.Disable();
        }

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

            // Set Selected Slot to 0 (First in Inv)
            SetSelectedSlot(0);
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

            if (inputManager.PlayerUI.InventorySelectionBack.triggered && selectedSlotIndex != 0) { SetSelectedSlot(selectedSlotIndex - 1); }
            else if (inputManager.PlayerUI.InventorySelectionBack.triggered && selectedSlotIndex == 0) { SetSelectedSlot(9); }
            if (inputManager.PlayerUI.InventorySelectionForward.triggered && selectedSlotIndex != 9) { SetSelectedSlot(selectedSlotIndex + 1); }
            else if (inputManager.PlayerUI.InventorySelectionForward.triggered && selectedSlotIndex == 9) { SetSelectedSlot(0); }
        }

        public void SetSelectedSlot(int slotIndex)
        {
            selectedSlotIndex = slotIndex;
            selectedSlot = slots[slotIndex];
            updateSelectorUI?.Invoke(slotIndex);
        }

        public void TryAddToSlot(Slot slot)
        {
            bool itemDealt = false;

            for (int s = 0; s < slots.Length; s++) // Check for stackable slots
            {
                // Not the best but check every slot per slot for a stack of tiles, if it's the same tile, stack, otherwise MAKE NEW STACK
                if (slots[s].item.tileType == slot.item.tileType)
                {
                    if (slots[s].count + slot.count < maxStackSize)
                    {
                        // STACK ITEMS
                        slots[s].count += slot.count;

                        itemDealt = true;

                        updateSlotCallback?.Invoke(s);
                        break;
                    }
                }
            }

            if (!itemDealt)
            {
                for (int s = 0; s < slots.Length; s++) // Find Empty Slot To Add To
                {
                    if (slot.item != slots[s].item) // No other slot with this item was found, lets see if this one is eligible
                    {
                        // MAKE NEW STACK
                        if (slots[s].empty)
                        {
                            slots[s].item = slot.item;
                            slots[s].empty = false;
                            slots[s].count = 0;
                            slots[s].count++;

                            updateSlotCallback?.Invoke(s);

                            itemDealt = true;

                            break;
                        }
                    }
                }
            }
            if (!itemDealt) // Player Inventory Full
            {
                UnityEngine.Debug.LogWarning("Player Inventory Full, Not sure what to do with destroyed Tile");
                // Do something like spawn entity for dropped tile or goowop beeboops events and such
            }
        }

        public void SetSlot(int slotIndex, Slot slot)
        {
            slots[slotIndex].empty = slot.empty;
            slots[slotIndex].count = slot.count;
            slots[slotIndex].item.itemType = slot.item.itemType;
            slots[slotIndex].item.tileType = slot.item.tileType;

            updateSlotCallback?.Invoke(slotIndex);
        }

        public void ModifySlotCount(int slotIndex, int amount = 1)
        {
            if (slots[slotIndex].count + amount <= maxStackSize)
            {
                slots[slotIndex].count += amount;
            }

            updateSlotCallback?.Invoke(slotIndex);
        }

        public void TryTakeFromSlot(int slotIndex) // Take from Slot if Count > 0, Check if Count is 0 and Clear
        {
            if (slots[slotIndex].count > 0) // Take from Slot if Something There
            {
                slots[slotIndex].count--;

                updateSlotCallback?.Invoke(selectedSlotIndex);
            }

            if (slots[slotIndex].count <= 0) // Slot is Empty, Clear Slot
            {
                slots[slotIndex].empty = true;

                ClearSlot(slotIndex);
            }
        }

        public void ClearSlot(int slotIndex)
        {
            slots[slotIndex].empty = true;
            slots[slotIndex].count = 0;
            slots[slotIndex].item.itemType = Item.ItemType.Tile;
            slots[slotIndex].item.tileType = Tile.TileType.Air;

            updateAllSlotsCallback?.Invoke();
        }

        public void ClearSlotDirect(Slot slot)
        {
            slot.empty = true;
            slot.count = 0;
            slot.item.itemType = Item.ItemType.Tile;
            slot.item.tileType = Tile.TileType.Air;

            updateAllSlotsCallback?.Invoke();
        }



        public Slot[] GetInventoryToSave()
        {
            for (int slot = 0; slot < slots.Length; slot++)
            {
                slotsToSave[slot].item.itemType = slots[slot].item.itemType;
                slotsToSave[slot].item.tileType = slots[slot].item.tileType;
                slotsToSave[slot].empty = slots[slot].empty;
                slotsToSave[slot].count = slots[slot].count;
            }

            return slotsToSave;
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
            }

            inventoryLoaded = true;

            updateAllSlotsCallback?.Invoke();
        }
    }

}