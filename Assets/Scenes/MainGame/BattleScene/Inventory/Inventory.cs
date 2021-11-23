using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    #region Singleton

    public static Inventory instance;

    void Awake ()
    {
        instance = this;
    }

    #endregion


    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public Item currentlySelectedItem;

    public int space = 9;	// Amount of item spaces, which is currently a 3x3 matrix

    // Our current list of items in the inventory
    public List<Item> items = new List<Item>();
    
    // Change to array of items
    // Have another array 

    // Add a new item if enough room
    public void Add (Item item)
    {
        if (item.showInInventory) {
            if (items.Count >= space) {
                Debug.Log ("Not enough room.");
                return;
            }

            items.Add (item);

            if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke ();
        }
    }

    // Remove an item
    public void Remove (Item item)
    {
        items.Remove(item);

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

    public List<Item> GetInventory()
    {
        return this.items;
    }

    public void SetInventory(List<Item> savedItems)
    {
        this.items = savedItems;
    }

    public void SetCurrentlyUsedItem(Item currentItem)
    {
        this.currentlySelectedItem = currentItem;
    }
}