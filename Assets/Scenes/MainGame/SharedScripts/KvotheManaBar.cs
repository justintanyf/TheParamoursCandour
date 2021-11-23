using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KvotheManaBar : MonoBehaviour
{
    public Slider manaBar;

    private void Start()
    {
        manaBar = GetComponent<Slider>();
        manaBar.maxValue = CharStats.GetMaxMana();
        manaBar.value = CharStats.GetCurrentMana();
    }

    public void SetMaxMana(int maxMana)
    {
        manaBar.maxValue = maxMana;
    }
    
    public void SetMana(int mana)
    {
        manaBar.value = mana;
    }
}
