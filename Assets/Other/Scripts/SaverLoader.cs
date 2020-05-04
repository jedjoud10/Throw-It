using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
//Loads and saves contents in files
public class SaverLoader
{
    static string datadir = Application.persistentDataPath;
    //Load
    public static string Load(string file) 
    {
        if (!File.Exists(file)) Debug.LogError("File : " + file + " does not exist !");//Uh ohhh...stinkyyyyy...stinkyyyy error...hahahaha
        string data = File.ReadAllText(datadir + "/" + file);
        return data;
    }
    //Save
    public static void Save(string filepath, string data) 
    {
        File.WriteAllText(datadir + "/" + filepath + ".json", data);
    }
}
