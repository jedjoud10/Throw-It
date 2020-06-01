using UnityEngine;
using System.IO;
using System;

//Loads and saves data in files
public class SaverLoader
{
    static string persistentDir = Application.persistentDataPath;
    //Json files
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
    //Txt files
    public static string[] LoadTxtLines(string file)
    {
        return File.ReadAllLines(persistentDir + "/" + file);
    }
    public static void SaveTxtLines(string file, string[] txtLines)
    {
        File.WriteAllLines(persistentDir + "/" + file, txtLines);
    }
    public static bool Exists(string file) { return File.Exists(persistentDir + "/" + file); }//If a file exists

}