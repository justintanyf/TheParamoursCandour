using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST, WAIT }
public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject enemyGameObject;
    public GameObject fragmentCanvas;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    private EnemyStats enemyStats;

    public TMP_Text dialogueText;
    public TMP_Text battleLog;
    public ScrollRect scrollRect;
    private int battleLogNumber;

    public BattleHUD KvotheHUD;
    public BattleHUD EnemyHUD;
    
    public BattleState state;
    // Start is called before the first frame update

    private int numberOfEscapeAttempts;

    public bool usedBefore = false; // TO DELETE

    void Start()
    {
        SetEnemyPrefab();
        state = BattleState.START;
        battleLogNumber = 1;
        StartCoroutine(SetupBattle());
    }

    private void SetEnemyPrefab()
    {
        // if (Move.instance == null)
        // {
        //  this.enemyPrefab = enemyPrefab;
        // }
        // else
        if (Move.instance.DoesEncounteredEnemyToPassExist())
        {
            this.enemyPrefab = Move.instance.GetEncounteredEnemyToPass();
        }
        else
        {
            this.enemyPrefab = enemyPrefab;
        }
    }
    
    public IEnumerator SetupBattle()
    {
        GameObject playerGameObject = Instantiate(playerPrefab, playerBattleStation, false);
        
        enemyGameObject = Instantiate(enemyPrefab, enemyBattleStation, false);
        enemyGameObject.transform.position = enemyBattleStation.transform.position;
        while (enemyGameObject.GetComponent<SpriteRenderer>().bounds.size.x >
               enemyBattleStation.GetComponent<SpriteRenderer>().bounds.size.x * 1.3)
        {
            print("IT WORKS");
            Vector3 currentScale = enemyGameObject.transform.localScale;
            enemyGameObject.transform.localScale = new Vector3(currentScale.x * 0.9f, currentScale.y * 0.9f, 1f);
        }
        enemyStats = enemyGameObject.GetComponent<EnemyStats>();

        NewDialogueText("A wild " + enemyStats.name + " approaches...");
        
        KvotheHUD.SetHUDForKvothe();
        EnemyHUD.SetHUDForEnemy(enemyStats);

        yield return new WaitForSeconds(1f);
        
        state = BattleState.PLAYERTURN;
        numberOfEscapeAttempts = 0;
        PlayerTurn();
    }

    public EnemyStats GetCurrentEnemyStats()
    {
        return this.enemyStats;
    }

    IEnumerator PlayerAttack(int damageOnEnemy, string newText)
    {
        state = BattleState.WAIT;// This is necessary, otherwise attacks can be spammed
        bool isDead = enemyStats.DrainEnemyHp(damageOnEnemy);
        EnemyHUD.SetHP(enemyStats.curHealth);
        NewDialogueText(newText);
        // Damage the enemy
        yield return new WaitForSeconds(1f);
        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator UseManaAndPlayerAttack(int manaDrainAmount, int damageOnEnemy, string newText)
    {
        CharStats.DrainMana(manaDrainAmount);
        KvotheHUD.SetMP(CharStats.GetCurrentMana());

        yield return new WaitForSeconds(1f);

        StartCoroutine(PlayerAttack(damageOnEnemy, newText));
    }

    IEnumerator PlayerHeal(int healAmount)
    {
        state = BattleState.WAIT;// This is necessary, otherwise heals can be spammed
        CharStats.Heal(healAmount);
        KvotheHUD.SetHP(CharStats.GetCurrentHP());
        NewDialogueText("You feel renewed Strength and heal for " + healAmount + ".");

        yield return new WaitForSeconds(1f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    
    IEnumerator PlayerIncreaseMana(int increaseManaAmount)
    {
        state = BattleState.WAIT;// This is necessary, otherwise heals can be spammed
        CharStats.IncreaseMana(increaseManaAmount);
        KvotheHUD.SetMP(CharStats.GetCurrentMana());
        NewDialogueText("Kvothe's mind settles and clear, gaining " + increaseManaAmount + ".");

        yield return new WaitForSeconds(1f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        // First check if there are debuffs
        if (BuffSystem.instance.enemyBurningDebuff.CheckIsActive())
        {
            state = BattleState.WAIT;// This is necessary, otherwise attacks can be spammed
            bool isDeadFromStatus = enemyStats.DrainEnemyHp(BuffSystem.instance.enemyBurningDebuff.damagePerTurn);
            EnemyHUD.SetHP(enemyStats.curHealth);
            NewDialogueText(BuffSystem.instance.enemyBurningDebuff.textWhenBuffTakesEffect);
            // Damage the enemy
            yield return new WaitForSeconds(1f);
            if (isDeadFromStatus)
            {
                state = BattleState.WON;
                EndBattle();
            }
            yield return new WaitForSeconds(1f);
            if (BuffSystem.instance.enemyBurningDebuff.GetNumberOfTurnsLeft() - 1 == 1)
            {
                BuffSystem.instance.enemyBurningDebuff.SetNotActive();
            }
            else
            {
                BuffSystem.instance.enemyBurningDebuff.SetNumberOfTurnsRemaining(
                    BuffSystem.instance.enemyBurningDebuff.GetNumberOfTurnsLeft() - 1);
            }
        }

        KeyValuePair<String, int> enemyMovePair = this.enemyStats.GetNextEnemyMove();
        NewDialogueText(this.enemyStats.name + " uses " + enemyMovePair.Key + " and does " + enemyMovePair.Value + "!");
        // Here we have the logic of how the enemy attacks

        yield return new WaitForSeconds(1f);
        
        bool isDead = CharStats.TakeDamage(enemyMovePair.Value); // Should be checking for a bool if Kvothe dies
        KvotheHUD.SetHP(CharStats.GetCurrentHP());

        yield return new WaitForSeconds(1f);
        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON && enemyStats.name != "Boss")
        {
            dialogueText.text = "You won the battle!";
            CharStats.numFaeDefeated++;
            if (CharStats.numFaeDefeated == 1)
            {
                this.enemyGameObject.SetActive(false);
                fragmentCanvas.SetActive(true);
                CharStats.fragmentsFound[6] = true;
                // call verification to store in db
                return;
            }
            // Load out of this screen to the appropriate screen
            SceneManager.LoadScene("ExplorationScene");
        } 
        else if (state == BattleState.WON && enemyStats.name == "Boss")
        {
            // handle boss win - win game screen
            SceneManager.LoadScene("Win");
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You lost the battle!";
            // Load out of this screen to the appropriate screen - lose game screen
            SceneManager.LoadScene("Lose");
        }
    }

    public void OnFragmentAcknowledgement()
    {
        Debug.Log("wtf");
        fragmentCanvas.SetActive(false);
        SceneManager.LoadScene("ExplorationScene");
    }

    public void OnEscapeButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        StartCoroutine(AttemptEscape());
    }
    
    IEnumerator AttemptEscape()
    {
        state = BattleState.WAIT;
        
        float probabilityOfEscape = ( (CharStats.charLevel / enemyStats.enemyLevel) + numberOfEscapeAttempts / 10 ) / 2;
        var random = new System.Random();
        double rand = random.NextDouble();
        if (rand <= probabilityOfEscape)
        {
            //failed to escape
            NewDialogueText("You failed to escape!");
            yield return new WaitForSeconds(1f);
            numberOfEscapeAttempts++;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            //successfully escaped
            NewDialogueText("You successfully escaped!");
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene("ExplorationScene");
        }
    }

    public void PlayerTurn()
    {
        // Here we check for any healing buffs
        if (BuffSystem.instance.kvotheHealingBuff.CheckIsActive())
        {
            CharStats.curHealth += BuffSystem.instance.kvotheHealingBuff.healPerTurn;
            KvotheHUD.SetHP(CharStats.GetCurrentHP());
            NewDialogueText(BuffSystem.instance.kvotheHealingBuff.textWhenBuffTakesEffect);
            if (BuffSystem.instance.kvotheHealingBuff.GetNumberOfTurnsLeft() - 1 == 1)
            {
                BuffSystem.instance.kvotheHealingBuff.SetNotActive();
            }
            else
            {
                BuffSystem.instance.kvotheHealingBuff.SetNumberOfTurnsRemaining(
                    BuffSystem.instance.kvotheHealingBuff.GetNumberOfTurnsLeft() - 1);
            }
        }
        NewDialogueText("Choose an action...");
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        int totalDamage = CharStats.GetDamageValue();
        string newText = "Caesura glides through the air as it had a million times before, doing " + totalDamage + "!";
        StartCoroutine(PlayerAttack(totalDamage, newText));
    }
    
    public void OnHealthPotion(int healAmount)
    {
        if (state != BattleState.PLAYERTURN)
            return;
        StartCoroutine(PlayerHeal(healAmount));
    }
    
    public void OnManaPotion(int increaseManaAmount)
    {
        if (state != BattleState.PLAYERTURN)
            return;
        StartCoroutine(PlayerIncreaseMana(increaseManaAmount));
    }

    public void NewDialogueText(string newText)
    {
        battleLog.text = battleLog.text + "\n" + battleLogNumber + ") "+ newText;
        battleLogNumber++;
        dialogueText.text = newText;
        scrollRect.verticalNormalizedPosition = 0f;
    }
    
    public void ToSkillTree()
    {
        SceneManager.LoadScene("SkillTree");
    }

    public void OnQuitToHomeScreen()
    {
        SceneManager.LoadScene("Starting Screen");
    }

    public void OnQuitToDesktopButton()
    {
        Application.Quit();
    }

    public int CalculateNamingManaDrain()
    {
        int currentCharLevel = CharStats.GetCharLevel();
        int manaDrain = - (currentCharLevel * currentCharLevel) + 100; // -x^2 + 100?
        return manaDrain;
    }
    
    public void OnFireNamingButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        int manaDrain = this.CalculateNamingManaDrain();
        if (manaDrain > CharStats.GetCurrentMana())
        {
            NewDialogueText("You do not have enough mana for that!");
            return;
        }
        StartCoroutine(IgnisName());
    }
    
    IEnumerator IgnisName()
    {
        /*
         * Fire Elemental
                Saving: Fire’s real name, damage over time for 3 enemy turns
                Enslaving: Fire Elemental inflicts 50% damage that ignores armour

         */
        state = BattleState.WAIT;// This is necessary, otherwise it can be spammed
        int manaDrain = this.CalculateNamingManaDrain();
        int currentCharLevel = CharStats.GetCharLevel();
        // if (CharStats.GetDefeatedFireMiniBoss())  // Here we check whether or not the mini-boss was slaved or enslaved, for now let's go with saved
        if (this.usedBefore) // TO DELETE
        { // Saved
            int damagePerTurn = currentCharLevel < 10
                ? currentCharLevel * 2 + 25
                : Convert.ToInt32(Math.Floor(currentCharLevel * 1.5));
            NewDialogueText("\"Ignis!\"" + ", Kvothe roars, causing the temperature to rise rapidly. " +
                            enemyStats.name + " erupts into flames!" );
            yield return new WaitForSeconds(0.5f);
            string newText = enemyStats.name + " burned for " + damagePerTurn + ".";
            BuffSystem.instance.enemyBurningDebuff.StartBurning(damagePerTurn, 2, newText); // 2 as we are counting until 0
            StartCoroutine(UseManaAndPlayerAttack(manaDrain, damagePerTurn, newText));
            this.usedBefore = !this.usedBefore; // TO DELETE
        }
        else
        { // Enslaved
            string newText = "\"Apollo!\"" + ", Kvothe beckons, calling forth the enslaved fire elemental. " +
                            "It stares at Kvothe with empty eyes, before suddenly charging at the " +
                            enemyStats.name + " and erupting into a fiery ball!";
            int damage = -(currentCharLevel * currentCharLevel) + 100; 
            StartCoroutine(UseManaAndPlayerAttack(manaDrain, damage, newText));
            yield return new WaitForSeconds(1f);
            this.usedBefore = !this.usedBefore; // TO DELETE
        }
    }
    
    public void OnWaterNamingButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        int manaDrain = this.CalculateNamingManaDrain();
        if (manaDrain > CharStats.GetCurrentMana())
        {
            NewDialogueText("You do not have enough mana for that!");
            return;
        }
        StartCoroutine(AquaButton());
    }
    
    IEnumerator AquaButton()
    {
        /*
         * Water Elemental
            Saving: Water’s real name, heal over time for 3 player turns
            Enslaving: Water Elemental regains all health and overheals by 30%
         */
        state = BattleState.WAIT;// This is necessary, otherwise it can be spammed
        int manaDrain = this.CalculateNamingManaDrain();
        CharStats.DrainMana(manaDrain);
        KvotheHUD.SetMP(CharStats.GetCurrentMana());
        int currentCharLevel = CharStats.GetCharLevel();
        //if (CharStats.GetDefeatedWaterMiniBoss())  // Here we check whether or not the mini-boss was slaved or enslaved, for now let's go with saved
        if (usedBefore)
        { // Saved
            int healPerTurn = Convert.ToInt32(CharStats.GetMaxHP() * 0.25); //Heal for 25% every turn?
            NewDialogueText("\"Aqua\"" +
                            ", Kvothe uttered, like the calm before the storm. It felt too humid to breathe, and his sweat glistened, before glowing an eerie blue.");
            string newText = "Kvothe healed for " + healPerTurn + ".";
            BuffSystem.instance.kvotheHealingBuff.StartHealing(healPerTurn, 3, newText); // 2 as we are counting until 0
            yield return new WaitForSeconds(1f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
            this.usedBefore = !this.usedBefore; // TO DELETE
        }
        else
        { // Enslaved
            NewDialogueText( "\"Amphitrite!\"" + ", Kvothe beckons, calling forth the enslaved water elemental. " +
                             "It shot a menacing glance at Kvothe, before suddenly glowing blindingly blue. " +
                             "Kvothe feel pins and needles wash over him.");
            yield return new WaitForSeconds(0.5f);
            CharStats.curHealth = CharStats.GetMaxHP();
            KvotheHUD.SetHP(CharStats.GetCurrentHP());
            NewDialogueText("Kvothe heals back to full health");
            yield return new WaitForSeconds(0.5f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
            this.usedBefore = !this.usedBefore; // TO DELETE
        }
    }
}
