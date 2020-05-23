using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
//Loads and saves data in files
public class SaverLoader
{
    static string persistentDir = Application.persistentDataPath;
    public static object Load(string file, object defaultValue, Type type) 
    {
        if (!File.Exists(persistentDir + "/" + file)) 
        {
            //Uh ohhh...stinkyyyyy...stinkyyyy error...hahahaha   
            Debug.LogWarning("File : " + file + " does not exist !");
            Save(file, defaultValue);
            return defaultValue;
        }
        object obj = defaultValue;
        obj = JsonUtility.FromJson(File.ReadAllText(persistentDir + "/" + file), type);
        Save(file, obj);//Resave just in case file has missing data
        return obj;
    }
    public static void Save(string file, object data) 
    {
        string stringData = JsonUtility.ToJson(data, true);
        File.WriteAllText(persistentDir + "/" + file, stringData);
    }
    public static bool Exists(string file) { return File.Exists(persistentDir + "/" + file); }//If a file exists

}
public static class UnityExtensionMethods
{
    //https://answers.unity.com/questions/182209/checking-for-quaternion-values-to-not-be-nan.html
    /// <summary>
    /// Determines whether the quaternion is safe for interpolation or use with transform.rotation.
    /// </summary>
    /// <returns><c>false</c> if using the quaternion in Quaternion.Lerp() will result in an error (eg. NaN values or zero-length quaternion).</returns>
    /// <param name="quaternion">Quaternion.</param>
    public static bool IsValid(this Quaternion quaternion)
    {
        bool isNaN = float.IsNaN(quaternion.x + quaternion.y + quaternion.z + quaternion.w);

        bool isZero = quaternion.x == 0 && quaternion.y == 0 && quaternion.z == 0 && quaternion.w == 0;

        return !(isNaN || isZero);
    }
}
//Picks a random message when something happens to a player
public static class RandomPlayerMessages 
{
    //Data
    private static string[] snowballDeathMessages = new string[2] 
    {
        "{0} lost their nose to {1}'s shot.",
        "{1} thoroughly memed {0}."
    };
    private static string[] hypothermiaDeathMessages = new string[2]
    {
        "{0} met a chilly end.",
        "{0} froze to oblivion."
    };
    private static string[] leftgameDeathMessages = new string[3] 
    {
        "{0} threw in the towel.",
        "{0} went to go play minecraft.",
        "{0} realized they left the oven on."
    };
    private static string[] joingameMessages = new string[4]
    {
        "{0} dropped in. Welcome to the warzone.",
        "{0} joined. You're in Snow Man's Land.",
        "{0} popped into the fort. You'll be assigned to a station soon.",
        "Guess who came to the playground? {0}!"
    };
    private static string[] machineyDeathMessages = new string[1] 
    {
        "{1} was killed by THE AGE OF AUTOMATION."
    };
    #region Deaths
    public static string SnowballDeathMessage(string damagedPlayerName, string snowballOwner) 
    {
        //Pick a random death message
        string unformatted = snowballDeathMessages[UnityEngine.Random.Range(0, snowballDeathMessages.Length)];
        //Format it correctly
        return string.Format(unformatted, damagedPlayerName, snowballOwner);
    }
    public static string HypothermiaDeathMessage(string damagedPlayerName)
    {
        //Pick a random death message
        string unformatted = hypothermiaDeathMessages[UnityEngine.Random.Range(0, hypothermiaDeathMessages.Length)];
        //Format it correctly
        return string.Format(unformatted, damagedPlayerName);
    }
    public static string LeftgameDeathMessage(string damagedPlayerName)
    {
        //Pick a random death message
        string unformatted = leftgameDeathMessages[UnityEngine.Random.Range(0, leftgameDeathMessages.Length)];
        //Format it correctly
        return string.Format(unformatted, damagedPlayerName);
    }
    #endregion
    public static string JoingameMessage(string playerName) 
    {
        //Pick a random join message
        string unformatted = joingameMessages[UnityEngine.Random.Range(0, joingameMessages.Length)];
        //Format it correctly
        return string.Format(unformatted, playerName);
    }
}