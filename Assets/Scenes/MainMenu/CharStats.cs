using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Npgsql;
using UnityEngine;

public static class CharStats
{
    // what is here? should be skilltree, level, total play time, coords, room
    public static int charId;
    public static string charName;
    public static int charLevel;
    public static bool prologueCompleted;
    // private static int playtime;
    public static float xCoord;
    public static float yCoord;
    public static bool haveLastCoords = false;
    public static int entryDoor = -1;
    public static int currRoom = 0;
    public static int tempCounter = 1;
    
    public static string kvotheName = "Kvothe";
    public static int curHealth = 100;
    public static int maxHealth = 100;
    public static int curMana = 100;
    public static int maxMana = 100;
    public static int swordBaseDamage = 25;
    public static int numFaeDefeated = 0; // TODO: load this from db
    
    //public static List<Item> savedInventory;
    public static int[] savedInventory = new int[9]; // TODO: update this number when finalised
    public static int[] savedChest = new int[9]; // TODO: update this number when finalised
    public static bool[] acquiredRecipes = new bool[] {true, true, true, true, true}; // temp
    public static bool[] availableShopItems = new bool[] {true, true, true, true, true}; // temp

    public static int[] miniSaved = new int[4];
    public static bool[] fragmentsFound = new bool[10];
    public static int numFragments = 0;

    // skills
    private static string[] nodeNames = { "t1l1s1", "t1l1s2", "t1l1s3", "t1l2s1", "t2l1s1", "t2l1s2",
        "t3l1s1", "t3l1s2", "t3l1s3", "t3l2s1", "t4l1s1", "t4l1s2", "t4l1s3", "t4l1s4", "t5l1s1", "t5l1s2", "t5l2s1", "t5l3s1" };
    private static Dictionary<String, Node> nodes = new Dictionary<String, Node>();
    private static Dictionary<String, String[]> prereqs = new Dictionary<String, String[]> {
        { "t1l1s1", new String[] {} },
        { "t1l1s2", new String[] {} },
        { "t1l1s3", new String[] {} },
        { "t1l2s1", new String[] { "t1l1s1", "t1l1s2", "t1l1s3" } },
        { "t2l1s1", new String[] {} },
        { "t2l1s2", new String[] {} },
        { "t3l1s1", new String[] {} },
        { "t3l1s2", new String[] {} },
        { "t3l1s3", new String[] {} },
        { "t3l2s1", new String[] { "t3l1s1", "t3l1s2", "t3l1s3" } },
        { "t4l1s1", new String[] {} },
        { "t4l1s2", new String[] {} },
        { "t4l1s3", new String[] {} },
        { "t4l1s4", new String[] {} },
        { "t5l1s1", new String[] {} },
        { "t5l1s2", new String[] {} },
        { "t5l2s1", new String[] { "t5l1s1", "t5l1s2" } },
        { "t5l3s1", new String[] { "t5l2s1" } }
    };
    private static Dictionary<string, int> maxLevels = new Dictionary<string, int> {
        { "t1l1s1", 3 },
        { "t1l1s2", 3 },
        { "t1l1s3", 3 },
        { "t1l2s1", 1 },
        { "t2l1s1", 1 },
        { "t2l1s2", 1 },
        { "t3l1s1", 1 },
        { "t3l1s2", 1 },
        { "t3l1s3", 1 },
        { "t3l2s1", 1 },
        { "t4l1s1", 1 },
        { "t4l1s2", 1 },
        { "t4l1s3", 1 },
        { "t4l1s4", 1 },
        { "t5l1s1", 3 },
        { "t5l1s2", 3 },
        { "t5l2s1", 1 },
        { "t5l3s1", 1 }
    };
    private static Dictionary<string, string> nodeDescs = new Dictionary<string, string> {
        { "t1l1s1", "t1l1s1 description" },
        { "t1l1s2", "t1l1s2 description" },
        { "t1l1s3", "t1l1s3 description" },
        { "t1l2s1", "t1l2s1 description" },
        { "t2l1s1", "t2l1s1 description" },
        { "t2l1s2", "t2l1s2 description" },
        { "t3l1s1", "t3l1s1 description" },
        { "t3l1s2", "t3l1s2 description" },
        { "t3l1s3", "t3l1s3 description" },
        { "t3l2s1", "t3l2s1 description" },
        { "t4l1s1", "t4l1s1 description" },
        { "t4l1s2", "t4l1s2 description" },
        { "t4l1s3", "t4l1s3 description" },
        { "t4l1s4", "t4l1s4 description" },
        { "t5l1s1", "t5l1s1 description" },
        { "t5l1s2", "t5l1s2 description" },
        { "t5l2s1", "t5l2s1 description" },
        { "t5l3s1", "t5l3s1 description" }
    };

