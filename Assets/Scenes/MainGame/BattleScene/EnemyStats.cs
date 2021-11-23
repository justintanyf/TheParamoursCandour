using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public new string name = "Forest Ranger";
    public int curHealth;
    public int maxHealth;
    public int curMana;
    public int maxMana;
    public int enemyLevel = 1;
    public List<Item> enemyInventory;

    /*
     * Must always include this new value in the prefab
     * Current: 
     * 0 = Fire Type
     * 1 = Water Type
     *
     * Future:
     * 2 = Earth Type
     * 3 = Air Type
     */
    public int enemyType;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        /*
         * Ok I want to call from the scene what enemy is being spawned and that will determine
         * what stats the enemy has
         * which sprite the enemy uses
         * What movesets?
         */
        curHealth = maxHealth;
        curMana = maxMana;
    }

    public bool DrainEnemyHp(int damage)
    {
        curHealth -= damage;
        if (curHealth <= 0)
            return true;
        else
            return false;
    }
    
    public void DrainEnemyMp(int manaUsed)
    {
        curMana -= manaUsed;
    }

    // Must override this everytime!
    public virtual KeyValuePair<String, int> GetNextEnemyMove()
    {
        return new KeyValuePair<string, int>("Parent Move", 10 + this.enemyLevel);
    }
}