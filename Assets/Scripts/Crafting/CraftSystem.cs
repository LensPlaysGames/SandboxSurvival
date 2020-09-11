using System;
using UnityEngine;

public class CraftSystem
{
    public const int numberOfRecipeSlots = 2;

    public Slot[] recipeSlots;
    public Slot outputSlot;

    private Slot mouseDragSlot;

    public delegate void UICraftingEvent(int whatSlot);
    public UICraftingEvent updateRecipeSlotUI;

    public delegate void UIAllCraftingEvent();
    public UIAllCraftingEvent updateAllRecipeSlots;
    public UIAllCraftingEvent updateOutputSlotUI;

    public CraftSystem()
    {
        recipeSlots = new Slot[numberOfRecipeSlots];
        for (int i = 0; i < recipeSlots.Length; i++)
        {
            recipeSlots[i] = new Slot();
            recipeSlots[i].empty = true;
            recipeSlots[i].count = 0;
            recipeSlots[i].item = new Item();
            recipeSlots[i].item.itemType = Item.ItemType.Tile;
            recipeSlots[i].item.tileType = Tile.TileType.Air;
        }

        outputSlot = new Slot();
        outputSlot.item = new Item();
    }



    public void TryAddToSlot(Slot slot, int whatSlot)
    {
        if (recipeSlots[whatSlot] != null)
        {
            if (recipeSlots[whatSlot].empty == true) // Fill Slot
            {
                recipeSlots[whatSlot].empty = false;
                recipeSlots[whatSlot].count = slot.count;
                recipeSlots[whatSlot].item = slot.item;

                updateRecipeSlotUI(whatSlot);
            }
            else if (recipeSlots[whatSlot].item.tileType == slot.item.tileType) // Stack Tiles
            {
                recipeSlots[whatSlot].count++;

                updateRecipeSlotUI(whatSlot);
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("Crafting Slot " + whatSlot + " Was Not Found, Can Not Try to Add Item");
        }
    }



    public void TakeFromSlot(int whatSlot)
    {
        if (recipeSlots[whatSlot].count > 0) // Slot is Not Empty, Take From Slot
        {
            recipeSlots[whatSlot].count--;
            updateRecipeSlotUI(whatSlot);
        }
        if (recipeSlots[whatSlot].count <= 0) // Slot is Empty, Clear Slot Data
        {
            ClearSlot(whatSlot);
        }
    }

    public void ClearSlot(int whatSlot)
    {
        recipeSlots[whatSlot].empty = true;
        recipeSlots[whatSlot].count = 0;
        recipeSlots[whatSlot].item.itemType = Item.ItemType.Tile;
        recipeSlots[whatSlot].item.tileType = Tile.TileType.Air;

        updateRecipeSlotUI(whatSlot);
    }



    public Item GetItemAtSlot(int whatSlot)
    {
        if (recipeSlots[whatSlot] != null)
        {
            return recipeSlots[whatSlot].item;
        }
        else
        {
            UnityEngine.Debug.LogWarning("Crafting Slot " + whatSlot + " Was Not Found, Can Not Get Item");
            return null;
        }
    }
}
