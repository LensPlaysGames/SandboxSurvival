namespace LensorRadii.U_Grow
{
    public interface ISlotContainer
    {

        /*
        - SetSlot()             Use slot index and slot to overwrite a slot in the inventory with passed slot

        - TryAddToSlot()        Use a slot and try to merge it anywhere it can fit in the available slots

        - ModifySlotCount()     Use slot index and an integer, 'amount' (default 1), to add an amount to the slot at the passed slot index

        - ClearSlot()           Use slot index to set the slot back to defaults (empty)
        */

        void SetSlot(int slotIndex, Slot slot);
        void TryAddToSlot(Slot slot);
        void ModifySlotCount(int slotIndex, int amount);
        void ClearSlot(int slotIndex);
    }
}