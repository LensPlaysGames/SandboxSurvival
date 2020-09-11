using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CraftUI : MonoBehaviour
{
    public static CraftUI instance;

    void Awake()
    {
        if (instance != null) 
        { 
            UnityEngine.Debug.LogError("Multiple Crafting UIs, :|"); 
            Destroy(this.gameObject); 
        }
        else 
        { 
            instance = this; 
            GameReferences.craftUI = instance; 
        }
    }

    private CraftSystem craftSystem;

    [SerializeField]
    private List<Transform> recipeSlots;
    [SerializeField]
    private List<TextMeshProUGUI> countTexts;

    [SerializeField]
    private Transform outputSlot;
    [SerializeField]
    private TextMeshProUGUI outputText;

    private GameObject empty;

    int a = 0;
    void Start()
    {
        empty = Resources.Load<GameObject>("Prefabs/EmptyImagePrefab");

        craftSystem = GameReferences.craftSystem;

        Transform BGContainer = transform.Find("CraftMenuBackground");
        for (int s = 0; s < CraftSystem.numberOfRecipeSlots; s++)
        {
            a++;
            Transform recipeSlotTransform = BGContainer.Find("Recipe Slot (" + (s + 1) + ")");
            recipeSlots.Insert(s, recipeSlotTransform);
            countTexts.Insert(s, recipeSlotTransform.Find("CountTextPanel").Find("Slot Count").GetComponent<TextMeshProUGUI>());
        }

        outputSlot = BGContainer.Find("Output Slot");
        outputText = outputSlot.Find("CountTextPanel").Find("Slot Count").GetComponent<TextMeshProUGUI>();

        craftSystem.updateRecipeSlotUI += UpdateRecipeSlot;
        craftSystem.updateAllRecipeSlots += UpdateAllRecipeSlots;
    }

    void UpdateRecipeSlot(int slotIndex)
    {
        Slot slotData = craftSystem.recipeSlots[slotIndex];

        if (recipeSlots[slotIndex].Find("EmptyImagePrefab(Clone)") != null)
        {
            Destroy(recipeSlots[slotIndex].Find("EmptyImagePrefab(Clone)").gameObject);
        }

        if (slotData.count != 0) // Something in Slot, Update UI
        {
            GameObject image = Instantiate(empty, recipeSlots[slotIndex]);
            image.transform.SetSiblingIndex(0);
            image.GetComponent<SpriteRenderer>().sprite = GlobalReferences.DDDOL.spriteDB[(int)slotData.item.tileType];
            countTexts[slotIndex].text = slotData.count.ToString();
            countTexts[slotIndex].transform.localPosition = Vector3.zero;
        }
        else // Slot is Empty, Destroy any Visual there
        {
            if (recipeSlots[slotIndex].Find("EmptyImagePrefab(Clone)") != null)
            {
                Destroy(recipeSlots[slotIndex].Find("EmptyImagePrefab(Clone)").gameObject);
            }

            countTexts[slotIndex].text = "";
        }
    }

    void UpdateAllRecipeSlots()
    {
        for (int i = 0; i < recipeSlots.Count; i++)
        {
            UpdateRecipeSlot(i);
        }
    }

}
