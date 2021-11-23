using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Lore : MonoBehaviour
{
    public Sprite[] loreImages; // to change in the inspector
    public GameObject[] rows = new GameObject[13]; // to change to actual num (in inspector???)
    public int[,] startEndIndex = new int[,] {
        {0, 2},
        {3, 5},
        {6, 9},
        {10, 11},
        {12, 12}
    };
    public GameObject tableContents;
    public GameObject tableDataTemplate;
    public GameObject catSelection;
    public GameObject charSelection;
    public GameObject charImage;
    public GameObject charInfo;
    public TMP_Text charDesc;
    public string selected = "";
    private Color32 selectedColour = new Color32(63, 130, 219, 100);
    private Color32 deselectedColour = new Color32(0, 0, 0, 0);
    
    void Start()
    {
        Debug.Log(UserStats.loreDetails.GetLength(0));
        for (int i = 0; i < UserStats.loreDetails.GetLength(0); i++)
        {
            GameObject currChar = Instantiate(tableDataTemplate, tableContents.transform);
            Text[] charFields = currChar.GetComponentsInChildren<Text>();
            charFields[0].text = UserStats.loreDetails[i,0];
            Sprite currCharImage = loreImages[i];
            string currCharDesc = UserStats.loreDetails[i,1];

            currChar.AddComponent(typeof(EventTrigger));
            EventTrigger trigger = currChar.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(delegate { SetSelectedChar(currCharImage, currCharDesc, currChar); });
            trigger.triggers.Add(entry);

            rows[i] = currChar;
            currChar.SetActive(false);
        }
    }

    public void SetSelectedChar(Sprite img, string desc, GameObject charRow)
    {
        if (selected == desc)
        {
            charRow.GetComponent<Image>().color = deselectedColour;
            charImage.GetComponent<Image>().sprite = null;
            charDesc.text = "";
            charImage.SetActive(false);
            charInfo.SetActive(false);
            selected = "";
        }
        else
        {
            foreach (Transform child in tableContents.transform)
            {
                GameObject temp = child.gameObject;
                temp.GetComponent<Image>().color = deselectedColour;
            }

            charRow.GetComponent<Image>().color = selectedColour;
            charImage.GetComponent<Image>().sprite = img;
            charDesc.text = desc;
            charImage.SetActive(true);
            charInfo.SetActive(true);
            selected = desc;
        }
    }

    public void ToFire()
    {
        CategoryHandler(0);
    }

    public void ToWater()
    {
        CategoryHandler(1);
    }

    public void ToCharacter()
    {
        CategoryHandler(2);
    }

    public void ToEarth()
    {
        CategoryHandler(3);
    }

    public void ToAir()
    {
        CategoryHandler(4);
    }

    public void CategoryHandler(int num)
    {
        int start = startEndIndex[num,0];
        int end = startEndIndex[num,1];
        Debug.Log(start + " " + end);

        foreach (GameObject row in rows)
        {
            row.SetActive(false);
        }

        for (int i = start; i <= end; i++)
        {
            rows[i].SetActive(true);
        }

        catSelection.SetActive(false);
        charSelection.SetActive(true);
    }

    public void ToCategorySelection()
    {
        selected = "";
        catSelection.SetActive(true);
        charSelection.SetActive(false);
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

    public void ToSkilltree()
    {
        SceneManager.LoadScene("SkillTree");
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
