using UnityEngine;

public class HeldItem : MonoBehaviour
{
    public static HeldItem instance;
    public static Inventory inventory;
    public static InventoryUI invUI;

    public int heldSlotIndex;
    public int mousedOverIndex;

    public Slot heldSlot;
    public Slot mousedOver;

    void Start()
    {
        inventory = GameObject.Find("Player").GetComponent<Inventory>();
        invUI = GameObject.Find("UICanvas").transform.Find("--InventoryUI--").GetComponent<InventoryUI>(); ;
    }

    void FixedUpdate()
    {
        ItemDragHandler[] slots = invUI.GetComponentsInChildren<ItemDragHandler>();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].isHeld == true)
            {
                heldSlotIndex = i;
                heldSlot = inventory.slots[i];
            }
        }

        MouseOver[] s = invUI.GetComponentsInChildren<MouseOver>();
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i].mouseOver == true)
            {
                mousedOverIndex = i;
                mousedOver = inventory.slots[i];
            }
        }
    }

    public void AddItemToMousedOverSlot()
    {
        inventory.slots[mousedOverIndex] = heldSlot;
        inventory.slots[heldSlotIndex] = null;
        heldSlot = null;

        GetComponent<Inventory>().updateAllSlotsCallback?.Invoke();
    }
}
