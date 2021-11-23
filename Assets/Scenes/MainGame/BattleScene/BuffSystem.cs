using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffSystem : MonoBehaviour
{
    
    #region Singleton

    public static BuffSystem instance;

    void Awake ()
    {
        instance = this;
    }

    #endregion
    /*
     * A static class that contains all the different buffs and debuffs and what sprite they are tied to
     *
     * When you activate a debuff, it is added to the list, the name, the damage, and the number of ticks
     *
     * When you awaken the Burning Debuff, the sprite is set active, and a counter is activated, this returns an array where
     * 0 is name of effect, 1 is the number of ticks left
     *
     *
     * As we are focusing on fire and ice elemental first, have those set first
     */
    public BurningDebuff kvotheBurningDebuff;
    public HealingBuff kvotheHealingBuff;

    public BurningDebuff enemyBurningDebuff;
}
