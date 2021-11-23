using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using TMPro;

public class Move : MonoBehaviour
{
    public GameObject PlayerAvatar;
    private Rigidbody2D rb;
    private float dirX, dirY, moveSpeed;
    public Room currRoom;
    public GameObject itemEncounterMenu;
    public GameObject faeEncounterMenu;
    public GameObject bedMenu, chestMenu, craftingMenu, merchantMenu, defaultMenu;
    public GameObject logContents;
    public GameObject logTextTemplate;
    public bool defaultActive = true;
    public Tilemap groundMap;
    public Tilemap objsMap;
    public bool stopped = false;
    public int objIndex = -1;
    private int countFrames = 0;
    public MapLoader currentMapLoader;
    private bool encounteredEnemyToPassExists = false;
    public GameObject encounteredEnemyToPass;
    #region Singleton

    public static Move instance;

    void Awake ()
    {
        instance = this;
    }

    #endregion

    // Initialisation
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = 5f;

    }

    // Update is called once per frame
    void Update()
    {        
        if (stopped) return;

        dirX = Input.GetAxisRaw("Horizontal") * moveSpeed;
        dirY = Input.GetAxisRaw("Vertical") * moveSpeed;

        // change clamps dynamically, as well as start position
        transform.position = new Vector2(
            Mathf.Clamp(transform.position.x, -100f, 100f),
            Mathf.Clamp(transform.position.y, -100f, 100f)
        );

        // items
        for (int i = 0; i < currRoom.itemsPos.Count; i++)
        {   
            // might want to make it bounded also to allow for player to reach it from further away (or just +-1 here)
            // can do a binary search kinda thing as well
            if (currRoom.itemsPos[i][0] == Math.Round(rb.position.x) && currRoom.itemsPos[i][1] == Math.Round(rb.position.y))
            {
                stopped = true;
                Debug.Log("wuhu");
                objIndex = i;
                itemEncounterMenu.transform.position = new Vector3(rb.position.x, rb.position.y, 0);
                dirX = 0;
                dirY = 0;
                itemEncounterMenu.SetActive(true);
                break;
            }
        }
        if (stopped) return;

        // fae
        for (int i = 0; i < currRoom.faesPos.Count; i++)
        {
            if (currRoom.faesPos[i][0] < rb.position.x && currRoom.faesPos[i][1] > rb.position.x &&
                currRoom.faesPos[i][2] < rb.position.y && currRoom.faesPos[i][3] > rb.position.y)
            {
                stopped = true;
                Debug.Log("battleeee");
                objIndex = i;
                faeEncounterMenu.transform.position = new Vector3(rb.position.x, rb.position.y, 0);
                dirX = 0;
                dirY = 0;
                faeEncounterMenu.SetActive(true);
                break;
            }
        }
        if (stopped) return;

        // player room
        if (currRoom.isPlayerRoom)
        {
            if (!CharStats.fragmentsFound[1] && Math.Round(rb.position.y) > 13)
            {
                CharStats.fragmentsFound[1] = true;
                AddToLog("You've found a memory fragment! The memory hits you quite suddenly - in it, Auri lets you into her alchemy lab for the first time.......");
                AddToLog("This memory fragment has also unlocked the Alchemy subtree.");
                // call verification to store in db
            }
            
            if (currRoom.playerRoomItems[1,0] == Math.Round(rb.position.x) && currRoom.playerRoomItems[1,1] == Math.Round(rb.position.y))
            {
                BedInteraction();
            }
            else if (currRoom.playerRoomItems[0,0] - 1 < rb.position.x && currRoom.playerRoomItems[0,0] + 1 > rb.position.x &&
                currRoom.playerRoomItems[0,1] - 1 < rb.position.y && currRoom.playerRoomItems[0,1] + 1 > rb.position.y && !currRoom.auriInteracted)
            {
                AuriInteraction();
            }
            else if (currRoom.playerRoomItems[2,0] - 1 < rb.position.x && currRoom.playerRoomItems[2,0] + 1 > rb.position.x &&
                currRoom.playerRoomItems[2,1] - 1 < rb.position.y && currRoom.playerRoomItems[2,1] + 1 > rb.position.y)
            {
                ChestInteraction();
            }
            else if (currRoom.playerRoomItems[3,0] - 1 < rb.position.x && currRoom.playerRoomItems[3,0] + 1 > rb.position.x &&
                currRoom.playerRoomItems[3,1] - 1 < rb.position.y && currRoom.playerRoomItems[3,1] + 1 > rb.position.y)
            {
                CraftingInteraction();
            }
            else if (!defaultActive)
            {
                DefaultInteraction();
            }
        }

        if (CharStats.tempCounter == 2 && currRoom.coinLocation[0] == Math.Round(rb.position.x) && currRoom.coinLocation[1] == Math.Round(rb.position.y))
        {
            CharStats.fragmentsFound[3] = true;
            Tile coinTile = groundMap.GetTile<Tile>(new Vector3Int(currRoom.coinLocation[0],currRoom.coinLocation[1],0));
            coinTile.gameObject = null;
            groundMap.RefreshTile(new Vector3Int(currRoom.coinLocation[0],currRoom.coinLocation[1],0));
            currRoom.coinLocation[0] = -1;
            currRoom.coinLocation[1] = -1;
            AddToLog("You've found a memory fragment! The memory hits you quite suddenly - in it, Auri is giving you a coin, which she says will keep you safe at night.......");
            // call verification to store in db
        }

        // merchant
        if (currRoom.hasMerchant)
        {
            // +- 1 merchant location
            if (currRoom.merchantLocation[0] - 1 < rb.position.x && currRoom.merchantLocation[0] + 1 > rb.position.x &&
                currRoom.merchantLocation[1] - 1 < rb.position.y && currRoom.merchantLocation[1] + 1 > rb.position.y)
            {
                MerchantInteraction();
            }
            else if (!defaultActive)
            {
                DefaultInteraction();
            }
        }

        // feather
        if (!CharStats.fragmentsFound[8] && currRoom.hasFeather)
        {
            if (currRoom.featherLocation[0] == Math.Round(rb.position.x) && currRoom.featherLocation[1] == Math.Round(rb.position.y))
            {
                CharStats.fragmentsFound[8] = true;
                AddToLog("You've found a memory fragment! The memory hits you quite suddenly - in it, Auri has brought you a feather with the spring wind in it; however, she did not give it to you because you were late to meeting her.......");
                // TODO: call verification to store in db
                Tile tile = groundMap.GetTile<Tile>(new Vector3Int(currRoom.featherLocation[0], currRoom.featherLocation[1], 0));
                tile.gameObject = null;
                currRoom.hasFeather = false;
            }
        }

        // doors
        if (currRoom.doors[0,0] != -1 && currRoom.doors[0,1] != -1)
        {
            if (currRoom.doors[0,0] == Math.Round(rb.position.x) && 
                (currRoom.doors[0,1] == Math.Round(rb.position.y) || currRoom.doors[0,1] + 1 == Math.Round(rb.position.y)))
            {
                if (currRoom.diffDoor)
                {
                    GoToRoom(currRoom.connections[0], currRoom.actualDoor[0]);
                }
                else
                {
                    GoToRoom(currRoom.connections[0], 2);
                }
            }
        }
        if (currRoom.doors[1,0] != -1 && currRoom.doors[1,1] != -1)
        {
            if ((currRoom.doors[1,0] == Math.Round(rb.position.x) || currRoom.doors[1,0] + 1 == Math.Round(rb.position.x)) && 
                (currRoom.doors[1,1] == Math.Round(rb.position.y)))
            {
                if (currRoom.diffDoor)
                {
                    GoToRoom(currRoom.connections[1], currRoom.actualDoor[1]);
                }
                else
                {
                    GoToRoom(currRoom.connections[1], 3);
                }
            }
        }
        if (currRoom.doors[2,0] != -1 && currRoom.doors[2,1] != -1)
        {
            if (currRoom.doors[2,0] == Math.Round(rb.position.x) && 
                (currRoom.doors[2,1] == Math.Round(rb.position.y) || currRoom.doors[2,1] - 1 == Math.Round(rb.position.y)))
            {
                if (currRoom.diffDoor)
                {
                    GoToRoom(currRoom.connections[2], currRoom.actualDoor[2]);
                }
                else
                {
                    GoToRoom(currRoom.connections[2], 0);
                }
            }
        }
        if (currRoom.doors[3,0] != -1 && currRoom.doors[3,1] != -1)
        {
            if ((currRoom.doors[3,0] == Math.Round(rb.position.x) || currRoom.doors[3,0] - 1 == Math.Round(rb.position.x)) && 
                (currRoom.doors[3,1] == Math.Round(rb.position.y)))
            {
                if (currRoom.diffDoor)
                {
                    GoToRoom(currRoom.connections[3], currRoom.actualDoor[3]);
                }
                else
                {
                    GoToRoom(currRoom.connections[3], 1);
                }
            }
        }

        if (CharStats.haveLastCoords && countFrames > 0)
        {
            countFrames--;
        }
        else if (CharStats.haveLastCoords)
        {
            PlayerAvatar.transform.position = CharStats.GetLastCoords();
        }
    }

    public void AuriInteraction()
    {
        // switch based on how many memory fragments have been found
        currRoom.auriInteracted = true;
        AddToLog("is auri uwu");
    }
    
    public void BedInteraction()
    {
        bedMenu.SetActive(true);
        chestMenu.SetActive(false);
        craftingMenu.SetActive(false);
        merchantMenu.SetActive(false);
        defaultMenu.SetActive(false);
        defaultActive = false;
    }

    public void BedButton()
    {
        CharStats.curHealth = CharStats.maxHealth;
        CharStats.curMana = CharStats.maxMana;
        AddToLog("You wake up well rested, with your health and mana fully restored.");
        if (!CharStats.fragmentsFound[0])
        {
            CharStats.fragmentsFound[0] = true;
            AddToLog("You've found a memory fragment! The memory hits you quite suddenly - in it, Auri is showing you to your bed.......");
            // call verification to store in db
        }
    }

    public void ChestInteraction()
    {
        if (!CharStats.fragmentsFound[2])
        {
            CharStats.fragmentsFound[2] = true;
            AddToLog("You've found a memory fragment! The memory hits you quite suddenly - in it, Auri is showing to you a key that can open the moon.......");
            // call verification to store in db
        }
        
        bedMenu.SetActive(false);
        chestMenu.SetActive(true);
        craftingMenu.SetActive(false);
        merchantMenu.SetActive(false);
        defaultMenu.SetActive(false);
        defaultActive = false;
    }

    public void CraftingInteraction()
    {
        bedMenu.SetActive(false);
        chestMenu.SetActive(false);
        craftingMenu.SetActive(true);
        merchantMenu.SetActive(false);
        defaultMenu.SetActive(false);
        defaultActive = false;
    }

    public void MerchantInteraction()
    {
        bedMenu.SetActive(false);
        chestMenu.SetActive(false);
        craftingMenu.SetActive(false);
        merchantMenu.SetActive(true);
        defaultMenu.SetActive(false);
        defaultActive = false;
    }

    public void AddToLog(string text)
    {
        GameObject currRow = Instantiate(logTextTemplate, logContents.transform);
        TMP_Text logText = currRow.GetComponentInChildren<TMP_Text>();
        DateTime dt = DateTime.Now;
        string dtStr = dt.ToString("HH:mm");
        string finalText = "[" + dtStr + "] " + text;
        logText.text = finalText;
        currRoom.textList.Add(finalText);
        currRow.SetActive(true);
    }

    public void DefaultInteraction()
    {
        bedMenu.SetActive(false);
        chestMenu.SetActive(false);
        craftingMenu.SetActive(false);
        merchantMenu.SetActive(false);
        defaultMenu.SetActive(true);
        defaultActive = true;
    }
    
    public void ItemEncounterDone()
    {
        int x = currRoom.itemsPos[objIndex][0];
        int y = currRoom.itemsPos[objIndex][1];
        Vector3Int p = new Vector3Int(x,y,0);
        Tile tile = groundMap.GetTile<Tile>(p);
        tile.gameObject = null;
        currRoom.itemsPos.RemoveAt(objIndex);
        groundMap.RefreshTile(p);
        currRoom.cells[x, y].prefab = 0;
        currRoom.cells[x, y].hasObj = false;
        stopped = false;
    }

    private Save createSaveGameObject()
    {
        Save save = new Save();
        // need to set to whatever the current coords are
        save.xCoord = rb.position.x;
        save.yCoord = rb.position.y;
        save.haveLastCoords = true;
        save.rooms = Underthing.rooms;
        // save.adjList = Underthing.adjList;
        save.currRoom = CharStats.currRoom;
        return save;
    }

    public void GoToRoom(int nextRoom, int doorToComeOutFrom)
    {
        stopped = true;
        Debug.Log(currRoom.diffDoor + " alarm; next room: " + nextRoom + " door to come out from: " + doorToComeOutFrom);
        currRoom.resetMerchant = true;
        currRoom.auriInteracted = false;
        currRoom.textList.Clear();
        CharStats.tempCounter++;
        CharStats.currRoom = nextRoom;
        CharStats.xCoord = Underthing.rooms[nextRoom].doors[doorToComeOutFrom,0];
        CharStats.yCoord = Underthing.rooms[nextRoom].doors[doorToComeOutFrom,1];
        switch (doorToComeOutFrom)
        {
            case 0:
                CharStats.xCoord += 1;
                break;
            case 1:
                CharStats.yCoord -= 1;
                break;
            case 2:
                CharStats.xCoord -= 1;
                break;
            case 3:
                CharStats.yCoord += 1;
                break;
            default:
                Debug.Log("wtf");
                break;
        }
        CharStats.haveLastCoords = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    public void FaeEncounterDone()
    {
        CharStats.SetLastCoords(PlayerAvatar.transform.position);
        int x = currRoom.faesPos[objIndex][0] + 2; // to change if box around fae increases
        int y = currRoom.faesPos[objIndex][2] + 2; // to change if box around fae increases
        currRoom.faesPos.RemoveAt(objIndex);
        GameObject encounteredEnemy = currentMapLoader.prefabs[currRoom.cells[x, y].prefab];
        this.encounteredEnemyToPassExists = true;
        this.encounteredEnemyToPass = encounteredEnemy;
        Debug.Log(encounteredEnemy);
        currRoom.cells[x, y].prefab = 0;
        currRoom.cells[x, y].hasObj = false;
        stopped = false;
        // send to battle scene
        countFrames = 3; // to ensure the player is sent back to the same spot when re-entering exploration
        SceneManager.LoadScene("BattleScene");
    }

    public void SetEncounteredEnemyToPass(GameObject encounteredEnemy)
    {
        this.encounteredEnemyToPassExists = true;
        this.encounteredEnemyToPass = encounteredEnemy;
    }

    public bool DoesEncounteredEnemyToPassExist()
    {
        return this.encounteredEnemyToPassExists;
    }

    public GameObject GetEncounteredEnemyToPass()
    {
        return this.encounteredEnemyToPass;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(dirX, dirY);
        // rb.velocity = new Vector2(dirX * 2, dirY * 2);
    }

    public void ToSkillTree()
    {
        CharStats.SetLastCoords(PlayerAvatar.transform.position);
        SceneManager.LoadScene("SkillTree");
    }

    public void ToLore()
    {
        CharStats.SetLastCoords(PlayerAvatar.transform.position);
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
