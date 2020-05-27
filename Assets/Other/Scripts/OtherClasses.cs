using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
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
    private static string[] death_throwable_general = new string[0]
    {

    };
    private static string[] death_throwable_snowball = new string[3] 
    {
        "{0} lost their nose to {1}'s shot.",
        "{1} thoroughly memed {0}.",
        "{0}: I am dead!   {1}: Correct!"
    };
    private static string[] death_suicide = new string[2]
    {
        "{0} thought this was idiotic and went to hang themselves in style.",
        "What comes up, comes down on {0}."
    };
    private static string[] death_hypothermia = new string[2]
    {
        "{0} met a chilly end.",
        "{0} froze to oblivion."
    };
    private static string[] leftgame = new string[3] 
    {
        "{0} threw in the towel.",
        "{0} went to go play minecraft.",
        "{0} realized they left the oven on."
    };
    private static string[] joingame = new string[4]
    {
        "{0} dropped in. Welcome to the warzone.",
        "{0} joined. You're in Snow Man's Land.",
        "{0} popped into the fort. You'll be assigned to a station soon.",
        "Guess who came to the playground? {0}!"
    };
    private static string[] death_machinery_general = new string[1] 
    {
        "{1} was killed by THE AGE OF AUTOMATION."
    };
    #region Deaths
    public static string Death_Throwable_Snowball(string damagedPlayerName, string snowballOwner) 
    {
        //Pick a random death message
        string unformatted = death_throwable_snowball[UnityEngine.Random.Range(0, death_throwable_snowball.Length)];
        //Format it correctly
        return string.Format(unformatted, damagedPlayerName, snowballOwner);
    }
    public static string Death_Hypothermia(string damagedPlayerName)
    {
        //Pick a random death message
        string unformatted = death_hypothermia[UnityEngine.Random.Range(0, death_hypothermia.Length)];
        //Format it correctly
        return string.Format(unformatted, damagedPlayerName);
    }
    public static string Death_Suicide(string damagedPlayerName) 
    {
        //Pick a random death message
        string unformatted = death_suicide[UnityEngine.Random.Range(0, death_suicide.Length)];
        //Format it correctly
        return string.Format(unformatted, damagedPlayerName);
    }
    #endregion
    public static string Joingame(string playerName) 
    {
        //Pick a random join message
        string unformatted = joingame[UnityEngine.Random.Range(0, joingame.Length)];
        //Format it correctly
        return string.Format(unformatted, playerName);
    }
    public static string Leftgame(string playerName)
    {
        //Pick a random death message
        string unformatted = leftgame[UnityEngine.Random.Range(0, leftgame.Length)];
        //Format it correctly
        return string.Format(unformatted, playerName);
    }
}
//Loads all the items from the resources folder
public static class ItemsHandler 
{
    private static Item[] items;//The loaded items
    //Load all the items from the resource folder and store them into the variable
    public static void LoadAllItems() 
    {
        items = Resources.LoadAll<Item>("Items");
        items = items.OrderBy(item => int.Parse(item.name.Split('_')[0])).ToArray();
        Debug.LogWarning("Loaded " + items.Length + " items !");        
    }
    //Transform an itemID into an item
    public static Item ID2Item(int id) 
    {
        if(id < items.Length && id != -1) 
        {
            return items[id];
        }
        else
        {
            return null;
        }
    }
}