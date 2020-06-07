using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Class representing an base item
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Create new Item")]
public class Item : ScriptableObject
{
    public string itemName;//The name of the item
    public GameObject itemModel;//The model of this item
    public string itemIconName;//The name for the icon of this specific item
    [HideInInspector]
    public Texture itemIcon = null;//The icon for this item
    public string itemDescription;//The unique description for this item
}