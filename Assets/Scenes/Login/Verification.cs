using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Npgsql;
using UnityEngine;

public static class Verification
{
    // TODO: for all, check for null and use try/catch i guess
    public static bool VerifyEmail(string email)
    {
        string pattern = @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])";
        Regex regex = new Regex(pattern);
        Match match = regex.Match(email);
        return match.Success;
    }

    public static bool VerifyUniqueUsername(string username, NpgsqlConnection conn)
    {
        conn.Open();
        Int64 output;
        using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username=@u", conn))
        {
            cmd.Parameters.AddWithValue("u", username);
            output = (Int64) cmd.ExecuteScalar();
        }
        conn.Close();
        return (output == 0);
    }

    public static bool VerifyPassword(string password) 
    {
        // TODO: create some restrictions for password
        return true;
    }

    public static bool VerifySignUp(string username, string password, string email, DateTime currentDate, NpgsqlConnection conn)
    {
        // params to avoid sql injection and improve performance
        conn.Open();
        int output;
        using (var cmd = new NpgsqlCommand("INSERT INTO users (username,password,email,created_on,last_login) VALUES (@u,@p,@e,@c,@l)", conn))
        {
            cmd.Parameters.AddWithValue("u", username);
            cmd.Parameters.AddWithValue("p", password);
            cmd.Parameters.AddWithValue("e", email);
            cmd.Parameters.AddWithValue("c", currentDate);
            cmd.Parameters.AddWithValue("l", currentDate);
            output = cmd.ExecuteNonQuery();
        }
        conn.Close();
        return (output == 1);
    }

    public static bool VerifyLogIn(string username, string password, NpgsqlConnection conn)
    {
        conn.Open();
        Int64 output;
        using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE (username=@u OR email=@u)AND password=@p", conn))
        {
            cmd.Parameters.AddWithValue("u", username);
            cmd.Parameters.AddWithValue("p", password);
            output = (Int64) cmd.ExecuteScalar();
        }
        conn.Close();
        return (output == 1);
    }

    public static bool VerifyCharacterAdd(string charName, NpgsqlConnection conn)
    {
        conn.Open();
        DateTime currentDate = DateTime.Now;
        int output;
        using (var cmd = new NpgsqlCommand("INSERT INTO characters (user_id,last_accessed,character_name,character_level) VALUES (@u,@l,@cn,@cl)", conn))
        {
            cmd.Parameters.AddWithValue("u", UserStats.userId);
            cmd.Parameters.AddWithValue("l", currentDate);
            cmd.Parameters.AddWithValue("cn", charName);
            cmd.Parameters.AddWithValue("cl", 0);
            output = cmd.ExecuteNonQuery();
        }
        // if output is 1, also insert into skills and progress table
        if (output != 1)
        {
            conn.Close();
            return false;
        }
        // in actual table, have table schema have default to 0
        int charId;
        using (var cmd = new NpgsqlCommand("SELECT character_id FROM characters WHERE user_id=@u AND character_name=@cn", conn))
        {
            cmd.Parameters.AddWithValue("u", UserStats.userId);
            cmd.Parameters.AddWithValue("cn", charName);
            charId = (int) cmd.ExecuteScalar();
        }
        using (var cmd = new NpgsqlCommand("INSERT INTO skills (character_id) VALUES (@c)", conn))
        {
            cmd.Parameters.AddWithValue("c", charId);
            output = cmd.ExecuteNonQuery();
        }
        if (output != 1)
        {
            conn.Close();
            return false;
        }
        using (var cmd = new NpgsqlCommand("INSERT INTO progress (character_id) VALUES (@c)", conn))
        {
            cmd.Parameters.AddWithValue("c", charId);
            output = cmd.ExecuteNonQuery();
        }
        if (output != 1)
        {
            conn.Close();
            return false;
        }
        // TODO: update to inventory
        using (var cmd = new NpgsqlCommand("INSERT INTO inventorytemp (character_id) VALUES (@c)", conn))
        {
            cmd.Parameters.AddWithValue("c", charId);
            output = cmd.ExecuteNonQuery();
        }
        if (output != 1)
        {
            conn.Close();
            return false;
        }
        // TODO: update to chest, reset to normal vals
        using (var cmd = new NpgsqlCommand("INSERT INTO chesttemp (character_id, chest1, chest2, chest5) VALUES (@c, 1, 4, 99)", conn))
        {
            cmd.Parameters.AddWithValue("c", charId);
            output = cmd.ExecuteNonQuery();
        }

        conn.Close();
        CharStats.charId = charId;
        return (output == 1);
    }

    public static bool VerifyCharacterDelete(int charId, NpgsqlConnection conn)
    {
        conn.Open();
        int output;
        using (var cmd = new NpgsqlCommand("DELETE FROM characters WHERE character_id=@c", conn))
        {
            cmd.Parameters.AddWithValue("c", charId);
            output = cmd.ExecuteNonQuery();
        }
        conn.Close();
        return (output == 1);
    }
}
