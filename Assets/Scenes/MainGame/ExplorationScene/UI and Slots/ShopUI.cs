using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public Transform itemsParent;
    public Item[] shopItems;
    private ShopSlot[] slots;    

    // Start is called before the first frame update
    void Start()
    {
        slots = itemsParent.GetComponentsInChildren<ShopSlot>();
        UpdateUI();
    }

    public void UpdateUI()
    {
        // assuming num shopItems and num slots length is the same
        int currSlot = 0;
        for (int i = 0; i < CharStats.availableShopItems.Length; i++)
        {
            if (CharStats.availableShopItems[i])
            {
                slots[currSlot].AddItem(shopItems[i]);
                currSlot++;
            }
        }
    }
}
