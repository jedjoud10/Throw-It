using MLAPI;
using MLAPI.NetworkedVar.Collections;
using MLAPI.NetworkedVar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//The whole inventory for this player
public class PlayerInventoryScript : NetworkedBehaviour
{
    public Transform cameraObject;//The camera of the player
    public GameObject itemGameObject;//The base item object
    private Item equipedItem;//The current item the player is holding
    private NetworkedList<Item> inventory = new NetworkedList<Item>(new NetworkedVarSettings() { WritePermission = NetworkedVarPermission.OwnerOnly, ReadPermission = NetworkedVarPermission.Everyone });//What the player is currently holding in their inventory
    const int maxInventorySize = 10;//Maximum number of items that the player can hold
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Drops an item infront of the player
    private bool DropItem(Item item) 
    {
        if (RemoveItem(item)) 
        {
            GameObject itemObject = Instantiate(itemGameObject, cameraObject.position + cameraObject.forward * 5, Quaternion.identity);
            return true;
        }
        else
        {
            return false;
        }
    }
    //Removes an item from the inventory
    private bool RemoveItem(Item item) 
    {
        return inventory.Remove(item);
    }
    //Adds an item to the inventory
    private bool AddItem(Item item)
    { 
        if(inventory.Count < maxInventorySize) 
        {
            inventory.Add(item);
            return true;
        }
        else
        {
            return false;
        }
    }
    //Equips an item
    private void EquipItem(Item item) 
    {
        if(!inventory.Contains(item)) 
        {
            //we dont have that item, so set the equiped item as null
            equipedItem = null;
        }
        else
        {
            //Convert the inventory item intto a equiped item
            equipedItem = item;
            RemoveItem(item);
        }
    }
    //Un-equip an item
    private void UnequipItem() 
    {
        if (equipedItem != null) 
        {
            //Convert the equiped item intto an inventory item
            AddItem(equipedItem);
            equipedItem = null;        
        }
    }
}
