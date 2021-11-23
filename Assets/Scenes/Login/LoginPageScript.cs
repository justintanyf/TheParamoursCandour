using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Npgsql;
using UnityEngine.SceneManagement;


public class LoginPageScript : MonoBehaviour
{
    private TMP_InputField usernameFieldSU;
    private TMP_InputField emailFieldSU;
    private TMP_InputField passwordFieldSU;
    private TMP_InputField usernameFieldLI;
    private TMP_InputField passwordFieldLI;
    private TMP_InputField OTPFieldSU;
    private Toggle OTPToggle;
    private Canvas invalidUserNameCanvas;
    private Canvas invalidEmailCanvas;
    private Canvas invalidPasswordCanvas;
    private Canvas incorrectLoginCanvas;
    private bool signUpInitialised = false;
    private bool logInInitialised = false;
    private bool SignUpWithOTP;
    //private static int user_id;
    private string username;
    private string email;
    private string password;
    private string OTP;
    //private Button submitButton;

    public void SetUserNameField(string s)
    {
        username = s;
    }

    public void SetEmailField(string s)
    {
        email = s;
    }

    public void SetPasswordField(string s)
    {
        password = s;
    }

    public void SetOTPField(string s)
    {
        OTP = s;
    }

    public void SetSignUpWithOTP(bool b)
    {
        SignUpWithOTP = b;
    }

