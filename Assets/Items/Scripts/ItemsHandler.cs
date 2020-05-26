using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//Loads all the items from the resources folder
public class ItemsHandler
{
    public string test = "";
    private static Item[] loadedItems;//The items that have been loaded from JSON files
    private static Texture2D[] loadedItemIcons;//The loaded textures for the items
    private static GameObject[] loadedPrefabs;//The loaded prefabs from the resource folder
    private static GameObject[] loadedModels;//The loaded models from the resource folder
    //Initialize the ItemsHandler
    public static void Init() 
    {
        LoadAllPrefabs();
        LoadAllModels();
        LoadAllItemIcons();
        LoadAllItems();
        Debug.Log("Finished loading data !");
    }
    //Load all the items from the streamingAssets folder and store them
    public static void LoadAllItems()
    {
        string itemsDataPath = Application.streamingAssetsPath + "/Items/";
        string[] itemPathes = Directory.GetFiles(itemsDataPath, "*.json");        
        loadedItems = new Item[itemPathes.Length];
        for (int i = 0; i < itemPathes.Length; i++)
        {
            //There is definitely a better way im sure
            string itemName = Path.GetFileName(itemPathes[i]);
            string itemType = itemName.Split('_')[0];//Get the "type" of the item from its name
            Item loadedItem = JsonUtility.FromJson<Item>(File.ReadAllText(itemPathes[i]));
            if (itemType == "throwable") { loadedItem = JsonUtility.FromJson<Throwable>(File.ReadAllText(itemPathes[i])); }
            if (itemType == "consumable") { loadedItem = JsonUtility.FromJson<Consumable>(File.ReadAllText(itemPathes[i])); }
            loadedItems[i] = loadedItem;
        }
        Debug.LogWarning("Loaded " + loadedItems.Length + " items !");
    }
    //Load all the prefabs from the resource folder and store them
    public static void LoadAllPrefabs() 
    {
        loadedPrefabs = Resources.LoadAll<GameObject>("Prefabs");
        Debug.LogWarning("Loaded " + loadedPrefabs.Length + " prefabs !");
    }
    //Load all the models from the resource folder and store them
    public static void LoadAllModels()
    {
        loadedModels = Resources.LoadAll<GameObject>("Models");
        Debug.LogWarning("Loaded " + loadedModels.Length + " models !");
    }
    //Load all the textures from the streamingAssets folder and store them
    public static void LoadAllItemIcons() 
    {
        string itemIconsDataPath = Application.streamingAssetsPath + "/ItemIcons/";
        string[] itemIconPathes = Directory.GetFiles(itemIconsDataPath, "*.png");
        loadedItemIcons = new Texture2D[itemIconPathes.Length];
        for (int i = 0; i < itemIconPathes.Length; i++)
        {
            string itemIconName = Path.GetFileName(itemIconPathes[i]);
            loadedItemIcons[i] = new Texture2D(2, 2);
            loadedItemIcons[i].LoadImage(File.ReadAllBytes(itemIconPathes[i]));
        }
        Debug.LogWarning("Loaded " + loadedItemIcons.Length + " item icons !");
    }
    //Transform an itemID into an item
    public static Item ID2Item(int id)
    {
        if (id < loadedItems.Length && id != -1)
        {
            return loadedItems[id];
        }
        else
        {
            return null;
        }
    }
    //Transform a modelID into a prefab
    public static GameObject ID2Prefab(int id) 
    {
        if (id < loadedPrefabs.Length && id != -1)
        {
            return loadedPrefabs[id];
        }
        else
        {
            return null;
        }
    }
    //Transform a prefabID into a model
    public static GameObject ID2Model(int id)
    {
        if (id < loadedModels.Length && id != -1)
        {
            return loadedModels[id];
        }
        else
        {
            return null;
        }
    }
    //Transform a itemiconID into a texture2d
    public static Texture2D ID2ItemIcon(int id) 
    {
        if (id < loadedItemIcons.Length && id != -1)
        {
            return loadedItemIcons[id];
        }
        else
        {
            return null;
        }
    }
}