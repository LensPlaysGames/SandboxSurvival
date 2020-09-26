using UnityEngine;

namespace LensorRadii.U_Grow
{
    public class UIMouseManager : MonoBehaviour
    {
        #region Singleton/Init

        public static UIMouseManager instance;

        void Awake()
        {
            if (instance != null)
            {
                UnityEngine.Debug.LogError("Mulitple UIMouseManagers!!");
                Destroy(this);
            }
            else
            {
                instance = this;
                GameReferences.uIMouseManager = instance;
            }
        }

        #endregion

        [SerializeField]
        private Slot mouseSlot;
        [SerializeField]
        private Slot hoverSlot;

        [SerializeField]
        private int cachedSlotIndex;
        [SerializeField]
        private int hoverSlotIndex;

        public Slot MouseSlot => mouseSlot;
        public Slot HoverSlot => hoverSlot;

        SlotDragHandler[] slots;

        public Chest interactingChest;

        void Start()
        {
            // Subscribe to Every SlotDragHandler Event...
            slots = FindObjectsOfType<SlotDragHandler>();
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].SetMouseSlot += SetMouseSlot;

                slots[i].EndDrag += TryEndDrag;

                slots[i].HoverOverSlot += SetHoverSlot;
                slots[i].ClearHoverSlot += ClearHoverSlot;
            }

