using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item/Sword")]
public class Sword : Item
{
    public string type;
    public int strength = 0;
    public int durability = 0;
    
    public override void Use()
    {
        
    }
}
