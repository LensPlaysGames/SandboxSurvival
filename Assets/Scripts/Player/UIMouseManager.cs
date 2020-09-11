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
    }

    public void TryEndDrag(int unused)
    {
        // Check if hovered slot is there
        // If it is, Transfer Mouse Slot Data to Slot That is Hovered Over
        if (hoverSlotIndex == 12 && cachedSlotIndex != hoverSlotIndex)
        {
            // Dont Do Anything If Drag Item Onto Output Slot in Craft UI
        }
        else if (hoverSlotIndex >= 10 && cachedSlotIndex != hoverSlotIndex)
        {
            GameReferences.craftSystem.recipeSlots[hoverSlotIndex - 10].empty = mouseSlot.empty;
            GameReferences.craftSystem.recipeSlots[hoverSlotIndex - 10].count = mouseSlot.count;
            GameReferences.craftSystem.recipeSlots[hoverSlotIndex - 10].item.itemType = mouseSlot.item.itemType;
            GameReferences.craftSystem.recipeSlots[hoverSlotIndex - 10].item.tileType = mouseSlot.item.tileType;

            if (cachedSlotIndex == 12)
            {
                ClearSlot(GameReferences.craftSystem.outputSlot);
            }
            else if (cachedSlotIndex >= 10)
            {
                ClearSlot(GameReferences.craftSystem.recipeSlots[cachedSlotIndex - 10]);
            }
            else if(cachedSlotIndex >= 0)
            {
                ClearSlot(GameReferences.playerInv.slots[cachedSlotIndex]);

                GameReferences.playerInv.updateAllSlotsCallback();
            }

            GameReferences.craftSystem.updateRecipeSlotUI(hoverSlotIndex - 10);
        }
        else if (hoverSlotIndex >= 0 && cachedSlotIndex != hoverSlotIndex)
        {
            GameReferences.playerInv.slots[hoverSlotIndex].empty = mouseSlot.empty;
            GameReferences.playerInv.slots[hoverSlotIndex].count = mouseSlot.count;
            GameReferences.playerInv.slots[hoverSlotIndex].item.itemType = mouseSlot.item.itemType;
            GameReferences.playerInv.slots[hoverSlotIndex].item.tileType = mouseSlot.item.tileType;

            if (cachedSlotIndex == 12)
            {
                ClearSlot(GameReferences.craftSystem.outputSlot);
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

        ClearMouseSlot(0);
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
