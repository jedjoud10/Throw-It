using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Reflection;

//Loads all the items from the resources folder
public static class ItemsHandler
{
    private static Item[] items;//The loaded items
    public static GameObject itemBase;//The item base
    //Load all the items from the resource folder and store them into the variable
    public static void LoadAllItems()
    {
        itemBase = Resources.Load<GameObject>("ItemBase");
        items = Resources.LoadAll<Item>("Items");
        items = items.OrderBy(item => int.Parse(item.name.Split('_')[0])).ToArray();
        Debug.LogWarning("Loaded " + items.Length + " items !");
    }
    //Transform an itemID into an item
    public static Item ID2Item(int id)
    {
        if (id < items.Length && id != -1)
        {
            return items[id];
        }
        else
        {
            return null;
        }
    }
}
