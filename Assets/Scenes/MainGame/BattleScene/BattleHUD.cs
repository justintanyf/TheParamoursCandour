using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Text levelText;
    public Slider hpSlider;
    public Slider manaSlider;
    public GameObject bindingCanvas;

    public void SetHUDForKvothe()
    {
        hpSlider.maxValue = CharStats.GetMaxHP();
        hpSlider.value = CharStats.GetCurrentHP();
        manaSlider.maxValue = CharStats.GetMaxMana();
        manaSlider.value = CharStats.GetCurrentMana();
    }

    public void SetHUDForEnemy(EnemyStats enemyStats)
    {
        hpSlider.maxValue = enemyStats.maxHealth;
        hpSlider.value = enemyStats.curHealth;
        manaSlider.maxValue = enemyStats.maxMana;
        manaSlider.value = enemyStats.curMana;
    }

    public void SetHP(int hp)
    {
        hpSlider.value = hp;
    }

    public void SetMP(int mana)
    {
        manaSlider.value = mana;
    }
    
    public void OnBindButton()
    {
        bindingCanvas.SetActive(!bindingCanvas.activeSelf);
    }
}
