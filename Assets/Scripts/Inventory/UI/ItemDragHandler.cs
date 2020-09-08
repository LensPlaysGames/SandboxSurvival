using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isHeld;

    private CanvasGroup canvasGroup;
    private Vector3 cachedPos;

    private GameObject player;

    private void Start() 
    {
        canvasGroup = GetComponent<CanvasGroup>();
        cachedPos = transform.localPosition;

        player = GameObject.Find("Player");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;

        // Copy Slot We Were Over to Held Item Data
        isHeld = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 moveTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        moveTo.z = 0f;
        transform.position = moveTo;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isHeld = false;

        transform.localPosition = cachedPos;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (player.GetComponent<HeldItem>().mousedOver.empty == true)
        {
            // Plop this slot into moused over one
            UnityEngine.Debug.Log("You are Over an Empty Slot");

            player.GetComponent<HeldItem>().AddItemToMousedOverSlot(); // At this slot, add held item
        } 
    }
}