            cachedSlotIndex = -1;
            hoverSlotIndex = -1;
        }

        private void FixedUpdate()
        {
            if (FindObjectsOfType<SlotDragHandler>().Length != slots.Length)
            {
                slots = FindObjectsOfType<SlotDragHandler>();

                for (int i = 0; i < slots.Length; i++)
                {
                    slots[i].SetMouseSlot += SetMouseSlot;

                    slots[i].EndDrag += TryEndDrag;

                    slots[i].HoverOverSlot += SetHoverSlot;
                    slots[i].ClearHoverSlot += ClearHoverSlot;
                }
            }
        }

        public void SetMouseSlot(int slotIndex)
        {
            if (slotIndex >= 13 && slotIndex <= 89) // Set mouseSlot from Chest Slot [slotIndex - 13]
            {
                // Actually find the chest and ya know, read the slot we need ta, yaf uc k
                mouseSlot = interactingChest.slots[slotIndex - 13];
            }
            else if (slotIndex == 12) // Set mouseSlot from CraftSystem output Slot
            {
                mouseSlot = GameReferences.craftSystem.outputSlot;
            }
            else if (slotIndex >= 10) // Set mouseSlot from CraftSystem Recipe Slots
            {
                mouseSlot = GameReferences.craftSystem.recipeSlots[slotIndex - 10];
            }
            else // Set mouseSlot from Player Inventory
            {
                mouseSlot = GameReferences.playerInv.slots[slotIndex];
            }

            cachedSlotIndex = slotIndex;
        }

        public void ClearMouseSlot()
        {
            mouseSlot.empty = true;
            mouseSlot.count = 0;
            mouseSlot.item.itemType = Item.ItemType.Tile;
            mouseSlot.item.tileType = Tile.TileType.Air;
            cachedSlotIndex = -1;
        }



        public void TryEndDrag(int unused)
        {
            /*/ ~ Lens

            I feel the need to sign this due to the sheer stupidity required in order to achieve something of this nature
            127 LINES GOD-DAMNIT... 127...

            DOWN TO 98! WOO HOO! (Consolidated ClearCachedSlot Methods Into Each System)

            /*/

            if (cachedSlotIndex != hoverSlotIndex && hoverSlotIndex != -1) // Check if hovered slot is there. If it is, Transfer Mouse Slot Data to Slot That is Hovered Over
            {
                if (hoverSlotIndex >= 13 && hoverSlotIndex <= 89) // If Hovering Over Chest
                {
                    int hoverINDEX = hoverSlotIndex - 13;

                    if (interactingChest.slots[hoverINDEX].count == 0)
                    {
                        interactingChest.SetSlot(hoverINDEX, mouseSlot);
                    }
                    else if (interactingChest.slots[hoverINDEX].item.itemType == mouseSlot.item.itemType)
                    {
                        if (interactingChest.slots[hoverINDEX].item.tileType == mouseSlot.item.tileType)
                        {
                            interactingChest.ModifySlotCount(hoverINDEX, mouseSlot.count);
                        }
                    }

                    ClearCachedSlot();

                    interactingChest.UpdateUI();
                }
                else if (hoverSlotIndex == 12) // If Hovering Over Output Slot
                {
                    return; // Dont Do Anything If Drag Item Onto Output Slot in Craft UI
                }
                else if (hoverSlotIndex >= 10) // If Hovering Over Recipe Slots
                {
                    int hoverINDEX = hoverSlotIndex - 10;

                    if (GameReferences.craftSystem.recipeSlots[hoverINDEX].count == 0) // If Slot is Empty
                    {
                        Debug.Log("Setting Hovered Over Recipe Slot to Mouse Slot Data!");

                        GameReferences.craftSystem.SetSlot(hoverINDEX, mouseSlot);

                        ClearCachedSlot();

                        GameReferences.craftSystem.updateRecipeSlotUI?.Invoke(hoverINDEX);
                    }
                    else // Or slot Can Stack
                    {
                        if (GameReferences.craftSystem.recipeSlots[hoverINDEX].item.itemType == mouseSlot.item.itemType)
                        {
                            if (GameReferences.craftSystem.recipeSlots[hoverINDEX].item.tileType == mouseSlot.item.tileType)
                            {
                                Debug.Log("Adding Mouse Slot Data to Hovered Over Craft Recipe Slot");

                                GameReferences.craftSystem.AddToSlot(hoverINDEX, mouseSlot);

                                ClearCachedSlot();

                                GameReferences.craftSystem.updateRecipeSlotUI?.Invoke(hoverINDEX);
                            }
                        }
                        return;
                    }
                }
                else if (hoverSlotIndex >= 0) // If Hovering Over Player Inv Slots
                {
                    if (GameReferences.playerInv.slots[hoverSlotIndex].count == 0)
                    {
                        Debug.Log("Setting Hovered Over Player Inventory Slot to Mouse Slot Data");

                        GameReferences.playerInv.SetSlot(hoverSlotIndex, mouseSlot);

                        ClearCachedSlot();

                        GameReferences.playerInv.updateAllSlotsCallback?.Invoke();
                    }
                    else
                    {
                        if (GameReferences.playerInv.slots[hoverSlotIndex].item.itemType == mouseSlot.item.itemType)
                        {
                            if (GameReferences.playerInv.slots[hoverSlotIndex].item.tileType == mouseSlot.item.tileType)
                            {
                                Debug.Log("Adding Mouse Slot Data to Hovered Over Player Inventory Slot");

                                GameReferences.playerInv.ModifySlotCount(hoverSlotIndex, mouseSlot.count);

                                ClearCachedSlot();

                                GameReferences.playerInv.updateAllSlotsCallback?.Invoke();
                            }
                        }

                        return;
                    }
                }

                ClearMouseSlot();
            }
        }

        public void ClearCachedSlot()
        {
            if (cachedSlotIndex != -1)
            {
                if (cachedSlotIndex >= 13 && cachedSlotIndex <= 89)
                {
                    interactingChest.ClearSlot(cachedSlotIndex - 13);
                }
                else if (cachedSlotIndex == 12)
                {
                    GameReferences.craftSystem.OnCraftItem();
                }
                else if (cachedSlotIndex >= 10)
                {
                    GameReferences.craftSystem.ClearSlot(cachedSlotIndex - 10);
                }
                else if (cachedSlotIndex >= 0)
                {
                    GameReferences.playerInv.ClearSlot(cachedSlotIndex);
                }
            }
        }

        public void ClearSlot(Slot slotToClear)
        {
            slotToClear.empty = true;
            slotToClear.count = 0;
            slotToClear.item.itemType = Item.ItemType.Tile;
            slotToClear.item.tileType = Tile.TileType.Air;
        }

        public void SetHoverSlot(int slotIndex)
        {
            if (slotIndex >= 13 && slotIndex <= 89)
            {
                int slotINDEX = slotIndex - 13;

                // Find Chest to set Hover Slot to...
            }
            else if (slotIndex == 12)
            {
                hoverSlot.empty = GameReferences.craftSystem.outputSlot.empty;
                hoverSlot.count = GameReferences.craftSystem.outputSlot.count;
                hoverSlot.item.itemType = GameReferences.craftSystem.outputSlot.item.itemType;
                hoverSlot.item.tileType = GameReferences.craftSystem.outputSlot.item.tileType;
            }
            else if (slotIndex >= 10)
            {
                int slotINDEX = slotIndex - 10;

                hoverSlot.empty = GameReferences.craftSystem.recipeSlots[slotINDEX].empty;
                hoverSlot.count = GameReferences.craftSystem.recipeSlots[slotINDEX].count;
                hoverSlot.item.itemType = GameReferences.craftSystem.recipeSlots[slotINDEX].item.itemType;
                hoverSlot.item.tileType = GameReferences.craftSystem.recipeSlots[slotINDEX].item.tileType;
            }
            else
            {
                hoverSlot.empty = GameReferences.playerInv.slots[slotIndex].empty;
                hoverSlot.count = GameReferences.playerInv.slots[slotIndex].count;
                hoverSlot.item.itemType = GameReferences.playerInv.slots[slotIndex].item.itemType;
                hoverSlot.item.tileType = GameReferences.playerInv.slots[slotIndex].item.tileType;
            }
            hoverSlotIndex = slotIndex;
        }

        public void ClearHoverSlot(int unused)
        {
            hoverSlot.empty = true;
            hoverSlot.count = 0;
            hoverSlot.item.itemType = Item.ItemType.Tile;
            hoverSlot.item.tileType = Tile.TileType.Air;
            hoverSlotIndex = -1;
        }
    }
}