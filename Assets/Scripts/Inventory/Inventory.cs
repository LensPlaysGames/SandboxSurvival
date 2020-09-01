using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public int selectedSlotIndex;
    public Slot selectedSlot;
    public GameObject slotSelector;

    public Slot[] slots;
    public Sprite[] sprites;

    public GameObject empty;

    private int a;
    
    void Start()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogError("Multiple Inventories attempting to Initialize");
        }
        else { instance = this; }

        foreach (Slot slot in slots)
        {
            a++;
            slot.slotParent = GameObject.Find("Slot (" + a + ")");
            slot.countText = GameObject.Find("Slot (" + a + ") Count");
            slot.tileType = Tile.TileType.Air;
            slot.empty = true;
            slot.count = 0;
            slot.sprite = null;
        }

        if (selectedSlot.slotParent == null)
        {
            selectedSlot = slots[0]; 
            slotSelector.transform.position = slots[0].slotParent.transform.position;
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { selectedSlot = slots[0]; slotSelector.transform.position = slots[0].slotParent.transform.position; selectedSlotIndex = 0; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { selectedSlot = slots[1]; slotSelector.transform.position = slots[1].slotParent.transform.position; selectedSlotIndex = 1; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { selectedSlot = slots[2]; slotSelector.transform.position = slots[2].slotParent.transform.position; selectedSlotIndex = 2; }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { selectedSlot = slots[3]; slotSelector.transform.position = slots[3].slotParent.transform.position; selectedSlotIndex = 3; }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { selectedSlot = slots[4]; slotSelector.transform.position = slots[4].slotParent.transform.position; selectedSlotIndex = 4; }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { selectedSlot = slots[5]; slotSelector.transform.position = slots[5].slotParent.transform.position; selectedSlotIndex = 5; }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { selectedSlot = slots[6]; slotSelector.transform.position = slots[6].slotParent.transform.position; selectedSlotIndex = 6; }
        if (Input.GetKeyDown(KeyCode.Alpha8)) { selectedSlot = slots[7]; slotSelector.transform.position = slots[7].slotParent.transform.position; selectedSlotIndex = 7; }
        if (Input.GetKeyDown(KeyCode.Alpha9)) { selectedSlot = slots[8]; slotSelector.transform.position = slots[8].slotParent.transform.position; selectedSlotIndex = 8; }
        if (Input.GetKeyDown(KeyCode.Alpha0)) { selectedSlot = slots[9]; slotSelector.transform.position = slots[9].slotParent.transform.position; selectedSlotIndex = 9; }
    }

    public void AddItemToSlot(Tile.TileType tileType)
    {
        bool itemDealt = false;

        for (int s1 = 0; s1 < slots.Length; s1++)
        {
            for (int s = 0; s < slots.Length; s++)
            {
                if (tileType == slots[s].tileType)
                {
                    // STACK ITEMS
                    slots[s].count++;

                    itemDealt = true;

                    UpdateSlotUI(s);
                    break;
                }
            }
            if (itemDealt)
            {
                break;
            }
            else if (tileType != slots[s1].tileType)
            {
                // MAKE NEW STACK
                if (slots[s1].empty)
                {
                    slots[s1].tileType = tileType;
                    slots[s1].empty = false;
                    slots[s1].count = 0;
                    slots[s1].count++;

                    slots[s1].sprite = sprites[(int)tileType];

                    itemDealt = true;

                    UpdateSlotUI(s1);
                    break;
                }
            }
        }
    }

    public void TakeFromSlot(Slot s)
    {
        s.count--;
        UpdateSlotUI(selectedSlotIndex);
    }

    void UpdateSlotUI(int slotNum) 
    {
        // If Image Exists In Slot, Destroy It
        if (slots[slotNum].slotParent.transform.Find("EmptyImagePrefab(Clone)") != null)
        {
            Destroy(slots[slotNum].slotParent.transform.Find("EmptyImagePrefab(Clone)").gameObject);
        }
        // If slot is populated, create image and update text
        if (slots[slotNum].count != 0)
        {
            GameObject item = Instantiate(empty, slots[slotNum].slotParent.transform);
            item.transform.SetSiblingIndex(item.transform.GetSiblingIndex() - 1);
            item.GetComponent<Image>().sprite = slots[slotNum].sprite;
            slots[slotNum].countText.GetComponent<TextMeshProUGUI>().text = slots[slotNum].count.ToString();
        }
        else // NO MORE ITEMS IN SLOT, REMOVE ATTRIBUTES
        {
            if (slots[slotNum].slotParent.transform.Find("EmptyImagePrefab(Clone)") != null)
            {
                Destroy(slots[slotNum].slotParent.transform.Find("EmptyImagePrefab(Clone)").gameObject);
            }

            slots[slotNum].tileType = Tile.TileType.Air;
            slots[slotNum].empty = true;
            slots[slotNum].count = 0;
            slots[slotNum].countText.GetComponent<TextMeshProUGUI>().text = "";
            slots[slotNum].sprite = null;
        }
    }
}
