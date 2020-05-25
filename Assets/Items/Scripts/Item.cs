using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Class representing an base item
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Create new Item")]
public class Item : ScriptableObject
{
    public string itemName;//The name of the item
    public GameObject itemModel;//The model of this item
    public Texture itemIcon;//The icon for this specific item
    public string itemDescription;//The unique description for this item
}