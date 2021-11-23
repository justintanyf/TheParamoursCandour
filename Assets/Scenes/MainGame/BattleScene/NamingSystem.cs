using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NamingSystem : MonoBehaviour
{
    /*
     * Basically use charStats to check if the node is unlocked and then allow the button to be interactable that chooses which one to use
     *
     *
     * ALSO NEED TO CREATE THE SAVE ENSLAVE SCENE TO CHECK WHICH SKILL TO USE
     */
    // Start is called before the first frame update

    public Button fireButton;
    public Button waterButton;
    public Button earthButton;
    public Button airButton;
    public BattleSystem battleSystem;

    void Start()
    {
        // Here we check if the skill is unlocked
        //if (CharStats.GetNode("t4l1s1").IsUnlocked())
        //{
            fireButton.interactable = true;
        //} 
        //if (CharStats.GetNode("t4l1s2").IsUnlocked())
        //{
            waterButton.interactable = true;
        //}
        if (CharStats.GetNode("t4l1s3").IsUnlocked())
        {
            earthButton.interactable = true;
        }
        if (CharStats.GetNode("t4l1s4").IsUnlocked())
        {
            airButton.interactable = true;
        }
    }

    public void OnFireButton()
    {
        battleSystem.OnFireNamingButton();
    }    
    
    public void OnWaterButton()
    {
        battleSystem.OnWaterNamingButton();
    }    
    
    public void OnEarthButton()
    {
        //battleSystem.OnFireNamingButton();
    }    
    
    public void OnAirButton()
    {
        //battleSystem.OnFireNamingButton();
    }

    public void OnNamingButton()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }
}
