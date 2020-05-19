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

    //Using custom path
    public static object Load(string path, string file, object defaultValue, Type type)
    {
        if (!File.Exists(persistentDir + "/" + file))
        {
            //Uh ohhh...stinkyyyyy...stinkyyyy error...hahahaha   
            Debug.LogWarning("File : " + file + " does not exist !");
            Save(file, defaultValue);
            return defaultValue;
        }
        object obj = defaultValue;
        obj = JsonUtility.FromJson(File.ReadAllText(path + "/" + file), type);
        Save(file, obj);//Resave just in case file has missing data
        return obj;
    }
    public static void Save(string path, string file, object data)
    {
        string stringData = JsonUtility.ToJson(data, true);
        File.WriteAllText(path + "/" + file, stringData);
    }
    public static bool Exists(string path, string file) { return File.Exists(path + "/" + file); }//If a file exists
}
