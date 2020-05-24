using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Class representing an base item
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Create new Item")]
public class Item : ScriptableObject
{
    public string itemName;//The name of the item
    public GameObject itemModel;//The model of this item
    //This is so useless
    virtual public void DropItem() //Called when a player drops this item
    {
        
    }
    virtual public void PickupItem() //Called when a player collects this item
    {
    
    }
}