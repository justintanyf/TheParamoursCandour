using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Npgsql;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SkillTree : MonoBehaviour
{
    public Button t1l1s1, t1l1s2, t1l1s3, t1l2s1;
    public Button t2l1s1, t2l1s2;
    public Button t3l1s1, t3l1s2, t3l1s3, t3l2s1;
    public Button t4l1s1, t4l1s2, t4l1s3, t4l1s4;
    public Button t5l1s1, t5l1s2, t5l2s1, t5l3s1;
    public Button sympathyButton, sygaldryButton, alchemyButton, namingButton, swordplayButton;
    public Button upgradeButton;
    public GameObject treeSelection;
    public GameObject sympathyTree;
    public GameObject sygaldryTree;
    public GameObject alchemyTree;
    public GameObject namingTree;
    public GameObject swordplayTree;
    public GameObject skillInfo;
    public GameObject fragmentCanvas;
    public TMP_Text infoField;
    public TMP_Text msgField;
    public Canvas sympathyLocked, sygaldryLocked, alchemyLocked, namingLocked, successCanvas;
    public TMP_Text successField;
    private NpgsqlConnection conn = LoginPageScript.GetConnection();

    // Start is called before the first frame update
    void Start()
    {    
        t1l1s1.onClick.AddListener(delegate { CheckUpgrade("t1l1s1"); });
        t1l1s2.onClick.AddListener(delegate { CheckUpgrade("t1l1s2"); });
        t1l1s3.onClick.AddListener(delegate { CheckUpgrade("t1l1s3"); });
        t1l2s1.onClick.AddListener(delegate { CheckUpgrade("t1l2s1"); });

        t2l1s1.onClick.AddListener(delegate { CheckUpgrade("t2l1s1"); });
        t2l1s2.onClick.AddListener(delegate { CheckUpgrade("t2l1s2"); });

        t3l1s1.onClick.AddListener(delegate { CheckUpgrade("t3l1s1"); });
        t3l1s2.onClick.AddListener(delegate { CheckUpgrade("t3l1s2"); });
        t3l1s3.onClick.AddListener(delegate { CheckUpgrade("t3l1s3"); });
        t3l2s1.onClick.AddListener(delegate { CheckUpgrade("t3l2s1"); });

        t4l1s1.onClick.AddListener(delegate { CheckUpgrade("t4l1s1"); });
        t4l1s2.onClick.AddListener(delegate { CheckUpgrade("t4l1s2"); });
        t4l1s3.onClick.AddListener(delegate { CheckUpgrade("t4l1s3"); });
        t4l1s4.onClick.AddListener(delegate { CheckUpgrade("t4l1s4"); });

        t5l1s1.onClick.AddListener(delegate { CheckUpgrade("t5l1s1"); });
        t5l1s2.onClick.AddListener(delegate { CheckUpgrade("t5l1s2"); });
        t5l2s1.onClick.AddListener(delegate { CheckUpgrade("t5l2s1"); });
        t5l3s1.onClick.AddListener(delegate { CheckUpgrade("t5l3s1"); });

        RefreshUnlocked();

        if (!CharStats.fragmentsFound[7])
        {
            fragmentCanvas.SetActive(true);
            sympathyButton.enabled = false;
            sygaldryButton.enabled = false;
            alchemyButton.enabled = false;
            namingButton.enabled = false;
            swordplayButton.enabled = false;
            CharStats.fragmentsFound[7] = true;
            // call verification to store in db
        }

        sympathyButton.interactable = true;
        sympathyLocked.gameObject.SetActive(false);
        sygaldryButton.interactable = true;
        sygaldryLocked.gameObject.SetActive(false);
        alchemyButton.interactable = true;
        alchemyLocked.gameObject.SetActive(false);
        namingButton.interactable = true;
        namingLocked.gameObject.SetActive(false);
        if (!CharStats.fragmentsFound[6]) // sympathy
        {
            sympathyButton.interactable = false;
            sympathyLocked.gameObject.SetActive(true);
        }
        if (!CharStats.fragmentsFound[5]) // sygaldry
        {
            sygaldryButton.interactable = false;
            sygaldryLocked.gameObject.SetActive(true);
        }
        if (!CharStats.fragmentsFound[1]) // alchemy
        {
            alchemyButton.interactable = false;
            alchemyLocked.gameObject.SetActive(true);
        }
        if (!CharStats.fragmentsFound[4]) // naming
        {
            namingButton.interactable = false;
            namingLocked.gameObject.SetActive(true);
        }
    }

    public void RefreshUnlocked()
    {
        Color32 unlocked = new Color32(255, 255, 255, 255);
        Color32 locked = new Color32(150, 150, 150, 255);

        if (!CharStats.GetNode("t1l1s1").IsUnlocked())
        {
            t1l1s1.GetComponent<Image>().color = locked;
        }
        else
        {
            t1l1s1.GetComponent<Image>().color = unlocked;
        }
        if (!CharStats.GetNode("t1l1s2").IsUnlocked())
        {
            t1l1s2.GetComponent<Image>().color = locked;
        }
        else
        {
            t1l1s2.GetComponent<Image>().color = unlocked;
        }
        if (!CharStats.GetNode("t1l1s3").IsUnlocked())
        {
            t1l1s3.GetComponent<Image>().color = locked;
        }
        else
        {
            t1l1s3.GetComponent<Image>().color = unlocked;
        }
        if (!CharStats.GetNode("t1l2s1").IsUnlocked())
        {
            t1l2s1.GetComponent<Image>().color = locked;
        }
        else
        {
            t1l2s1.GetComponent<Image>().color = unlocked;
        }

        if (!CharStats.GetNode("t2l1s1").IsUnlocked())
        {
            t2l1s1.GetComponent<Image>().color = locked;
        }
        else
        {
            t2l1s1.GetComponent<Image>().color = unlocked;
        }
        if (!CharStats.GetNode("t2l1s2").IsUnlocked())
        {
            t2l1s2.GetComponent<Image>().color = locked;
        }
        else
        {
            t2l1s2.GetComponent<Image>().color = unlocked;
        }
        
        if (!CharStats.GetNode("t3l1s1").IsUnlocked())
        {
            t3l1s1.GetComponent<Image>().color = locked;
        }
        else
        {
            t3l1s1.GetComponent<Image>().color = unlocked;
        }
        if (!CharStats.GetNode("t3l1s2").IsUnlocked())
        {
            t3l1s2.GetComponent<Image>().color = locked;
        }
        else
        {
            t3l1s2.GetComponent<Image>().color = unlocked;
        }
        if (!CharStats.GetNode("t3l1s3").IsUnlocked())
        {
            t3l1s3.GetComponent<Image>().color = locked;
        }
        else
        {
            t3l1s3.GetComponent<Image>().color = unlocked;
        }
        if (!CharStats.GetNode("t3l2s1").IsUnlocked())
        {
            t3l2s1.GetComponent<Image>().color = locked;
        }
        else
        {
            t3l2s1.GetComponent<Image>().color = unlocked;
        }
        
        if (!CharStats.GetNode("t4l1s1").IsUnlocked())
        {
            t4l1s1.GetComponent<Image>().color = locked;
        }
        else
        {
            t4l1s1.GetComponent<Image>().color = unlocked;
        }
        if (!CharStats.GetNode("t4l1s2").IsUnlocked())
        {
            t4l1s2.GetComponent<Image>().color = locked;
        }
        else
        {
            t4l1s2.GetComponent<Image>().color = unlocked;
        }
        if (!CharStats.GetNode("t4l1s3").IsUnlocked())
        {
            t4l1s3.GetComponent<Image>().color = locked;
        }
        else
        {
            t4l1s3.GetComponent<Image>().color = unlocked;
        }
        if (!CharStats.GetNode("t4l1s4").IsUnlocked())
        {
            t4l1s4.GetComponent<Image>().color = locked;
        }
        else
        {
            t4l1s4.GetComponent<Image>().color = unlocked;
        }

        if (!CharStats.GetNode("t5l1s1").IsUnlocked())
        {
            t5l1s1.GetComponent<Image>().color = locked;
        }
        else
        {
            t5l1s1.GetComponent<Image>().color = unlocked;
        }
        if (!CharStats.GetNode("t5l1s2").IsUnlocked())
        {
            t5l1s2.GetComponent<Image>().color = locked;
        }
        else
        {
            t5l1s2.GetComponent<Image>().color = unlocked;
        }
        if (!CharStats.GetNode("t5l2s1").IsUnlocked())
        {
            t5l2s1.GetComponent<Image>().color = locked;
        }
        else
        {
            t5l2s1.GetComponent<Image>().color = unlocked;
        }
        if (!CharStats.GetNode("t5l3s1").IsUnlocked())
        {
            t5l3s1.GetComponent<Image>().color = locked;
        }
        else
        {
            t5l3s1.GetComponent<Image>().color = unlocked;
        }
    }

    public void OnFragmentAcknowledgement()
    {
        fragmentCanvas.SetActive(false);
        sympathyButton.enabled = true;
        sygaldryButton.enabled = true;
        alchemyButton.enabled = true;
        namingButton.enabled = true;
        swordplayButton.enabled = true;
    }

    public void CheckUpgrade(string nodeName)
    {
        Debug.Log(nodeName);
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.interactable = true;
        infoField.SetText("");
        msgField.SetText("");

        Node node = CharStats.GetNode(nodeName);
        infoField.SetText("Your skill " + node.GetDesc() + " is at level " + node.GetLevel() + " of " + node.GetMaxLevel() + ".");

        if (!node.PrereqsCleared())
        {
            // TODO: show what prereqs
            msgField.SetText("Prereqs not cleared! Cannot upgrade. See the skills laid out on the left for more information.");
            upgradeButton.interactable = false;
            return;
        }
        if (node.IsMaxLevel())
        {
            msgField.SetText("Max level reached! Cannot upgrade.");
            upgradeButton.interactable = false;
            return;
        }

        // TODO: add cost as a check

        // TODO: cost???
        msgField.SetText("Click to upgrade!");
        upgradeButton.onClick.AddListener(delegate { Upgrade(nodeName); });
    }

    public void Upgrade(string nodeName)
    {
        Node node = CharStats.GetNode(nodeName);
        
        int output;
        conn.Open();
        using (var cmd = new NpgsqlCommand("UPDATE skills SET " + nodeName + " = @l WHERE character_id = @c", conn))
        {
            cmd.Parameters.AddWithValue("l", node.GetLevel() + 1);
            cmd.Parameters.AddWithValue("c", CharStats.charId);
            output = cmd.ExecuteNonQuery();
        }
        conn.Close();

        if (output != 1)
        {
            msgField.SetText("uhhhhhh im not sure what happened");
            return;
        }

        node.Unlock();
        successField.text = "Your skill " + node.GetDesc() + " has been successfully upgraded! It is now at level " + node.GetLevel() + " Congrats (:";
        successCanvas.gameObject.SetActive(true);
        Debug.Log("unlocked, new level of " + nodeName + " is " + node.GetLevel());
        CheckUpgrade(nodeName);
        RefreshUnlocked();
        return;
    }

    public void ToSympathy()
    {
        treeSelection.SetActive(false);
        sympathyTree.SetActive(true);
        skillInfo.SetActive(true);
    }

    public void ToSygaldry()
    {
        treeSelection.SetActive(false);
        sygaldryTree.SetActive(true);
        skillInfo.SetActive(true);
    }

    public void ToAlchemy()
    {
        treeSelection.SetActive(false);
        alchemyTree.SetActive(true);
        skillInfo.SetActive(true);
    }

    public void ToNaming()
    {
        treeSelection.SetActive(false);
        namingTree.SetActive(true);
        skillInfo.SetActive(true);
    }

    public void ToSwordplay()
    {
        treeSelection.SetActive(false);
        swordplayTree.SetActive(true);
        skillInfo.SetActive(true);
    }

    public void ToSelection()
    {
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.interactable = true;
        infoField.SetText("");
        msgField.SetText("");

        treeSelection.SetActive(true);
        sympathyTree.SetActive(false);
        sygaldryTree.SetActive(false);
        alchemyTree.SetActive(false);
        namingTree.SetActive(false);
        swordplayTree.SetActive(false);
        skillInfo.SetActive(false);
    }

    private Save createSaveGameObject()
    {
        Save save = new Save();
        // need to set to whatever the current coords are
        save.xCoord = CharStats.xCoord;
        save.yCoord = CharStats.yCoord;
        save.haveLastCoords = true;
        save.rooms = Underthing.rooms;
        save.currRoom = CharStats.currRoom;
        return save;
    }

    public void SaveBySerialization()
    {
        // save local files
        Save save = createSaveGameObject();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(Application.persistentDataPath + "/" + CharStats.charId + " Data.txt");
        bf.Serialize(fileStream, save);
        fileStream.Close();

        // TODO: save to db
    }

    public void ToExploration()
    {
        SceneManager.LoadScene("ExplorationScene");
    }

    public void ToLore()
    {
        SceneManager.LoadScene("Lore");
    }

    public void SaveAndQuitToMain()
    {
        SaveBySerialization();
        SceneManager.LoadScene("MainMenu");
    }

    public void SaveAndQuitToDesktop()
    {
        SaveBySerialization();
        Application.Quit();
    }
}
