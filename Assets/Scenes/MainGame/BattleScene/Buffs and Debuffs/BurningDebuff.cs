using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BurningDebuff: BuffTemplate
{
    public int damagePerTurn;

    public void StartBurning(int damagePerTurn, int numberOfTurns, string textWhenBuffIsUsed)
    {
        this.gameObject.SetActive(true);
        this.remainingActiveTurnsLeft = numberOfTurns;
        this.damagePerTurn = damagePerTurn;
        this.SetActive();
        this.textWhenBuffTakesEffect = textWhenBuffIsUsed;
    }

    public int GetDamagePerTurn()
    {
        return this.damagePerTurn;
    }
}
