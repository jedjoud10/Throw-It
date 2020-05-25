using MLAPI;
using MLAPI.NetworkedVar.Collections;
using MLAPI.NetworkedVar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.UI;
//The whole inventory for this player
public class PlayerInventoryScript : NetworkedBehaviour
{
    public Transform cameraObject;//The camera of the player
    public GameObject itemGameObject;//The base item object
    private PlayerInventoryUIManagerScript inventoryUIManager;//Manages the UI for this inventory
    private PlayerControllerScript playerController;//Movement and rotation controller for this player
    private int equipedItem;//The current item the player is holding
    private NetworkedList<int> inventory = new NetworkedList<int>(new NetworkedVarSettings() { WritePermission = NetworkedVarPermission.OwnerOnly, ReadPermission = NetworkedVarPermission.Everyone });//What the player is currently holding in their inventory
    private PlayerHealthScript healthScript;//The health script of the player
    const int maxInventorySize = 10;//Maximum number of items that the player can hold
    private bool inventoryOpened;//If the UI for the inventory is visible
    private bool inventoryButton;//If the inventory toggle button is pressed right now
    // Start is called before the first frame update
    void Start()
    {
        if (IsLocalPlayer) 
        {
            healthScript = GetComponent<PlayerHealthScript>();
            inventoryUIManager = GetComponent<PlayerInventoryUIManagerScript>();
            playerController = GetComponent<PlayerControllerScript>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("PickupItem") > 0) 
        {
            RaycastHit hit;//Result of raycast
            if (Physics.Raycast(cameraObject.position, cameraObject.forward, out hit))
            {
                if(hit.transform.GetComponent<ItemScript>() != null) 
                {
                    PickupItem(hit.transform.GetComponent<ItemScript>());
                }
            }
        }
        if(Input.GetAxis("ToggleInventory") > 0) 
        {
            if(inventoryButton == false) 
            {
                inventoryButton = true;//Oh no, the heavy is DEAD
                inventoryOpened = !inventoryOpened;
                //Make the player uncrontolable if the inventory is opened
                playerController.controllable = !inventoryOpened;
                inventoryUIManager.ToggleInventory();
                UpdateUIInventory();
                if (inventoryOpened) 
                {
                    //Show the cursor
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    //Hide the cursor
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
        else
        {
            inventoryButton = false;
        }
    }
    //Pick up an item
    private bool PickupItem(ItemScript itemScript) 
    {
        if(itemScript != null && itemScript.itemID != -1) 
        {
            Destroy(itemScript.gameObject);
            return AddItem(itemScript.itemID);
        }
        else
        {
            return false;
        }
    }
    //Drops an item infront of the player
    private bool DropItem(int itemID) 
    {
        if (RemoveItem(itemID)) 
        {
            //Spawn a new base item into the world
            GameObject itemObject = Instantiate(itemGameObject, cameraObject.position + cameraObject.forward * 5, Quaternion.identity);
            //Set the item's model
            itemObject.GetComponent<ItemScript>().SetItemModel(ItemsHandler.ID2Item(itemID).itemModel);
            return true;
        }
        else
        {
            //Could not drop the item
            return false;
        }
    }
    //Removes an item from the inventory
    private bool RemoveItem(int itemID) 
    {
        if (inventory.Remove(itemID)) 
        {
            UpdateUIInventory();
            return true;
        }
        else
        {
            //Could not remove the item
            return false;
        }
    }
    //Removes an item at a specified index from the inventory
    private void RemoveItemAtIndex(int itemIndex) 
    {
        inventory.RemoveAt(itemIndex);
        UpdateUIInventory();
    }
    //Adds an item to the inventory
    private bool AddItem(int itemID)
    { 
        if(inventory.Count < maxInventorySize) 
        {
            inventory.Add(itemID);
            UpdateUIInventory();
            return true;
        }
        else
        {
            //Could not add the item because it exceeds the max inventory size
            return false;
        }
    }
    //Equips an item
    private void EquipItem(int itemID) 
    {
        if(!inventory.Contains(itemID)) 
        {
            //we dont have that item, so set the equiped item as null
            equipedItem = -1;
        }
        else
        {
            //Convert the inventory item into a equiped item
            inventoryUIManager.SelectItem(ItemsHandler.ID2Item(itemID));
            equipedItem = itemID;
        }
    }
    //Equip an item using a specific index
    public void EquipItemWithIndex(int itemIndex) 
    {
        if(itemIndex < inventory.Count && itemIndex != -1) 
        {
            EquipItem(inventory[itemIndex]);        
        }
        else
        {
            inventoryUIManager.SelectItem(null);
        }
    }
    //Un-equip an item
    private void UnequipItem() 
    {
        if (equipedItem != -1) 
        {
            //Convert the equiped item into an inventory item
            AddItem(equipedItem);
            equipedItem = -1;        
        }
    }
    //Consume a consumable item
    private bool ConsumeItem(int itemID) 
    {
        Item item = ItemsHandler.ID2Item(itemID);
        if(item is Consumable) 
        {
            //Add health to player
            Consumable consumable = (Consumable)item;
            if (healthScript.HealPlayer(consumable.healthRegeneration))
            {
                return RemoveItem(itemID);
            }
            else
            {
                return false;//Could not consume the item, health is already at max value
            }
        }
        else
        {
            //Item is not a consumable
            return false;
        }
    }
    //Update the inventory UI
    private void UpdateUIInventory() 
    {
        Texture[] textures = new Texture[maxInventorySize];
        for (int i = 0; i < textures.Length; i++)
        {
            if(i >= inventory.Count) 
            {
                //The inventory doesnt have the correct items, so set the textures as blank
                textures[i] = null;
                continue;
            }
            else
            {
                if (ItemsHandler.ID2Item(inventory[i]) != null)
                {
                    textures[i] = ItemsHandler.ID2Item(inventory[i]).itemIcon;
                }
                else
                {
                    //The itemID does not exist in the loaded items
                    textures[i] = null;
                    continue;
                }
            }
        }
        inventoryUIManager.SetItemIcons(textures);
    }
}
