using UnityEngine;

namespace LensorRadii.U_Grow
{

    /*  INVENTORY METHODS

    If this is confusing, 'used' just means that is what is passed to the function, followed by what it do. 

        - ISlotContainer:
            - SetSlot()             Use slot index and slot to overwrite a slot in the inventory                                                    UPDATES UI
            - TryAddToSlot()        Use a slot and try to merge it anywhere it can fit in the available slots                                       UPDATES UI
            - ModifySlotCount()     Use slot index and an integer, 'amount' (default 1), to add an amount to the slot at the passed slot index      UPDATES UI
            - ClearSlot()           Use slot index to set the slot back to defaults (empty)                                                         UPDATES UI

        - SetSelectedSlot()         Use a slot index to set 'selectedSlot' variable to the given slot at slot index                                 UPDATES SELECTOR ICON UI
     */

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

        #region UI Event Declaration

        public delegate void UpdateUIEvent(int slotNum);
        public UpdateUIEvent updateSelectorUI;
        public UpdateUIEvent updateSlotCallback;

        public delegate void UpdateAllUIEvent();
        public UpdateAllUIEvent updateAllSlotsCallback;

        #endregion

        private void OnEnable()
        {
            inputManager.Enable();
        }
        private void OnDisable()
        {
            inputManager.Disable();
        }

        private void Start()
        {
            // Initialize Inventory if Not Loaded
            if (!inventoryLoaded)
            {
                int a = 0;
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

        private void Update()
        {
            // Hard-Coded buttons to select inventory slots
            // Will replace with proper input manager mcgubbins at least before I add re-assignable buttons
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

            // Mouse Scroll Changes Selected Slot
            if (Input.mouseScrollDelta.y > 0 && selectedSlotIndex != 0) { SetSelectedSlot(selectedSlotIndex - 1); }
            else if (Input.mouseScrollDelta.y > 0 && selectedSlotIndex == 0) { SetSelectedSlot(9); }
            if (Input.mouseScrollDelta.y < 0 && selectedSlotIndex != 9) { SetSelectedSlot(selectedSlotIndex + 1); }
            else if (Input.mouseScrollDelta.y < 0 && selectedSlotIndex == 9) { SetSelectedSlot(0); }

            // Some input manager mcgubbins
            if (inputManager.PlayerUI.InventorySelectionBack.triggered && selectedSlotIndex != 0) { SetSelectedSlot(selectedSlotIndex - 1); }
            else if (inputManager.PlayerUI.InventorySelectionBack.triggered && selectedSlotIndex == 0) { SetSelectedSlot(9); }
            if (inputManager.PlayerUI.InventorySelectionForward.triggered && selectedSlotIndex != 9) { SetSelectedSlot(selectedSlotIndex + 1); }
            else if (inputManager.PlayerUI.InventorySelectionForward.triggered && selectedSlotIndex == 9) { SetSelectedSlot(0); }
        }

        public void SetSlot(int slotIndex, Slot slot)
        {
            slots[slotIndex].empty = slot.empty;
            slots[slotIndex].count = slot.count;
            slots[slotIndex].item.itemType = slot.item.itemType;
            slots[slotIndex].item.tileType = slot.item.tileType;

            updateSlotCallback?.Invoke(slotIndex);
        }

        public void TryAddToSlot(Slot slot)
        {
            bool itemDealt = false;

            for (int s = 0; s < slots.Length; s++)                      // Check for stackable slots
            {
                if (slots[s].item.tileType == slot.item.tileType)       // Slot tileType's match, ready to stack!
                {
                    if (slots[s].count + slot.count <= maxStackSize)    // Make sure slot has room to stack
                    {
                        slots[s].count += slot.count;                   // STACK ITEMS

                        itemDealt = true;                               // Set itemDealt flag to skip more checking

                        updateSlotCallback?.Invoke(s);                  // UPDATE UI
                        break;                                          // Don't check anymore slots, we found one bois
                    }
                }
            }

            if (!itemDealt)                                 // If item could not stack,
            {
                for (int s = 0; s < slots.Length; s++)      // Find Empty Slot To Add To
                {
                    if (slots[s].empty)                     // MAKE NEW STACK
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
            if (!itemDealt) // Player Inventory Full
            {
                Debug.LogWarning("Player Inventory Full, Not sure what to do with destroyed Tile");
                GameReferences.uIHandler.SendNotif("Player Inventory Full, Tile Voided", 5, Color.red);

                // TO-DO
                // Do something like spawn entity for dropped tile or goowop beeboops events and such
            }
        }

        public void ModifySlotCount(int slotIndex, int amount = 1)
        {
            if (slots[slotIndex].count + amount <= maxStackSize)
            {
                slots[slotIndex].count += amount;
            }

            updateSlotCallback?.Invoke(slotIndex);
        }

        public void ClearSlot(int slotIndex)                        // Pass a slot index to reset to default: empty
        {
            slots[slotIndex].empty = true;
            slots[slotIndex].count = 0;
            slots[slotIndex].item.itemType = Item.ItemType.Tile;
            slots[slotIndex].item.tileType = Tile.TileType.Air;

            updateAllSlotsCallback?.Invoke();                       // UPDATE UI
        }

        public void SetSelectedSlot(int slotIndex)
        {
            selectedSlotIndex = slotIndex;
            selectedSlot = slots[slotIndex];
            updateSelectorUI?.Invoke(slotIndex);                    // UPDATE SELECTOR ICON UI
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

            updateAllSlotsCallback?.Invoke();                       // UPDATE UI
        }
    }

}