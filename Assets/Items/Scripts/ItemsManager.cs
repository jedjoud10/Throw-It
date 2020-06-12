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
    public static Item[] items;//The loaded items
    public static GameObject itemBase;//The item base
    //Load all the items from the resource folder and store them into the variable
    public static void LoadAllItems()
    {
        itemBase = Resources.Load<GameObject>("ItemBase");
        items = Resources.LoadAll<Item>("Items");
        items = items.OrderBy(item => int.Parse(item.name.Split('_')[0])).ToArray();
        for (int i = 0; i < items.Length; i++)
        {
            Texture2D itemIcon = TexturesManager.LoadTexture(items[i].itemIconName);
            if(itemIcon != null) 
            {
                Texture2D newTexture = new Texture2D(itemIcon.width, itemIcon.height, itemIcon.format, false);
                newTexture.LoadImage(itemIcon.EncodeToPNG());
                items[i].itemIcon = newTexture;            
            }
            else
            {
                items[i].itemIcon = TexturesManager.LoadTexture("item_default.png");
            }
        }
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
