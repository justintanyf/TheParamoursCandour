using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        if (item != null) // TODO: need more checks to see if it can be bought
        {
            itemDescriptonPanelTMP.SetText(item.name + "purchase!");
            // TODO: save item to inventory, deduct materials
            moveScript.AddToLog("You have successfully purchase " + item.name + ".");
            // TODO: remove item visibility from charstats and update ui if needed
            if (item.name == "Caesura") // TODO: update name
            {
                CharStats.fragmentsFound[4] = true;
                moveScript.AddToLog("You've found a memory fragment! The memory hits you quite suddenly - in it, Auri is giving you a wooden ring which she says keeps your secrets.......");
                moveScript.AddToLog("This memory fragment has also unlocked the Naming subtree.");
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
