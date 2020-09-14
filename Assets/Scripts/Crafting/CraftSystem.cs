using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U_Grow
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

        public CraftSystem(MonoBehaviour mb)
        {
            recipeSlots = new Slot[numberOfRecipeSlots];
            for (int i = 0; i < recipeSlots.Length; i++)
            {
                recipeSlots[i] = new Slot
                {
                    empty = true,
                    count = 0,
                    item = new Item
                    {
                        itemType = Item.ItemType.Tile,
                        tileType = Tile.TileType.Air
                    }
                };
            }

            outputSlot = new Slot
            {
                item = new Item()
            };

            mb.StartCoroutine(InitRecipesWaitForLoad(.5f));
        }
        IEnumerator InitRecipesWaitForLoad(float x)
        {
            yield return new WaitForSeconds(x);
            recipes = GameReferences.listOfRecipes.recipes;
            Debug.Log("Initialized Recipes for CraftSystem");
        }

        public void AddToSlot(int whatSlot, Slot slot)
        {
            if (recipeSlots[whatSlot] != null)
            {
                if (recipeSlots[whatSlot].empty == true) // Fill Slot
                {
                    recipeSlots[whatSlot].empty = false;
                    recipeSlots[whatSlot].count = slot.count;
                    recipeSlots[whatSlot].item.itemType = slot.item.itemType;
                    recipeSlots[whatSlot].item.tileType = slot.item.tileType;

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

                TryCraft();
            }
            else
            {
                Debug.LogWarning("Crafting Slot " + whatSlot + " Was Not Found, Can Not Try to Add Item");
            }
        }
        public void TryAddToSlot(Slot slot)
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

            TryCraft();
        }
        public void TryTakeFromSlot(int whatSlot)
        {
            if (recipeSlots[whatSlot].count > 0) // Slot is Not Empty, Take From Slot
            {
                recipeSlots[whatSlot].count--;

                updateRecipeSlotUI?.Invoke(whatSlot);
            }
            if (recipeSlots[whatSlot].count <= 0) // Slot is Empty, Clear Slot Data
            {
                ClearSlot(whatSlot);
            }

            TryCraft();
        }
        public void ModifySlotCount(int whatSlot, int amount)
        {
            recipeSlots[whatSlot].count += amount;
            Mathf.Clamp(recipeSlots[whatSlot].count, 0f, 90f);

            updateRecipeSlotUI?.Invoke(whatSlot);

            TryCraft();
        }
        public void ClearSlot(int whatSlot)
        {
            recipeSlots[whatSlot].empty = true;
            recipeSlots[whatSlot].count = 0;
            recipeSlots[whatSlot].item.itemType = Item.ItemType.Tile;
            recipeSlots[whatSlot].item.tileType = Tile.TileType.Air;

            updateRecipeSlotUI?.Invoke(whatSlot);

            TryCraft();
        }

        public void ClearOutputSlot()
        {
            outputSlot.empty = true;
            outputSlot.count = 0;
            outputSlot.item.itemType = Item.ItemType.Tile;
            outputSlot.item.tileType = Tile.TileType.Air;

            updateOutputSlotUI?.Invoke();

            TryCraft();
        }

        private void TryCraft()
        {
            Debug.Log("Trying to Craft!");

            Slot output = GetRecipeOutput();

            if (output != null)
            {
                Debug.Log("Setting Output Slot!");

                outputSlot = output;

                updateOutputSlotUI?.Invoke();
            }
            else // Output is Null, Clear Output Slot
            {
                Debug.Log("No Output, Clearing Slot!");

                ClearOutputSlot();
            }
        }

        public void SpendRecipeIngredients()
        {
            Debug.Log($"Spending {cachedRecipe.name} Ingredients!");

            for (int i = 0; i < cachedRecipe.ingredients.Length; i++)
            {
                for (int c = 0; c < cachedRecipe.ingredients[i].count; c++)
                {
                    TryTakeFromSlot(i);
                }
            }
        }

        private Slot GetRecipeOutput()
        {
            Slot output = new Slot
            {
                count = 0,
                empty = true,
                item = new Item
                {
                    itemType = Item.ItemType.Tile,
                    tileType = Tile.TileType.Air
                }
            };

            for (int r = 0; r < recipes.Count; r++)
            {
                Debug.Log("Testing Recipe " + recipes[r].name);

                output = recipes[r].output;

                for (int slot = 0; slot < recipeSlots.Length; slot++)
                {
                    if (recipeSlots[slot].item.itemType != recipes[r].ingredients[slot].item.itemType)
                    {
                        Debug.Log("Can Not Craft: itemType does not match!");
                        output = null;
                    }
                    else // Item Type Matches
                    {
                        if (recipeSlots[slot].item.tileType != recipes[r].ingredients[slot].item.tileType) // But Tile Type Does Not
                        {
                            Debug.Log("Can Not Craft: tileType does not match!");
                            output = null;
                        }
                    }
                }
                if (output != null) 
                {
                    Debug.Log("Valid Recipe Found: " + recipes[r].name);
                    cachedRecipe = recipes[r];
                    return output;
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