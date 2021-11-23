using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Npgsql;

public class TestSuite
{
    private string connectString;
    private NpgsqlConnection conn;

    [SetUp]
    public void Setup()
    {
        connectString = "Server=" + "ec2-54-90-211-192.compute-1.amazonaws.com" + ";"
            + "Database=" + "d78t5ejq1fca8p" + ";"
            + "User ID=" + "vejafmwwilcxts" + ";"
            + "Password=" + "ad9ee22908db8dd733b181e7a83ba0511f01fd99039309fb03a6912ce04e660d" + ";"
            + "SSL Mode=Require;Trust Server Certificate=true;";
        conn = new NpgsqlConnection(connectString);
    }

    [UnityTest]
    public IEnumerator SignUpUsername()
    {
        string username = "acc";
        yield return null;
        Assert.AreEqual(Verification.VerifyUniqueUsername(username, conn), false);
    }

    [UnityTest]
    public IEnumerator SignUpEmail()
    {
        string email = "asd@asda";
        yield return null;
        Assert.AreEqual(Verification.VerifyEmail(email), false);
    }

    [TearDown]
    public void TearDown()
    {
        // Object.Destroy(game.gameObject);
    }
}