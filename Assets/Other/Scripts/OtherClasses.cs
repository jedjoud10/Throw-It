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
//Picks a random death message when a player dies
public static class RandomPlayerDeathMessages 
{
    private static string[] snowballDeathMessages = new string[4] 
    {
        "{0} got shrekt because {1} hit them",
        "It seems like {1} has destroyed {0}",
        "{0} has stopped existing because of {1}",
        "{0} : Get shrekt.  {1} : No u"
    };
    public static string RandomSnowballDeathMessage(string damagedPlayerName, string snowballOwner) 
    {
        //Pick a random death message
        string unformatted = snowballDeathMessages[UnityEngine.Random.Range(0, snowballDeathMessages.Length)];
        //Format it correctly
        return string.Format(unformatted, damagedPlayerName, snowballOwner);
    }
}