using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public GameObject inventoryUI;

    public Item[] items;
    
    private Inventory inventory;

    private InventorySlot[] slots;    
    public BattleSystem battleSystem;


    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        UpdateUI();// Should not be called but for now
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
                inventory.items[i].AssignBattleSystem(battleSystem);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    // public void UpdateUI()
    // {
    //     // assuming num inv items <= num slots length
    //     int currSlot = 0;
    //     for (int i = 0; i < CharStats.savedInventory.Length; i++)
    //     {
    //         if (CharStats.savedInventory[i] > 0)
    //         {
    //             slots[currSlot].AddItem(items[i], CharStats.savedInventory[i], i);
    //             currSlot++;
    //         }
    //     }
    //     for (int i = currSlot; i < slots.Length; i++)
    //     {
    //         slots[i].RemoveItem();
    //     }
    // }

    public void OnInventoryButton()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
        //print(CharStats);

    }
}
