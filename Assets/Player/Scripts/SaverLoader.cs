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
        string data = File.ReadAllText(datadir + file);
        return data;
    }
    //Save
    public static void Save(string file, string data) 
    {
        if(!Directory.Exists(datadir)) 
        {
            Directory.CreateDirectory(datadir);//Create dir
        }
        File.WriteAllText(datadir + file + ".json", data);
    }
}
//Class to make arrays to json. Found on StackOverflow
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
