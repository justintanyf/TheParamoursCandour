using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ChestSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    private Item item;
    public TMP_Text quantity;
    public TMP_Text itemDescriptonPanelTMP;
    public TMP_InputField deleteInput, addToInventoryInput;
    public Button itemButton, addToChestButton, deleteFromChestButton, addToInventoryButton;
    public Canvas selectedCanvas;
    public ChestUI chestUI;
    public int index;

    public void AddItem(Item newItem, int num, int ind)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        quantity.text = num.ToString();
        index = ind;
    }

    public void RemoveItem()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        quantity.text = "";
        index = -1;
    }

    public void Deselect()
    {
        selectedCanvas.gameObject.SetActive(false);
    }
    
    public void OnItemClick()
    {
        if (item == null) return;

        if (chestUI.selectedIndex == -1) // nothing selected
        {
            selectedCanvas.gameObject.SetActive(true);
            chestUI.selectedIndex = index;
            addToChestButton.gameObject.SetActive(false);
            deleteInput.gameObject.SetActive(true);
            deleteFromChestButton.gameObject.SetActive(true);
            addToInventoryInput.gameObject.SetActive(true);
            addToInventoryButton.gameObject.SetActive(true);
            deleteInput.text = "";
            addToInventoryInput.text = "";
        }
        else if (chestUI.selectedIndex == this.index) // this was selected again
        {
            selectedCanvas.gameObject.SetActive(false);
            chestUI.selectedIndex = -1;
            addToChestButton.gameObject.SetActive(true);
            deleteInput.gameObject.SetActive(false);
            deleteFromChestButton.gameObject.SetActive(false);
            addToInventoryInput.gameObject.SetActive(false);
            addToInventoryButton.gameObject.SetActive(false);
            deleteInput.text = "";
            addToInventoryInput.text = "";
        }
        else // something else was selected previously
        {
            chestUI.DeselectAll();
            selectedCanvas.gameObject.SetActive(true);
            chestUI.selectedIndex = index;
            addToChestButton.gameObject.SetActive(false);
            deleteInput.gameObject.SetActive(true);
            deleteFromChestButton.gameObject.SetActive(true);
            addToInventoryInput.gameObject.SetActive(true);
            addToInventoryButton.gameObject.SetActive(true);
            deleteInput.text = "";
            addToInventoryInput.text = "";
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            //itemDescriptonPanelTMP.SetText(item.descriptionOfItem);
        }
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        //itemDescriptonPanelTMP.SetText("");
    }
}
