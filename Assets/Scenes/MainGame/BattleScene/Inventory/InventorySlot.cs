using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public Button removeButton;
    
    private Item item;

    public GameObject itemDescriptonPanel;
    public TMP_Text itemDescriptonPanelTMP;

    public int numberOfItems;
    public int index;

    public Button confirmItemUsageButton;
    public Button closeItemDescriptionButton;

    private static bool clickedOnItemDescription = false;

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        // numberOfItems = num;
        // index = ind;
        /*
         * If you want to have the removeButton work, uncomment the button
         * removeButton.interactable = true;
         */
    }

    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;
        /*
         * If you want to have the removeButton work, uncomment the button
         * removeButton.interactable = false;
         */
    }
    public void RemoveItem()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        numberOfItems = 0;
        index = -1;
    }

    public void OnRemoveButton()
    {
        Inventory.instance.Remove(item);
    }
    
    public void useItem()
    {
        if (item != null)
        {
            itemDescriptonPanel.gameObject.SetActive(true); //setActive
            Inventory.instance.SetCurrentlyUsedItem(this.item);
            itemDescriptonPanelTMP.SetText(item.descriptionOfItem);
            clickedOnItemDescription = true;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            itemDescriptonPanel.gameObject.SetActive(true); //setActive
            itemDescriptonPanelTMP.SetText(item.descriptionOfItem);
        }
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        if (clickedOnItemDescription)
        {
            return;
        }
        itemDescriptonPanel.gameObject.SetActive(false); // deactivate the component
        itemDescriptonPanelTMP.SetText("");
    }

    public void OnUseItemConfirmation()
    {
        Inventory.instance.currentlySelectedItem.Use();
        itemDescriptonPanel.gameObject.SetActive(false); // deactivate the component
        clickedOnItemDescription = false;
    }

    public void OnCloseItemDescriptionCanvas()
    {
        itemDescriptonPanel.gameObject.SetActive(false); // deactivate the component
        clickedOnItemDescription = false;
    }
}
