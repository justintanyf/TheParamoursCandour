using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEnemy : EnemyStats
{
    public override KeyValuePair<String, int> GetNextEnemyMove()
    {
        // Have a part that checks how many bindings the char has, and then react
        var random = new System.Random();
        double rand = random.NextDouble();
        if (rand < 0.3) // Tackle
        {
            return new KeyValuePair<string, int>("Tackle", 20 + this.enemyLevel * 2);
        }
        else //Ember
        {
            // Gotta check whether to apply burning buff
            rand = random.NextDouble();
            //if (CharStats.HasBurningDebuff() && rand < 0.7) {}
            return new KeyValuePair<String, int>("Cinder", 20 + this.enemyLevel * 1);
        }
    }
}
