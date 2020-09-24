using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LensorRadii.U_Grow
{
    public class Chest : MonoBehaviour, IInteracteable, ISlotContainer
    {
        private const int maxChestSize = 77;

        public int numberOfSlots = 18;
        public int maxStackSize = 90;

        public int x, y;

        public List<Slot> slots = new List<Slot>();
        private List<GameObject> slotParents = new List<GameObject>();
        private List<TextMeshProUGUI> countTexts = new List<TextMeshProUGUI>();

        private GameObject empty;
        private GameObject slotPrefab;

        private bool loaded = false;
        private void Start()
        {
            // Prefab for UI image
            empty = Resources.Load<GameObject>("Prefabs/EmptyImagePrefab");
            slotPrefab = Resources.Load<GameObject>("Prefabs/Slot");

            Level level = GameReferences.levelGenerator.GetLevelInstance();

            x = (int)(transform.position.x / level.Scale);
            y = (int)(transform.position.y / level.Scale);

            #region Initialize slots Data

            if (numberOfSlots > maxChestSize) { numberOfSlots = maxChestSize; }

            for (int s = 0; s < numberOfSlots; s++)
            {
                Slot slot = new Slot();
                slots.Add(slot);
            }

            Debug.Log("Chest Initialized.");
            Debug.Log("Slots Initialized: " + slots.Count);

            #endregion

            StartCoroutine(LoadChestAfterX(.2f));
        }

        private IEnumerator LoadChestAfterX(float x)
        {
            yield return new WaitForSeconds(x);
            if (GameReferences.levelGenerator.GetLevelInstance()?.GetTileDataAt(this.x, y)?.inventorySlots != null)
            {
                LoadChest();
            }
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

            for (int s = 0; s < slots.Count; s++) // Check for stackable slots
            {
                if (slots[s].item.tileType == slot.item.tileType)
                {
                    if (slots[s].count + slot.count < maxStackSize)
                    {
                        Debug.Log("Found Stackable Slot in Chest!");

                        slots[s].count += slot.count;

                        itemDealt = true;

                        UpdateSlotUI(s);
                        break;
                    }
                }
            }

            if (!itemDealt) // Check for empty slots
            {
                for (int s = 0; s < slots.Count; s++)
                {
                    if (slots[s].empty)
                    {
                        Debug.Log("Found Empty Slot in Chest!");

                        slots[s].empty = false;
                        slots[s].count = slot.count;
                        slots[s].item.itemType = slot.item.itemType;
                        slots[s].item.tileType = slot.item.tileType;

                        itemDealt = true;

                        UpdateSlotUI(s);
                        break;
                    }
                }
            }

            if (!itemDealt) // No Available Slots
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
                slots[slotIndex].empty = true;

                ClearSlot(slotIndex);
            }
        }

        public void Use()
        {
            if (!GameReferences.uIHandler.inMenu)
            {
                Debug.Log("Opening Chest!");
                Debug.Log("Chest Capacity: " + numberOfSlots);
                Debug.Log("Slots Found: " + slots.Count);
                Debug.Log("Slot Parents Found (should be 0): " + slotParents.Count);

                GameReferences.uIMouseManager.interactingChest = this;

                GameReferences.uIHandler.inMenu = true;
                GameReferences.uIHandler.ExitMenu += ExitUI; // THIS IS NOT ASS

                GameReferences.chestUI.gameObject.SetActive(true);

                // Fill Chest UI with SlotPrefabs (Instantiate), Fill slotParents List from Newly Created Prefabs (.Add())
                for (int s = 0; s < slots.Count; s++)
                {
                    GameObject slot = Instantiate(slotPrefab, GameReferences.chestUI.transform.Find("ChestBG"));
                    slot.GetComponent<SlotDragHandler>().slotIndex = s + 13; // THIS IS ASS
                    slotParents.Add(slot);
                    countTexts.Add(slot.transform.Find("CountTextPanel").transform.Find("Slot Count").GetComponent<TextMeshProUGUI>());
                }

                Debug.Log("Slot Parents Found: " + slotParents.Count);

                // Update UI
                UpdateUI();
            }
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

            if (slotParents[slotIndex].transform.Find("EmptyImagePrefab(Clone)")?.gameObject != null)
            {
                Destroy(slotParents[slotIndex].transform.Find("EmptyImagePrefab(Clone)").gameObject);
            }

            if (slotData.count != 0)
            {
                GameObject item = Instantiate(empty, slotParents[slotIndex].transform);
                item.transform.SetSiblingIndex(0);
                item.GetComponent<Image>().sprite = GlobalReferences.DDDOL.spriteDB[(int)slotData.item.tileType];
            }

            countTexts[slotIndex].text = slotData.count.ToString();
        }

        public void ExitUI()
        {
            for (int s = 0; s < slotParents.Count; s++)
            {
                Destroy(slotParents[s]);
            }
            slotParents.Clear();
            countTexts.Clear();

            GameReferences.uIHandler.inMenu = false;
            GameReferences.chestUI.gameObject.SetActive(false);

            GameReferences.uIMouseManager.interactingChest = null;

            Level level = GameReferences.levelGenerator.GetLevelInstance();
            ExtraTileData data = new ExtraTileData(level, x, y);
            data.SetSlots(GetSlotsToSave());
            level.tileDatas[x, y] = data;
        }

        public List<Slot> GetSlotsToSave()
        {
            List<Slot> slotsToSave = new List<Slot>();

            for (int s = 0; s < slots.Count; s++)
            {
                Slot slot = new Slot();

                slotsToSave.Add(slot);
            }

            return slotsToSave;
        }

        public void LoadChest()
        {
            List<Slot> savedSlots = GameReferences.levelGenerator.GetLevelInstance().GetTileDataAt(x, y).inventorySlots;

            for (int s = 0; s < savedSlots.Count; s++)
            {
                slots[s].empty = savedSlots[s].empty;
                slots[s].count = savedSlots[s].count;
                slots[s].item.itemType = savedSlots[s].item.itemType;
                slots[s].item.tileType = savedSlots[s].item.tileType;
            }

            UpdateUI();
        }
    }
}