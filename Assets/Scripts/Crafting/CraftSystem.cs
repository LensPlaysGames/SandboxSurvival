using System;
using UnityEngine;

public class CraftSystem
{
    public const int numberOfRecipeSlots = 2;

    public Slot[] recipeSlots;
    public Slot outputSlot;

    private Slot mouseDragSlot;

    private Slot[] recipe;

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

        recipe = new Slot[numberOfRecipeSlots];
        for (int rSlot = 0; rSlot < recipe.Length; rSlot++)
        {
            recipe[rSlot] = new Slot();
            recipe[rSlot].empty = true;
            recipe[rSlot].count = 1;
            recipe[rSlot].item = new Item();
            recipe[rSlot].item.itemType = Item.ItemType.Tile;
            recipe[rSlot].item.tileType = Tile.TileType.Adobe;
        }
    }



    public void TryAddToRecipeSlot(Slot slot, int whatSlot)
    {
        if (recipeSlots[whatSlot] != null)
        {
            if (recipeSlots[whatSlot].empty == true) // Fill Slot
            {
                recipeSlots[whatSlot].empty = false;
                recipeSlots[whatSlot].count = slot.count;
                recipeSlots[whatSlot].item.itemType = slot.item.itemType;
                recipeSlots[whatSlot].item.tileType = slot.item.tileType;

                updateRecipeSlotUI(whatSlot);
            }
            else if (recipeSlots[whatSlot].item.itemType == slot.item.itemType) 
            {
                if (recipeSlots[whatSlot].item.tileType == slot.item.tileType) // Stack Tiles
                {
                    recipeSlots[whatSlot].count++;

                    updateRecipeSlotUI(whatSlot);
                }
            }

            TryCraft();
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
            ClearRecipeSlot(whatSlot);
        }

        TryCraft();
    }

    public void ClearRecipeSlot(int whatSlot)
    {
        recipeSlots[whatSlot].empty = true;
        recipeSlots[whatSlot].count = 0;
        recipeSlots[whatSlot].item.itemType = Item.ItemType.Tile;
        recipeSlots[whatSlot].item.tileType = Tile.TileType.Air;

        TryCraft();

        updateRecipeSlotUI(whatSlot);
    }

    public void ClearOutputSlot()
    {
        outputSlot.empty = true;
        outputSlot.count = 0;
        outputSlot.item.itemType = Item.ItemType.Tile;
        outputSlot.item.tileType = Tile.TileType.Air;

        updateOutputSlotUI();
    }



    private void TryCraft()
    {
        Slot output = GetRecipeOutput();
        if (output != null)
        {
            outputSlot = output;

            updateOutputSlotUI();
        }
        else // Output is Null, Clear Output Slot
        {
            ClearOutputSlot();
        }
    }

    public void SpendRecipeIngredients()
    {
        for (int i = 0; i < recipe.Length; i++)
        {
            for (int c = 0; c < recipe[i].count; c++)
            {
                UnityEngine.Debug.Log("Taking from Recipe Slot " + i);
                TakeFromSlot(i);
            }
        }
    }

    private Slot GetRecipeOutput()
    {
        Slot output = new Slot();

        output.empty = false;
        output.count = 3;
        output.item = new Item();
        output.item.itemType = Item.ItemType.Tile;
        output.item.tileType = Tile.TileType.AdobeBricks;

        for (int slot = 0; slot < recipeSlots.Length; slot++)
        {
            if (recipeSlots[slot].item.itemType != recipe[slot].item.itemType)
            {
                output = null;
            }
            else // Item Type Matches
            {
                if (recipeSlots[slot].item.tileType != recipe[slot].item.tileType) // But Tile Type Does Not
                {
                    output = null;
                }   
            }
        }

        return output;
    }



    public Slot GetRecipeSlot(int whatSlot)
    {
        if (recipeSlots[whatSlot] != null)
        {
            return recipeSlots[whatSlot];
        }
        else
        {
            UnityEngine.Debug.LogWarning("Recipe Crafting Slot " + whatSlot + " Was Not Found, Can Not Get Slot");
            return null;
        }
    }

    public Slot GetOutputSlot()
    {
        if (outputSlot != null)
        {
            return outputSlot;
        }
        else
        {
            UnityEngine.Debug.LogWarning("Output Crafting Slot Was Not Found, Can Not Get Slot");
            return null;
        }
    }
}
