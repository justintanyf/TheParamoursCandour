using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubInventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public Item[] items;
    private SubInventorySlot[] slots;    
    public int selectedIndex = -1;
    public string addToChestAmount;
    public TMP_InputField addFinalInput;
    public Button addToChestButton, deleteFromChestButton, addToInventoryButton, addFinalButton, backToChestButton;
    public GameObject chest, inventory;
    public Move moveScript;
    public ChestUI chestUI;
    public Canvas errorCanvas;

    void Start()
    {
        slots = itemsParent.GetComponentsInChildren<SubInventorySlot>();
        UpdateUI();
        inventory.SetActive(false);
        backToChestButton.gameObject.SetActive(false);
    }

    public void UpdateUI()
    {
        // assuming num inv items <= num slots length
        int currSlot = 0;
        for (int i = 0; i < CharStats.savedInventory.Length; i++)
        {
            if (CharStats.savedInventory[i] > 0)
            {
                slots[currSlot].AddItem(items[i], CharStats.savedInventory[i], i);
                currSlot++;
            }
        }
        for (int i = currSlot; i < slots.Length; i++)
        {
            slots[i].RemoveItem();
        }

        DeselectAll();
        selectedIndex = -1;
        addToChestButton.gameObject.SetActive(false);
        backToChestButton.gameObject.SetActive(true);
        addFinalInput.gameObject.SetActive(false);
        addFinalButton.gameObject.SetActive(false);
        addFinalInput.text = "";
    }

    public void OnFinalAdd()
    {
        int amt;
        if (!int.TryParse(addToChestAmount, out amt))
        {
            errorCanvas.gameObject.SetActive(true);
            return;
        }
        if (CharStats.savedInventory[selectedIndex] < amt || amt < 1)
        {
            errorCanvas.gameObject.SetActive(true);
            return;
        }
        CharStats.savedInventory[selectedIndex] -= amt;
        CharStats.savedChest[selectedIndex] += amt;
        moveScript.AddToLog("You have moved " + amt + " " + items[selectedIndex].name + "(s) to your chest.");
        chestUI.UpdateUI();
        UpdateUI();
    }

    public void OnBackToChest()
    {
        DeselectAll();
        selectedIndex = -1;
        backToChestButton.gameObject.SetActive(false);
        addFinalInput.gameObject.SetActive(false);
        addFinalButton.gameObject.SetActive(false);
        addFinalInput.text = "";
        inventory.SetActive(false);

        chest.SetActive(true);
        addToChestButton.gameObject.SetActive(true);
    }

    public void GetAddToChestAmount(string amt)
    {
        addToChestAmount = amt;
    }

    public void DeselectAll()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Deselect();
        }
    }
}
