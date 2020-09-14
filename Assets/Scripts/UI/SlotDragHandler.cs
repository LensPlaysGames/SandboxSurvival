using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace U_Grow
{
    public class SlotDragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private int slotIndex;

        public delegate void SlotDragHandlerEvent(int slotIndex);
        public SlotDragHandlerEvent SetMouseSlot;

        public SlotDragHandlerEvent EndDrag;

        public SlotDragHandlerEvent HoverOverSlot;
        public SlotDragHandlerEvent ClearHoverSlot;

        private GameObject empty;
        private GameObject imgContainer;

        void Awake()
        {
            empty = Resources.Load<GameObject>("Prefabs/EmptyImagePrefab");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // We have clicked the slot, Send event for mouse slot population with index of hovered over slot
            SetMouseSlot(slotIndex);

            imgContainer = Instantiate(empty, this.transform);
            imgContainer.transform.SetParent(GameReferences.playerInvUI.transform.Find("InventoryBackground").transform);
            imgContainer.transform.SetSiblingIndex(0);
            imgContainer.GetComponent<Image>().sprite = GlobalReferences.DDDOL.spriteDB[(int)GameReferences.uIMouseManager.MouseSlot.item.tileType];
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Move Slot Visual to Mouse Position
            imgContainer.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            EndDrag(-1);

            if (imgContainer != null) { Destroy(imgContainer); }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Hovering Over this Slot! Send Event to Set Hovered Slot to This Slots Index!
            HoverOverSlot(slotIndex);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // No Longer Hovering Over This Slot, Send Event to Set Hovered Slot to Null
            ClearHoverSlot(slotIndex);
        }
    }
}