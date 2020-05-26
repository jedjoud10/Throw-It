using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Class representing an base item
[System.Serializable]
public class Item
{
    public string itemName;//The name of the item
    public int itemModelID;//The model of this item using an ID
    public int itemIconID;//The icon id for this specific item
    public string itemDescription;//The unique description for this item
}