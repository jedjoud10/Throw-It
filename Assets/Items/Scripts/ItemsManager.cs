using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Reflection;

//Loads all the items from the resources folder
public static class ItemsManager
{
    public static Dictionary<string, Item> items;//The loaded items
    public static GameObject itemBase;//The item base
    //Load all the items from the resource folder and store them into the variable
    public static void LoadAllItems()
    {
        itemBase = Resources.Load<GameObject>("ItemBase");
        Item[] preItems = Resources.LoadAll<Item>("Items");
        items = new Dictionary<string, Item>();
        foreach (var item in preItems)
        {
            Item editedItem = item;
            Texture2D itemIcon = TexturesManager.LoadTexture(editedItem.itemIconName);
            if (itemIcon != null) 
            {
                Texture2D newTexture = new Texture2D(itemIcon.width, itemIcon.height, itemIcon.format, false);
                newTexture.LoadImage(itemIcon.EncodeToPNG());
                editedItem.itemIcon = newTexture;            
            }
            else
            {
                editedItem.itemIcon = TexturesManager.LoadTexture("item_default.png");
            }
            items.Add(item.name, editedItem);//Default
            SystemLogger.LogNewMessage("Loaded item: " + item.name);
        }
        Debug.LogWarning("Loaded " + items.Count + " items !");
    }
    //Transform an itemID into an item
    public static Item ID2Item(string id)
    {
        if (items.ContainsKey(id))
        {
            return items[id];
        }
        else
        {
            return null;
        }
    }
    //Gets a random item
    public static string RandomItemID()
    {
        return items.ElementAt(UnityEngine.Random.Range(0, items.Count)).Key;
    }
}