    public void SignUp()
    {
        if (!signUpInitialised) SignUpInitialise();
        usernameFieldSU.image.color = new Color32(255, 255, 255, 255);
        emailFieldSU.image.color = new Color32(255, 255, 255, 255);
        passwordFieldSU.image.color = new Color32(255, 255, 255, 255);
        // hide ui messsages
        invalidUserNameCanvas.enabled = false;
        invalidEmailCanvas.enabled = false;
        invalidPasswordCanvas.enabled = false;
        bool verified = true;
        if (!Verification.VerifyUniqueUsername(username, conn))
        {
            usernameFieldSU.image.color = new Color32(255, 150, 150, 255);
            invalidUserNameCanvas.enabled = true;
            verified = false;
        }
        if (!Verification.VerifyEmail(email))
        {
            // outline colour...
            emailFieldSU.image.color = new Color32(255, 150, 150, 255); // if you want more red, reduce GB, and vice-versa
            invalidEmailCanvas.enabled = true;
            verified = false;
        }
        if (!Verification.VerifyPassword(password))
        {
            passwordFieldSU.image.color = new Color32(255, 150, 150, 255);
            invalidPasswordCanvas.enabled = true;
            verified = false;
        }
        
        if(!verified) 
        {
            return;
        }
        DateTime currentDate = DateTime.Now;

        if (!Verification.VerifySignUp(username, password, email, currentDate, conn))
        {
            UnityEngine.Debug.Log("no"); // replace this with error ui msg
            return;
        }

        // load start screen (i.e. auto logged in)
        // gotta save them user details somewhere
        UserStats.Initialise(username, conn);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LogIn()
    {
        if (!logInInitialised) LogInInitialise();
        usernameFieldLI.image.color = new Color32(255, 255, 255, 255);
        passwordFieldLI.image.color = new Color32(255, 255, 255, 255);
        incorrectLoginCanvas.enabled = false;
        // hide ui messsages

        if (username == "" || username == null || password == "" || password == null)
        {
            usernameFieldLI.image.color = new Color32(255, 150, 150, 255);
            passwordFieldLI.image.color = new Color32(255, 150, 150, 255);
            incorrectLoginCanvas.enabled = true;
            return;
        }

        if (!Verification.VerifyLogIn(username, password, conn))
        {
            usernameFieldLI.image.color = new Color32(255, 150, 150, 255);
            passwordFieldLI.image.color = new Color32(255, 150, 150, 255);
            incorrectLoginCanvas.enabled = true;
            return;
        }
        // to get user_id, SELECT * FROM... then need to handle null
        // update last_login
        UserStats.Initialise(username, conn);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void backResetColor()
    {
        if (signUpInitialised)
        {
            usernameFieldSU.image.color = new Color32(255, 255, 255, 255);
            emailFieldSU.image.color = new Color32(255, 255, 255, 255);
            passwordFieldSU.image.color = new Color32(255, 255, 255, 255);
            // hide ui messsages
            invalidUserNameCanvas.enabled = false;
            invalidEmailCanvas.enabled = false;
            invalidPasswordCanvas.enabled = false;
        }
        if (logInInitialised)
        {
            usernameFieldLI.image.color = new Color32(255, 255, 255, 255);
            passwordFieldLI.image.color = new Color32(255, 255, 255, 255);
            // hide ui messsages
            incorrectLoginCanvas.enabled = false;
        }
    }

    public void SignUpInitialise()
    {
        var signUpMenu = GameObject.Find("Sign Up Menu");

        TMP_InputField[] signUpObjs = signUpMenu.GetComponentsInChildren<TMP_InputField>();
        usernameFieldSU = signUpObjs[0];
        emailFieldSU = signUpObjs[1];
        passwordFieldSU = signUpObjs[2];

        Canvas[] invalidObjs = signUpMenu.GetComponentsInChildren<Canvas>(true);
        foreach (Canvas canvas in invalidObjs)
        {
            canvas.enabled = false;
        }
        invalidUserNameCanvas = invalidObjs[0];
        invalidEmailCanvas = invalidObjs[1];
        invalidPasswordCanvas = invalidObjs[2];

        signUpInitialised = true;
    }

    public void LogInInitialise()
    {
        var logInMenu = GameObject.Find("Log In Menu");

        TMP_InputField[] logInObjs = logInMenu.GetComponentsInChildren<TMP_InputField>();
        usernameFieldLI = logInObjs[0];
        passwordFieldLI = logInObjs[1];

        Canvas[] incorrectObjs = logInMenu.GetComponentsInChildren<Canvas>(true);
        incorrectLoginCanvas = incorrectObjs[0];

        logInInitialised = true;
    }



    // https://forum.unity.com/threads/run-terminal-command-and-get-output-within-unity-application-osx.683164/#post-4574398
    private string GetConnectionString()
    {
        //string strCmdText;
        //strCmdText= "/C heroku config:get DATABASE_URL -a the-paramours-candour";
        string argument = "heroku config:get DATABASE_URL -a the-paramours-candour";
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "/bin/bash",
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                Arguments = " -c \"" + argument + " \""
            };
            Process myProcess = new Process
            {
                StartInfo = startInfo
            };
            myProcess.Start();
            string output = myProcess.StandardOutput.ReadToEnd();
            UnityEngine.Debug.Log(output);
            myProcess.WaitForExit();
            return output;
        }
        // gotta change this
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.ToString());
            return null;
        }
    }

    public string connectString = "Server=" + "ec2-54-90-211-192.compute-1.amazonaws.com" + ";"
    + "Database=" + "d78t5ejq1fca8p" + ";"
    + "User ID=" + "vejafmwwilcxts" + ";"
    + "Password=" + "ad9ee22908db8dd733b181e7a83ba0511f01fd99039309fb03a6912ce04e660d" + ";"
    + "SSL Mode=Require;Trust Server Certificate=true;";
    public static NpgsqlConnection conn;
    void Start()
    {
        // string output = GetConnectionString();
        // handle output null
        // my bb
        // Regex regex = new Regex("^(.*?)://(.*?):(.*?)@(.*?):(.*?)/(.*?)$");
        // Match match = regex.Match(output);
        // string server = match.Groups[4].Value;
        // string database = match.Groups[6].Value;
        // string user = match.Groups[2].Value;
        // string pw = match.Groups[3].Value;
        // string connectString = "Server="+server+";"
        //     + "Database="+database+";"
        //     + "User ID="+user+";"
        //     + "Password="+pw+";"
        //     + "SSL Mode=Require;Trust Server Certificate=true;";
        try
        {
            conn = new NpgsqlConnection(connectString);
            conn.Open();
            // UnityEngine.Debug.Log("noice");
        }
        // gotta change this
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.ToString());
        }
        finally
        {
            conn.Close();
        }
    }

    public static NpgsqlConnection GetConnection()
    {
        return conn;
    }
}