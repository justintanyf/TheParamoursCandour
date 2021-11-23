using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BindingSystem : MonoBehaviour
{
    public BattleSystem battleSystem;
    
    public GameObject tableContents;
    public GameObject tableDataTemplate;
    public Dictionary<int, GameObject> tableRows = new Dictionary<int, GameObject>();

    private Color32 selectedColour = new Color32(63, 130, 219, 100);
    private Color32 deselectedColour = new Color32(0, 0, 0, 0);
    
    private int selectedCharId = -1;


    public EnemyStats enemyStats;
    public static Dictionary<int, Item[]> listOfBindings = new Dictionary<int, Item[]>();
    private int bindingID;
    public Item caesura;

    // draw on charstats
    //make methods for connections and bind go battlesystem
    public void RegenerateBindings()
    {
        if (this.enemyStats == null)
        {
            this.enemyStats = battleSystem.GetCurrentEnemyStats();
        }
        this.bindingID = 0;
        // Generate a list of potential bindings, then choose how to display them
        
        // generate a list of potential bindings
        foreach (Item enemyItem in enemyStats.enemyInventory)
        {
            listOfBindings.Add(bindingID++, new Item[]{caesura, enemyItem});

        }
        foreach (Item charItem in Inventory.instance.items)
        {
            foreach (Item enemyItem in enemyStats.enemyInventory)
            {
                String newString = "Bind " + charItem + " to " + enemyItem;
                listOfBindings.Add(bindingID++, new Item[]{charItem, enemyItem});
            }
        }
        
        // Choose how to display them
        SetScrollViewText();
    }

    public void SetScrollViewText()
    {
        foreach (KeyValuePair<int, Item[]> potentialBinding in listOfBindings)
        {
            print("this happens");
            int currCharId = potentialBinding.Key;
            GameObject currChar = Instantiate(tableDataTemplate, tableContents.transform);
            Text[] charFields = currChar.GetComponentsInChildren<Text>();
            charFields[0].text = potentialBinding.Value[0].name;
            charFields[1].text = potentialBinding.Value[1].name;
            tableRows.Add(currCharId, currChar);
            Image currCharImage = currChar.GetComponent<Image>();

            currChar.AddComponent(typeof(EventTrigger));
            EventTrigger trigger = currChar.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(delegate { SetSelectedChar(currCharId, currCharImage); });
            trigger.triggers.Add(entry); 
            currChar.SetActive(true);
        }   
    }
    
    public void SetSelectedChar(int charId, Image charImage)
    {
        if (charImage.color == selectedColour)
        {
            charImage.color = deselectedColour;
            selectedCharId = -1;
            // nothing selected
            // addCharButton.gameObject.SetActive(true);
            // backCharButton.gameObject.SetActive(true);
            // deleteCharButton.gameObject.SetActive(false);
            // playButton.gameObject.SetActive(false);
        }
        else 
        {
            foreach (Transform child in tableContents.transform) // can also use tableRows
            {
                GameObject temp = child.gameObject;
                temp.GetComponent<Image>().color = deselectedColour;
            }
            charImage.color = selectedColour;
            selectedCharId = charId;
            // something selected
            // addCharButton.gameObject.SetActive(false);
            // backCharButton.gameObject.SetActive(false);
            // deleteCharButton.gameObject.SetActive(true);
            // playButton.gameObject.SetActive(true);
        }
        Debug.Log(selectedCharId);
    }

    public void OnBindingButton()
    {
        // this.gameObject.SetActive(!this.gameObject.activeSelf);
        // this.RegenerateBindings();
    }
}
