using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Item
{
    public bool isGreaterHealthPotion;
    private int healAmount = 30;

    public void Awake()
    {
        descriptionOfItem = "Heals for " + healAmount;
    }

    public override void Use ()
    {
        battleSystem.OnHealthPotion(healAmount);
    }
}
