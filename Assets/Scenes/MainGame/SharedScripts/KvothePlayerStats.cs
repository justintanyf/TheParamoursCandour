using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KvothePlayerStats : MonoBehaviour
{
    public new string name = "Kvothe";
    public int curHealth = 0;
    public int maxHealth = 100;
    public int curMana = 0;
    public int maxMana = 100;
    public int swordBaseDamage = 25;

    public KvotheHealthBar healthBar;
    public KvotheManaBar manaBar;

    // Start is called before the first frame update
    private void Start()
    {
        curHealth = maxHealth;
        curMana = maxMana;
    }
    
    // Update is called once per frame
    private void Update()
    {
        // this exists to test in the future
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     TakeDamage(10);
        // }
        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     DrainMana(10);
        // }
    }

    public bool TakeDamage(int damage)
    {
        curHealth -= damage;
        if (!System.Object.ReferenceEquals(healthBar, null))
        {
            healthBar.SetHealth(curHealth);
        }
        if (curHealth <= 0)
            return true;
        else
            return false;
    }
    
    public void Heal(int healAmount)
    {
        curHealth += healAmount;
        if (!System.Object.ReferenceEquals(healthBar, null))
        {
            healthBar.SetHealth(curHealth);
        }
        if (curHealth > maxHealth)
            curHealth = maxHealth;
    }

    public void DrainMana(int manaUsed)
    {
        curMana -= manaUsed;

        manaBar.SetMana(curMana);
    }

    public int GetDamageValue()
    {
        // Ideally have all the math calculated here based on the buffs
        return swordBaseDamage;
    }
}