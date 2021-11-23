using UnityEngine;

public class CraftingUI : MonoBehaviour
{
    public Transform itemsParent;
    public Item[] recipes;
    private CraftingSlot[] slots;    

    // Start is called before the first frame update
    void Start()
    {
        slots = itemsParent.GetComponentsInChildren<CraftingSlot>();
        UpdateUI();
    }

    public void UpdateUI()
    {
        // assuming num recipes and num slots length is the same
        int currSlot = 0;
        for (int i = 0; i < CharStats.acquiredRecipes.Length; i++)
        {
            if (CharStats.acquiredRecipes[i])
            {
                slots[currSlot].AddItem(recipes[i]);
                currSlot++;
            }
        }
    }
}