    // INITIALISE
    public static void Initialise(int charId, NpgsqlConnection conn)
    {
        CharStats.charId = charId;
        conn.Open();

        // skills + progress + inventory + chest
        // TODO: update to inventory + chest
        using (var cmd = new NpgsqlCommand("SELECT * FROM characters c INNER JOIN skills s ON c.character_id = s.character_id " + 
            "INNER JOIN progress pg ON c.character_id = pg.character_id " + 
            "INNER JOIN chesttemp cht ON c.character_id = cht.character_id " +
            "INNER JOIN inventorytemp it ON c.character_id = it.character_id WHERE c.character_id=@c", conn))
        {
            string tempVal;
            string field;
            Regex skillRegex = new Regex("([t][0-9]+)([l][0-9]+)([s][0-9]+)");
            cmd.Parameters.AddWithValue("c", charId);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    tempVal = reader[i].ToString();
                    field = reader.GetName(i);
                    Match match = skillRegex.Match(field);
                
                    // now need to store the data into the vars
                    // prolly can use a switch??
                    if (field == "character_name")
                    {
                        charName = tempVal;
                    }
                    else if (field == "character_level")
                    {
                        charLevel = int.Parse(tempVal);
                    }
                    else if (field == "prologue_completed")
                    {
                        prologueCompleted = bool.Parse(tempVal);
                    }
                    else if (field == "curr_hp")
                    {
                        curHealth = int.Parse(tempVal);
                    }
                    else if (field == "max_hp")
                    {
                        maxHealth = int.Parse(tempVal);
                    }
                    else if (field == "curr_mp")
                    {
                        curMana = int.Parse(tempVal);
                    }
                    else if (field == "max_mp")
                    {
                        maxMana = int.Parse(tempVal);
                    }
                    else if (field == "mini1_save")
                    {
                        miniSaved[0] = int.Parse(tempVal);
                    }
                    else if (field == "mini2_save")
                    {
                        miniSaved[1] = int.Parse(tempVal);
                    }
                    else if (field == "mini3_save")
                    {
                        miniSaved[2] = int.Parse(tempVal);
                    }
                    else if (field == "mini4_save")
                    {
                        miniSaved[3] = int.Parse(tempVal);
                    }
                    else if (field == "fragment1_found")
                    {
                        fragmentsFound[0] = bool.Parse(tempVal);
                    }
                    else if (field == "fragment2_found")
                    {
                        fragmentsFound[1] = bool.Parse(tempVal);
                    }
                    else if (field == "fragment3_found")
                    {
                        fragmentsFound[2] = bool.Parse(tempVal);
                    }
                    else if (field == "fragment4_found")
                    {
                        fragmentsFound[3] = bool.Parse(tempVal);
                    }
                    else if (field == "fragment5_found")
                    {
                        fragmentsFound[4] = bool.Parse(tempVal);
                    }
                    else if (field == "fragment6_found")
                    {
                        fragmentsFound[5] = bool.Parse(tempVal);
                    }
                    else if (field == "fragment7_found")
                    {
                        fragmentsFound[6] = bool.Parse(tempVal);
                    }
                    else if (field == "fragment8_found")
                    {
                        fragmentsFound[7] = bool.Parse(tempVal);
                    }
                    else if (field == "fragment9_found")
                    {
                        fragmentsFound[8] = bool.Parse(tempVal);
                    }
                    else if (field == "fragment10_found")
                    {
                        fragmentsFound[9] = bool.Parse(tempVal);
                    }
                    else if (field == "item1")
                    {
                        savedInventory[0] = int.Parse(tempVal);
                    }
                    else if (field == "item2")
                    {
                        savedInventory[1] = int.Parse(tempVal);
                    }
                    else if (field == "item3")
                    {
                        savedInventory[2] = int.Parse(tempVal);
                    }
                    else if (field == "item4")
                    {
                        savedInventory[3] = int.Parse(tempVal);
                    }
                    else if (field == "item5")
                    {
                        savedInventory[4] = int.Parse(tempVal);
                    }
                    else if (field == "item6")
                    {
                        savedInventory[5] = int.Parse(tempVal);
                    }
                    else if (field == "item7")
                    {
                        savedInventory[6] = int.Parse(tempVal);
                    }
                    else if (field == "item8")
                    {
                        savedInventory[7] = int.Parse(tempVal);
                    }
                    else if (field == "item9")
                    {
                        savedInventory[8] = int.Parse(tempVal);
                    }
                    else if (field == "chest1")
                    {
                        savedChest[0] = int.Parse(tempVal);
                    }
                    else if (field == "chest2")
                    {
                        savedChest[1] = int.Parse(tempVal);
                    }
                    else if (field == "chest3")
                    {
                        savedChest[2] = int.Parse(tempVal);
                    }
                    else if (field == "chest4")
                    {
                        savedChest[3] = int.Parse(tempVal);
                    }
                    else if (field == "chest5")
                    {
                        savedChest[4] = int.Parse(tempVal);
                    }
                    else if (field == "chest6")
                    {
                        savedChest[5] = int.Parse(tempVal);
                    }
                    else if (field == "chest7")
                    {
                        savedChest[6] = int.Parse(tempVal);
                    }
                    else if (field == "chest8")
                    {
                        savedChest[7] = int.Parse(tempVal);
                    }
                    else if (field == "chest9")
                    {
                        savedChest[8] = int.Parse(tempVal);
                    }
                    else if (match.Success)
                    {
                        Node[] prereq = prereqs[field].Select(nodeName => nodes[nodeName]).ToArray();
                        nodes.Add(field, new Node(int.Parse(tempVal), maxLevels[field], prereq, nodeDescs[field]));
                    }
                    else
                    {
                        // Debug.Log("char nothing: " + field);
                    }
                }
            }
        }

        conn.Close();

        for (int i = 0; i < 10; i++)
        {
            if (fragmentsFound[i]) numFragments++;
        }
        
        // Setup initial Values here, although I realise that the health and mana should be stored in sql
        // curHealth = maxHealth;
        // curMana = maxMana;
        
        // Set the list of items here 
        // savedInventory = from DB
    }

    public static void SetLastCoords(Vector3 lastCoords)
    {
        xCoord = lastCoords.x;
        yCoord = lastCoords.y;
        haveLastCoords = true;
    }

    public static Vector3 GetLastCoords()
    {
        haveLastCoords = false;
        return new Vector3(xCoord, yCoord, 0);;
    }

    public static void SetCurrRoom(int newRoom)
    {
        currRoom = newRoom;
    }

    public static int GetCurrRoom()
    {
        return currRoom;
    }

    public static Node GetNode(string node)
    {
        return nodes[node];
    }

    public static int GetCurrentHP()
    {
        return curHealth;
    }

    public static int GetMaxHP()
    {
        return maxHealth;
    }

    public static int GetCurrentMana()
    {
        return curMana;
    }

    public static int GetMaxMana()
    {
        return maxMana;
    }
    
    public static bool TakeDamage(int damage)
    {
        curHealth -= damage;
        if (curHealth <= 0)
            return true;
        else
            return false;
    }
    
    public static void Heal(int healAmount)
    {
        curHealth += healAmount;

        if (curHealth > maxHealth)
            curHealth = maxHealth;
    }

    public static void DrainMana(int manaUsed)
    {
        curMana -= manaUsed;
    }
    
    public static void IncreaseMana(int increaseManaAmount)
    {
        curMana += increaseManaAmount;
    }

    public static int GetDamageValue()
    {
        // Ideally have all the math calculated here based on the buffs
        return swordBaseDamage;
    }

    public static int GetCharLevel()
    {
        return charLevel;
    }

    public static bool GetDefeatedFireMiniBoss()
    {
        return miniSaved[0] == 1
            ? true
            : false;
    }
    
    public static bool GetDefeatedWaterMiniBoss()
    {
        return miniSaved[1] == 1
            ? true
            : false;
    }
}
