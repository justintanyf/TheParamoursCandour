using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SubInventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    private Item item;
    public TMP_Text quantity;
    public TMP_Text itemDescriptonPanelTMP;
    public TMP_InputField addFinalInput;
    public Button itemButton, addFinalButton, backToChestButton;
    public Canvas selectedCanvas;
    public SubInventoryUI subInvUI;
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
        
        if (subInvUI.selectedIndex == -1) // nothing selected
        {
            selectedCanvas.gameObject.SetActive(true);
            subInvUI.selectedIndex = index;
            backToChestButton.gameObject.SetActive(false);
            addFinalInput.gameObject.SetActive(true);
            addFinalButton.gameObject.SetActive(true);
            addFinalInput.text = "";
        }
        else if (subInvUI.selectedIndex == this.index) // this was selected again
        {
            selectedCanvas.gameObject.SetActive(false);
            subInvUI.selectedIndex = -1;
            backToChestButton.gameObject.SetActive(true);
            addFinalInput.gameObject.SetActive(false);
            addFinalButton.gameObject.SetActive(false);
            addFinalInput.text = "";
        }
        else // something else was selected previously
        {
            subInvUI.DeselectAll();
            selectedCanvas.gameObject.SetActive(true);
            subInvUI.selectedIndex = index;
            backToChestButton.gameObject.SetActive(false);
            addFinalInput.gameObject.SetActive(true);
            addFinalButton.gameObject.SetActive(true);
            addFinalInput.text = "";
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
