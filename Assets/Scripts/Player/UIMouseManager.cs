using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private int mouseSlotIndex;
    [SerializeField]
    private int hoverSlotIndex;

    public Slot MouseSlot
    {
        get
        {
            return mouseSlot;
        }
    }
    public Slot HoverSlot
    {
        get
        {
            return hoverSlot;
        }
    }

    void Start()
    {
        // Subscribe to Every SlotDragHandler Event...
        SlotDragHandler[] slots = FindObjectsOfType<SlotDragHandler>();
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetMouseSlot += SetMouseSlot;

            slots[i].EndDrag += TryEndDrag;

            slots[i].HoverOverSlot += SetHoverSlot;
            slots[i].ClearHoverSlot += ClearHoverSlot;
        }

        cachedSlotIndex = -1;
        mouseSlotIndex = -1;
        hoverSlotIndex = -1;
    }



    public void SetMouseSlot(int slotIndex)
    {
        if (slotIndex == 12)
        {
            mouseSlot = GameReferences.craftSystem.outputSlot;
        }
        else if (slotIndex >= 10)
        {
            // Set mouseSlot from CraftSystem
            mouseSlot = GameReferences.craftSystem.recipeSlots[slotIndex - 10];
        }
        else // Set mouseSlot from Player Inventory
        {
            mouseSlot = GameReferences.playerInv.slots[slotIndex];
        }

        mouseSlotIndex = slotIndex;
        cachedSlotIndex = slotIndex;
    }

    public void ClearMouseSlot(int unused)
    {
        mouseSlot.empty = true;
        mouseSlot.count = 0;
        mouseSlot.item.itemType = Item.ItemType.Tile;
        mouseSlot.item.tileType = Tile.TileType.Air;
        mouseSlotIndex = -1;
        cachedSlotIndex = -1;
    }



    public void TryEndDrag(int unused)
    {
        // Check if hovered slot is there
        // If it is, Transfer Mouse Slot Data to Slot That is Hovered Over
        if (cachedSlotIndex != hoverSlotIndex && hoverSlotIndex != -1)
        {
            if (hoverSlotIndex == 12) // If Hovering Over Output Slot
            {
                // Dont Do Anything If Drag Item Onto Output Slot in Craft UI
                return;
            }
            else if (hoverSlotIndex >= 10) // If Hovering Over Recipe Slots
            {
                int hoverIndex = hoverSlotIndex - 10;
                
                if (GameReferences.craftSystem.recipeSlots[hoverIndex].count == 0)
                {
                    UnityEngine.Debug.Log("Setting Hovered Over Recipe Slot to Mouse Slot Data!");

                    GameReferences.craftSystem.TryAddToRecipeSlot(mouseSlot, hoverIndex);

                    if (cachedSlotIndex == 12)
                    {
                        ClearSlot(GameReferences.craftSystem.outputSlot);

                        GameReferences.craftSystem.updateOutputSlotUI();
                    }
                    else if (cachedSlotIndex >= 10)
                    {
                        ClearSlot(GameReferences.craftSystem.recipeSlots[cachedSlotIndex - 10]);

                        GameReferences.craftSystem.updateRecipeSlotUI(cachedSlotIndex - 10);
                    }
                    else if (cachedSlotIndex >= 0)
                    {
                        ClearSlot(GameReferences.playerInv.slots[cachedSlotIndex]);

                        GameReferences.playerInv.updateAllSlotsCallback();
                    }

                    GameReferences.craftSystem.updateRecipeSlotUI(hoverIndex);
                }
                else
                {
                    if (GameReferences.craftSystem.recipeSlots[hoverIndex].item.itemType == mouseSlot.item.itemType)
                    {
                        if (GameReferences.craftSystem.recipeSlots[hoverIndex].item.tileType == mouseSlot.item.tileType)
                        {
                            UnityEngine.Debug.Log("Adding Mouse Slot Data to Hovered Over Craft Recipe Slot");

                            GameReferences.craftSystem.TryAddToRecipeSlot(mouseSlot, hoverIndex);

                            if (cachedSlotIndex == 12)
                            {
                                ClearSlot(GameReferences.craftSystem.outputSlot);

                                GameReferences.craftSystem.updateOutputSlotUI();
                            }
                            else if (cachedSlotIndex >= 10)
                            {
                                ClearSlot(GameReferences.craftSystem.recipeSlots[cachedSlotIndex - 10]);

                                GameReferences.craftSystem.updateRecipeSlotUI(cachedSlotIndex - 10);
                            }
                            else if (cachedSlotIndex >= 0)
                            {
                                ClearSlot(GameReferences.playerInv.slots[cachedSlotIndex]);

                                GameReferences.playerInv.updateAllSlotsCallback();
                            }

                            GameReferences.craftSystem.updateRecipeSlotUI(hoverIndex);
                        }
                    }
                    return;
                }
            }
            else if (hoverSlotIndex >= 0) // If Hovering Over Player Inv Slots
            {
                if (GameReferences.playerInv.slots[hoverSlotIndex].count == 0)
                {
                    UnityEngine.Debug.Log("Setting Hovered Over Player Inventory Slot to Mouse Slot Data");

                    GameReferences.playerInv.SetSlot(mouseSlot, hoverSlotIndex);

                    if (cachedSlotIndex == 12)
                    {
                        ClearSlot(GameReferences.craftSystem.outputSlot);

                        GameReferences.craftSystem.updateOutputSlotUI();
                    }
                    else if (cachedSlotIndex >= 10)
                    {
                        ClearSlot(GameReferences.craftSystem.recipeSlots[cachedSlotIndex - 10]);

                        GameReferences.craftSystem.updateAllRecipeSlots();
                    }
                    else if (cachedSlotIndex >= 0)
                    {
                        ClearSlot(GameReferences.playerInv.slots[cachedSlotIndex]);
                    }

                    GameReferences.playerInv.updateAllSlotsCallback();
                }
                else
                {
                    if (GameReferences.playerInv.slots[hoverSlotIndex].item.itemType == mouseSlot.item.itemType)
                    {
                        if (GameReferences.playerInv.slots[hoverSlotIndex].item.tileType == mouseSlot.item.tileType)
                        {
                            UnityEngine.Debug.Log("Adding Mouse Slot Data to Hovered Over Player Inventory Slot");

                            GameReferences.playerInv.AddToSlot(mouseSlot.count, hoverSlotIndex);

                            if (cachedSlotIndex == 12)
                            {
                                ClearSlot(GameReferences.craftSystem.outputSlot);

                                GameReferences.craftSystem.updateOutputSlotUI();
                            }
                            else if (cachedSlotIndex >= 10)
                            {
                                ClearSlot(GameReferences.craftSystem.recipeSlots[cachedSlotIndex - 10]);

                                GameReferences.craftSystem.updateAllRecipeSlots();
                            }
                            else if (cachedSlotIndex >= 0)
                            {
                                ClearSlot(GameReferences.playerInv.slots[cachedSlotIndex]);
                            }

                            GameReferences.playerInv.updateAllSlotsCallback();
                        }
                    }

                    return;
                }
            }

            ClearMouseSlot(-1);
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
        if (slotIndex == 12)
        {
            hoverSlot.empty = GameReferences.craftSystem.outputSlot.empty;
            hoverSlot.count = GameReferences.craftSystem.outputSlot.count;
            hoverSlot.item.itemType = GameReferences.craftSystem.outputSlot.item.itemType;
            hoverSlot.item.tileType = GameReferences.craftSystem.outputSlot.item.tileType;
        }
        else if (slotIndex >= 10)
        {
            hoverSlot.empty = GameReferences.craftSystem.recipeSlots[slotIndex - 10].empty;
            hoverSlot.count = GameReferences.craftSystem.recipeSlots[slotIndex - 10].count;
            hoverSlot.item.itemType = GameReferences.craftSystem.recipeSlots[slotIndex - 10].item.itemType;
            hoverSlot.item.tileType = GameReferences.craftSystem.recipeSlots[slotIndex - 10].item.tileType;
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
