using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item/Mana Potion")]

public class ManaPotion : Item
{
    public bool isGreaterManahPotion;
    private int ManaAmount = 50;

    public void Awake()
    {
        descriptionOfItem = "Restores " + ManaAmount + " mana.";
    }

    public override void Use ()
    {
        battleSystem.OnManaPotion(ManaAmount);
    }
}