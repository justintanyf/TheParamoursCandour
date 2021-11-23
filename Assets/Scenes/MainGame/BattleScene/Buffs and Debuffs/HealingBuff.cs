using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealingBuff: BuffTemplate
{
    public int healPerTurn;

    public void StartHealing(int healthPerTurn, int numberOfTurns, string textWhenBuffIsUsed)
    {
        this.gameObject.SetActive(true);
        this.remainingActiveTurnsLeft = numberOfTurns;
        this.healPerTurn = healthPerTurn;
        this.SetActive();
        this.textWhenBuffTakesEffect = textWhenBuffIsUsed;
    }

    public int GetHealPerTurn()
    {
        return this.healPerTurn;
    }
}