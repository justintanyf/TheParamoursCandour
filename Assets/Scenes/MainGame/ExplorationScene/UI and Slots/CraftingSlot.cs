using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CraftingSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;    
    private Item item;
    public TMP_Text itemDescriptonPanelTMP;
    public Move moveScript;

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
    }
    
    public void useItem()
    {
        if (item != null) // TODO: need more checks to see if it can be crafted
        {
            itemDescriptonPanelTMP.SetText(item.name + "crafted!");
            // TODO: save item to inventory, deduct materials
            moveScript.AddToLog(item.name + " has been successfully crafted.");
            // TODO: remove item visibility from charstats and update ui if needed
            if (!CharStats.fragmentsFound[5]) // TODO: update name
            {
                CharStats.fragmentsFound[5] = true;
                moveScript.AddToLog("You've found a memory fragment! The memory hits you quite suddenly - in it, you are toiling away at the Fishery as you struggle to afford the fees for the upcoming year.......");
                moveScript.AddToLog("This memory fragment has also unlocked the Sygaldry subtree.");
                // call verification to store in db
            }
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            itemDescriptonPanelTMP.SetText(item.descriptionOfItem);
        }
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        itemDescriptonPanelTMP.SetText("");
    }
}
