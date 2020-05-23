using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Class representing an base item
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Create new Item")]
public class Item : ScriptableObject
{
    //The type of this item
    public ItemType itemType 
    { 
        get 
        { 
            return itemType;
        } 
        protected set 
        {
            itemType = value;
        }
    }
    public string itemName;//The name of the item
    public enum ItemType 
    {
        General, Throwable, Consumable
    }
    virtual public void InitItem() 
    {
        
    }
}