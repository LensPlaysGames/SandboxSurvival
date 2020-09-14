namespace U_Grow
{
    public interface ISlotContainer
    {
        void SetSlot(int slotIndex, Slot slot);
        void TryAddToSlot(Slot slot);
        void TryTakeFromSlot(int slotIndex);
        void ModifySlotCount(int slotIndex, int amount);
        void ClearSlot(int slotIndex);
    }
}