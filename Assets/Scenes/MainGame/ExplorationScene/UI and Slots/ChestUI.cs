using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChestUI : MonoBehaviour
{
    public Transform itemsParent;
    public Item[] items;
    private ChestSlot[] slots;    
    public int selectedIndex = -1;
    public string deleteAmount, addToInventoryAmount;
    public TMP_InputField deleteInput, addToInventoryInput;
    public Button addToChestButton, deleteFromChestButton, addToInventoryButton, addFinalButton, backToChestButton;
    public GameObject chest, inventory;
    public Move moveScript;
    public SubInventoryUI subInvUI;
    public Canvas errorCanvas;

    void Start()
    {
        slots = itemsParent.GetComponentsInChildren<ChestSlot>();
        UpdateUI();
    }

    public void UpdateUI()
    {
        // assuming num chest items <= num slots length
        int currSlot = 0;
        for (int i = 0; i < CharStats.savedChest.Length; i++)
        {
            if (CharStats.savedChest[i] > 0)
            {
                slots[currSlot].AddItem(items[i], CharStats.savedChest[i], i);
                currSlot++;
            }
        }
        for (int i = currSlot; i < slots.Length; i++)
        {
            slots[i].RemoveItem();
        }

        DeselectAll();
        selectedIndex = -1;
        backToChestButton.gameObject.SetActive(false);
        addToChestButton.gameObject.SetActive(true);
        deleteInput.gameObject.SetActive(false);
        deleteFromChestButton.gameObject.SetActive(false);
        addToInventoryInput.gameObject.SetActive(false);
        addToInventoryButton.gameObject.SetActive(false);
        deleteInput.text = "";
        addToInventoryInput.text = "";
    }

    public void OnAddToChest()
    {
        DeselectAll();
        selectedIndex = -1;
        addToChestButton.gameObject.SetActive(false);
        deleteInput.gameObject.SetActive(false);
        deleteFromChestButton.gameObject.SetActive(false);
        addToInventoryInput.gameObject.SetActive(false);
        addToInventoryButton.gameObject.SetActive(false);
        deleteInput.text = "";
        addToInventoryInput.text = "";
        chest.SetActive(false);

        inventory.SetActive(true);
        backToChestButton.gameObject.SetActive(true);
    }

    public void OnDelete()
    {
        int amt;
        if (!int.TryParse(deleteAmount, out amt))
        {
            errorCanvas.gameObject.SetActive(true);
            return;
        }
        if (CharStats.savedChest[selectedIndex] < amt || amt < 1)
        {
            errorCanvas.gameObject.SetActive(true);
            return;
        }
        CharStats.savedChest[selectedIndex] -= amt;
        moveScript.AddToLog("You have deleted " + amt + " " + items[selectedIndex].name + "(s).");
        UpdateUI();
    }

    public void OnAddToInventory()
    {
        int amt;
        if (!int.TryParse(addToInventoryAmount, out amt))
        {
            errorCanvas.gameObject.SetActive(true);
            return;
        }
        if (CharStats.savedChest[selectedIndex] < amt || amt < 1)
        {
            errorCanvas.gameObject.SetActive(true);
            return;
        }
        CharStats.savedChest[selectedIndex] -= amt;
        CharStats.savedInventory[selectedIndex] += amt;
        moveScript.AddToLog("You have moved " + amt + " " + items[selectedIndex].name + "(s) to your inventory.");
        subInvUI.UpdateUI();
        UpdateUI();
    }

    public void GetDeleteAmount(string amt)
    {
        deleteAmount = amt;
    }

    public void GetAddToInventoryAmount(string amt)
    {
        addToInventoryAmount = amt;
    }

    public void DeselectAll()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Deselect();
        }
    }
}
