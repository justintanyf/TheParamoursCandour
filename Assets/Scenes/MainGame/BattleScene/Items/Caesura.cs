using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item/Caesura")]
public class Caesura : Item
{
    private int SwordDamage = 25;

    public void Awake()
    {
        descriptionOfItem = "Saiceres for " + SwordDamage;
    }

    public override void Use ()
    {
        //This should not work
    }
}