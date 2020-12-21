using System.Collections.Generic;
using UnityEngine;

namespace LensorRadii.U_Grow
{
    public class CraftSystem : ISlotContainer
    {
        public const int numberOfRecipeSlots = 2;

        public Slot[] recipeSlots;
        public Slot outputSlot;

        private List<Recipe> recipes = new List<Recipe>();
        private Recipe cachedRecipe;

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
            }

            outputSlot = new Slot();
        }

        public void AddToSlot(int whatSlot, Slot slot)
        {
            if (recipeSlots[whatSlot] != null)
            {
                if (recipeSlots[whatSlot].empty == true) // Fill Slot
                {
                    SetSlot(whatSlot, slot);

                    updateRecipeSlotUI?.Invoke(whatSlot);
                }
                else if (recipeSlots[whatSlot].item.itemType == slot.item.itemType)
                {
                    if (recipeSlots[whatSlot].item.tileType == slot.item.tileType) // Stack Tiles
                    {
                        recipeSlots[whatSlot].count++;

                        updateRecipeSlotUI?.Invoke(whatSlot);
                    }
                }

                TryCraft();                                                 // Check for valid recipe and craft if one is found
            }
            else
            {
                Debug.LogWarning("Crafting Slot " + whatSlot + " Was Not Found, Can Not Try to Add Item");
            }
        }
        public void TryAddToSlot(Slot slot) // No need to try to add to any available recipe slot
        {
            return;
        }
        public void SetSlot(int whatSlot, Slot slot)
        {
            recipeSlots[whatSlot].empty = slot.empty;
            recipeSlots[whatSlot].count = slot.count;
            recipeSlots[whatSlot].item.itemType = slot.item.itemType;
            recipeSlots[whatSlot].item.tileType = slot.item.tileType;

            updateRecipeSlotUI?.Invoke(whatSlot);

            TryCraft();                                                     // Check for valid recipe and craft if one is found
        }
        public void TryTakeFromSlot(int whatSlot)
        {
            if (recipeSlots[whatSlot].count > 0)                            // Slot is Not Empty, Take From Slot
            {
                recipeSlots[whatSlot].count--;

                updateRecipeSlotUI?.Invoke(whatSlot);
            }
            if (recipeSlots[whatSlot].count <= 0)                           // Slot is Empty, Clear Slot Data
            {
                ClearSlot(whatSlot);
            }

            TryCraft();                                                     // Check for valid recipe and craft if one is found
        }
        public void ModifySlotCount(int whatSlot, int amount)
        {
            recipeSlots[whatSlot].count += amount;
            Mathf.Clamp(recipeSlots[whatSlot].count, 0f, 90f);

            updateRecipeSlotUI?.Invoke(whatSlot);

            TryCraft();                                                     // Check for valid recipe and craft if one is found
        }
        public void ClearSlot(int whatSlot)
        {
            recipeSlots[whatSlot].empty = true;
            recipeSlots[whatSlot].count = 0;
            recipeSlots[whatSlot].item.itemType = Item.ItemType.Tile;
            recipeSlots[whatSlot].item.tileType = Tile.TileType.Air;

            updateRecipeSlotUI?.Invoke(whatSlot);

            TryCraft();                                                     // Check for valid recipe and craft if one is found
        }

        public void ClearOutputSlot()                                       // DO NOT PUT TryCraft() IN THIS METHOD, LOOPS TO INFINITY
        {
            outputSlot.empty = true;
            outputSlot.count = 0;
            outputSlot.item.itemType = Item.ItemType.Tile;
            outputSlot.item.tileType = Tile.TileType.Air;

            updateOutputSlotUI?.Invoke();
        }

        private void TryCraft()
        {
            Slot output = GetRecipeOutput();                                // Check for recipe with GetRecipeOutput()

            if (output != null)                                             // If one is found, populate the output crafting slot and update output slot UI
            {
                outputSlot = output;
                updateOutputSlotUI?.Invoke();
            }
            else                                                            // If one is NOT found, make sure the output slot has nothing in it (this may have ramifications)
            {
                ClearOutputSlot();
            }
        }

        public void SpendRecipeIngredients()
        {
            Debug.Log($"Spending {cachedRecipe.name} Ingredients!");

            for (int i = 0; i < cachedRecipe.ingredients.Length; i++)       // For each ingredient in the recipe
            {
                ModifySlotCount(i, -cachedRecipe.ingredients[i].count);     // Remove used ingredients
            }
        }


        private Slot GetRecipeOutput()
        {
            recipes = GameReferences.listOfRecipes.recipes;                                                     // Get reference to all recipes
            for (int r = 0; r < recipes.Count; r++)                                                             // For every recipe found,
            {
                Slot output = new Slot                                                                          // Construct output slot populated with recipe output data
                {
                    count = recipes[r].output.count,
                    empty = recipes[r].output.empty,
                    item = new Item
                    {
                        itemType = recipes[r].output.item.itemType,
                        tileType = recipes[r].output.item.tileType,
                    }
                };

                for (int slot = 0; slot < recipeSlots.Length; slot++)                                           // For every ingredient in recipe,
                {
                    if (recipeSlots[slot].item.itemType != recipes[r].ingredients[slot].item.itemType)          // If Item Type Does Not Match,
                    {
                        output = null;                                                                          // Null output
                    }
                    else                                                                                        // Or, If Item Type Matches,
                    {
                        if (recipeSlots[slot].item.tileType != recipes[r].ingredients[slot].item.tileType)      // And Tile Type Does Not Match
                        {
                            output = null;                                                                      // Null output
                        }
                    }
                }
                if (output != null)                         // If output was not nulled at this point, the current recipe is valid
                {
                    cachedRecipe = recipes[r];              // Set 'cachedRecipe' variable to be able to spend the correct ingredients when the player actually crafts this
                    return output;                          // Return output slot
                }
            }
            return null;                                    // No valid recipes, return null
        }

        public void OnTakeCraftedItem()                   // Used by the mouse manager when the player takes from the output slot
        {
            ClearOutputSlot();
            SpendRecipeIngredients();
        }


        public Slot GetRecipeSlot(int whatSlot)
        {
            if (recipeSlots[whatSlot] != null)
            {
                return recipeSlots[whatSlot];
            }
            else
            {
                Debug.LogWarning("Recipe Crafting Slot " + whatSlot + " Was Not Found, Can Not Get Slot");
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
                Debug.LogWarning("Output Crafting Slot Was Not Found, Can Not Get Slot");
                return null;
            }
        }
    }
}