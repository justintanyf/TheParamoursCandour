using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KvotheHealthBar : MonoBehaviour
{
    public Slider healthBar;

    private void Start()
    {
        healthBar = GetComponent<Slider>();
        this.SetMaxHealth(CharStats.GetMaxHP());
        this.SetHealth(CharStats.GetCurrentHP());
    }
    
    public void SetMaxHealth(int maxHP)
    {
        healthBar.maxValue = maxHP;
    }
    
    public void SetHealth(int currentHp)
    {
        healthBar.value = currentHp;
    }
}