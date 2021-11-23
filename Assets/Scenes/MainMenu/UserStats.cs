using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Npgsql;
using UnityEngine;

public static class UserStats
{
    // Should get any stats that have to be used or updated
    public static int userId;
    private static DateTime lastLogin; // for calculating total play time
    public static Dictionary<int, string[]> characters = new Dictionary<int, string[]>();
    // TODO: update actual number
    public static string[,] loreDetails = new string[13,2];

    public static void Initialise(string username, NpgsqlConnection conn)
    {
        conn.Open();
        // users
        using (var cmd = new NpgsqlCommand("SELECT * FROM users WHERE username=@u", conn))
        {
            string tempVal;
            string field;
            cmd.Parameters.AddWithValue("u", username);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    tempVal = reader[i].ToString();
                    field = reader.GetName(i);
                    // prolly can use a switch??
                    if (field == "user_id")
                    {
                        UserStats.userId = int.Parse(tempVal);
                    }
                    else if (field == "last_login")
                    {
                        UserStats.lastLogin = DateTime.Parse(tempVal); //might want to parseexact
                    }
                    else
                    {
                        // Debug.Log("nothing: " + field);
                    }
                }
            }
        }
        conn.Close();
        conn.Open();
        // chars - can do an order by? so the most recently played comes first
        using (var cmd = new NpgsqlCommand("SELECT * FROM characters WHERE user_id=@u ORDER BY last_accessed DESC", conn))
        {
            string tempVal;
            string field;
            int charId = -1;
            string charName = "";
            string charLevel = "";
            string charLastPlayed = "";
            cmd.Parameters.AddWithValue("u", UserStats.userId);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    tempVal = reader[i].ToString();
                    field = reader.GetName(i);
                    if (field == "character_id")
                    {
                        charId = int.Parse(tempVal);
                    }
                    else if (field == "character_name")
                    {
                        charName = tempVal;
                    }
                    else if (field == "character_level")
                    {
                        charLevel = tempVal;
                    }
                    else if (field == "last_accessed")
                    {
                        charLastPlayed = tempVal;
                    }
                    else
                    {
                        // Debug.Log("nothing: " + field);
                    }
                }

                characters.Add(charId, new string[]{charName,charLevel,charLastPlayed});
            }
        }
        conn.Close();
        conn.Open();
        // TODO: update when lore table finalised
        using (var cmd = new NpgsqlCommand("SELECT * FROM loretemp", conn))
        {
            string tempVal;
            string field;
            int loreIndex = -1;
            string loreName = "";
            string loreDesc = "";
            NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    tempVal = reader[i].ToString();
                    field = reader.GetName(i);
                    if (field == "image_id")
                    {
                        loreIndex = int.Parse(tempVal);
                    }
                    else if (field == "name")
                    {
                        loreName = tempVal;
                    }
                    else if (field == "lore_desc")
                    {
                        loreDesc = tempVal;
                    }
                    else
                    {
                        // Debug.Log("nothing: " + field);
                    }
                }

                loreDetails[loreIndex,0] = loreName;
                loreDetails[loreIndex,1] = loreDesc;
                // Debug.Log("lore index: " + loreIndex);
            }
        }
        conn.Close();
    }
}