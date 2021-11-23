using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffTemplate : MonoBehaviour
{
    public string textWhenBuffTakesEffect;
    public int remainingActiveTurnsLeft;
    private bool active = false;

    private void Start()
    {
    }

    public int GetNumberOfTurnsLeft()
    {
        return this.remainingActiveTurnsLeft;
    }

    public void SetNumberOfTurnsRemaining(int numberOfRemainingTurns)
    {
        this.remainingActiveTurnsLeft = numberOfRemainingTurns;
    }

    public void SetActive()
    {
        this.active = true;
    }

    public void SetNotActive()
    {
        this.active = false;
    }

    public bool CheckIsActive()
    {
        return this.active;
    }
}
