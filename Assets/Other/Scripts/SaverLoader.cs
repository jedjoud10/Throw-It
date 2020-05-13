using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
//Loads and saves data in files
public class SaverLoader
{
    static string datadir = Application.persistentDataPath;
    public static object Load(string file, object defaultValue, Type type) 
    {
        if (!File.Exists(datadir + "/" + file)) 
        {
            //Uh ohhh...stinkyyyyy...stinkyyyy error...hahahaha   
            Debug.LogWarning("File : " + file + " does not exist !");
            Save(file, defaultValue);
            return defaultValue;
        }
        object obj = defaultValue;
        obj = JsonUtility.FromJson(File.ReadAllText(datadir + "/" + file), type);
        Save(file, obj);//Resave just in case file has missing data
        return obj;
    }
    public static void Save(string file, object data) 
    {
        string stringData = JsonUtility.ToJson(data, true);
        File.WriteAllText(datadir + "/" + file, stringData);
    }
}
