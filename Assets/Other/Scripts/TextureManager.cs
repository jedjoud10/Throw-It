using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//Loads all the textures from the StreamingAssetsFolder
public static class TexturesManager
{
    //Loaded textures
    public static Dictionary<string, Texture2D> textures;
    //Load all the textures
    public static void LoadAllTextures() 
    {
        string path = Application.streamingAssetsPath + "/textures/";
        string[] texturePathes = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
        textures = new Dictionary<string, Texture2D>();
        for (int i = 0; i < texturePathes.Length; i++)
        {
            string itemIconName = Path.GetFileName(texturePathes[i]);
            Texture2D newTexture = new Texture2D(2, 2);
            newTexture.LoadImage(File.ReadAllBytes(texturePathes[i]));
            textures.Add(itemIconName, newTexture);
        }
        Debug.LogWarning("Loaded " + textures.Count + " textures !");
    }
    //Search the loaded textures and find the one with the corresponding name
    public static Texture2D LoadTexture(string id) 
    {
        if (textures.ContainsKey(id))
        {
            return textures[id];
        }
        else
        {
            return null;
        }
    }
}

