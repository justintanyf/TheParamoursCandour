using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Npgsql;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public GameObject characterOptions;
    public GameObject mainMenu;
    public GameObject addCharacterFields;
    public GameObject deleteConfirmation;
    public GameObject tableContents;
    public GameObject tableDataTemplate;
    public Canvas invalidCharNameCanvas;
    public Dictionary<int, GameObject> tableRows = new Dictionary<int, GameObject>();
    public TMP_Text charDeleteText;
    public Button addCharButton;
    public Button backCharButton;
    public Button deleteCharButton;
    public Button playButton;
    private NpgsqlConnection conn = LoginPageScript.GetConnection();
    private string charName;
    private int selectedCharId = -1;
    private Color32 selectedColour = new Color32(63, 130, 219, 100);
    private Color32 deselectedColour = new Color32(0, 0, 0, 0);

    void Start()
    {
        foreach (KeyValuePair<int, string[]> character in UserStats.characters)
        {
            //Debug.Log("charid is: " + UserStats.characters[i][0] + " char name is: " + UserStats.characters[i][1] + " at level " + UserStats.characters[i][2] + " last played at " + UserStats.characters[i][3]);
            int currCharId = character.Key;
            GameObject currChar = Instantiate(tableDataTemplate, tableContents.transform);
            Text[] charFields = currChar.GetComponentsInChildren<Text>();
            charFields[0].text = character.Value[0];
            charFields[1].text = character.Value[1];
            charFields[2].text = character.Value[2];
            tableRows.Add(currCharId, currChar);
            Image currCharImage = currChar.GetComponent<Image>();

            currChar.AddComponent(typeof(EventTrigger));
            EventTrigger trigger = currChar.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(delegate { SetSelectedChar(currCharId, currCharImage); });
            trigger.triggers.Add(entry); 
            currChar.SetActive(true);
        }
    }

    public void SetSelectedChar(int charId, Image charImage)
    {
        if (charImage.color == selectedColour)
        {
            charImage.color = deselectedColour;
            selectedCharId = -1;
            // nothing selected
            addCharButton.gameObject.SetActive(true);
            backCharButton.gameObject.SetActive(true);
            deleteCharButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(false);
        }
        else 
        {
            foreach (Transform child in tableContents.transform) // can also use tableRows
            {
                GameObject temp = child.gameObject;
                temp.GetComponent<Image>().color = deselectedColour;
            }
            charImage.color = selectedColour;
            selectedCharId = charId;
            // something selected
            addCharButton.gameObject.SetActive(false);
            backCharButton.gameObject.SetActive(false);
            deleteCharButton.gameObject.SetActive(true);
            playButton.gameObject.SetActive(true);
        }
        Debug.Log(selectedCharId);
    }

    public void SetCharNameField(string s)
    {
        charName = s;
    }
    
    public void SelectCharacter()
    { 
        characterOptions.SetActive(true);
        mainMenu.SetActive(false);
        addCharacterFields.SetActive(false);
        deleteConfirmation.SetActive(false);
        invalidCharNameCanvas.enabled = false;
    }

    public void CharacterBack()
    {
        characterOptions.SetActive(false);
        mainMenu.SetActive(true);
        addCharacterFields.SetActive(false);
        deleteConfirmation.SetActive(false);
        invalidCharNameCanvas.enabled = false;
    }

    public void OpenAddCharacter()
    {
        characterOptions.SetActive(false);
        mainMenu.SetActive(false);
        addCharacterFields.SetActive(true);
        deleteConfirmation.SetActive(false);
        invalidCharNameCanvas.enabled = false;
    }

    public void ToDeleteCharacter()
    {
        charDeleteText.text = UserStats.characters[selectedCharId][0];
        characterOptions.SetActive(false);
        mainMenu.SetActive(false);
        addCharacterFields.SetActive(false);
        deleteConfirmation.SetActive(true);
        invalidCharNameCanvas.enabled = false;
    }

    public void AddCharacter()
    {
        bool exists = false;
        foreach (KeyValuePair<int, string[]> character in UserStats.characters)
        {
            if (character.Value[0] == charName)
            {
                exists = true;
                invalidCharNameCanvas.enabled = true;
                break;
            }
        }

        if (exists)
        {
            // ui error
            Debug.Log("charname exists");
            return;
        }

        if (!Verification.VerifyCharacterAdd(charName, conn))
        {
            // ui error
            Debug.Log("char add issue");
            return;
        }

        CharStats.Initialise(CharStats.charId, conn);
        Underthing.Initialise(); // cos new char, just initialise no need deserialisation
        SceneManager.LoadScene("Prologue Scene");
    }

    // delete character
    public void DeleteCharacter()
    {
        if (!Verification.VerifyCharacterDelete(selectedCharId, conn))
        {
            // ui error
            Debug.Log("char delete issue");
            return;
        }

        UserStats.characters.Remove(selectedCharId);
        Destroy(tableRows[selectedCharId]);
        tableRows.Remove(selectedCharId);
        SelectCharacter();
        // set to nothing selected
        foreach (Transform child in tableContents.transform) // can also use tableRows
        {
            GameObject temp = child.gameObject;
            temp.GetComponent<Image>().color = deselectedColour;
        }
        selectedCharId = -1;
        addCharButton.gameObject.SetActive(true);
        backCharButton.gameObject.SetActive(true);
        deleteCharButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
    }

    // start with character 
    public void StartGame()
    {
        CharStats.Initialise(selectedCharId, conn);

        LoadByDeSerialization();

        if (CharStats.prologueCompleted)
        {
            SceneManager.LoadScene("ExplorationScene");
        }
        else 
        {
            SceneManager.LoadScene("Prologue Scene");
        }
    }

    public void LoadByDeSerialization()
    {
        if (File.Exists(Application.persistentDataPath + "/" + CharStats.charId + " Data.txt"))
        {
            // load game
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(Application.persistentDataPath + "/" + CharStats.charId + " Data.txt", FileMode.Open);
            Save save = bf.Deserialize(fileStream) as Save;
            fileStream.Close();

            // set everything to the other classes
            //UserStats.xAndYAndZCoords = save.xAndYAndZCoords;
            CharStats.xCoord = save.xCoord;
            CharStats.yCoord = save.yCoord;
            CharStats.haveLastCoords = save.haveLastCoords;
            Underthing.rooms = save.rooms;
            // Underthing.adjList = save.adjList;
            CharStats.currRoom = save.currRoom;
        }
        else
        {
            // no save file
            Underthing.Initialise();
        }
    }
}
