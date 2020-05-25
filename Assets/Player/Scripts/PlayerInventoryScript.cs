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
    private PlayerInventoryUIManagerScript inventoryUIManager;//Manages the UI for this inventory
    private PlayerControllerScript playerController;//Movement and rotation controller for this player
    private int equipedItem = -1;//The current item the player is holding
    public int equipedItemIndex = 0;//The index of the currently equiped item in the inventory
    private NetworkedVar<int[]> inventory = new NetworkedVar<int[]>(new NetworkedVarSettings() { WritePermission = NetworkedVarPermission.OwnerOnly, ReadPermission = NetworkedVarPermission.Everyone });//What the player is currently holding in their inventory
    private PlayerHealthScript healthScript;//The health script of the player
    const int maxInventorySize = 10;//Maximum number of items that the player can hold
    private bool inventoryOpened;//If the UI for the inventory is visible
    private bool inventoryButton;//If the inventory toggle button is pressed right now
    private PlayerThrowableThrowingScript playerThrowingScript;//How the player is gonna throws stuff
    // Start is called before the first frame update
    void Start()
    {
        if (IsLocalPlayer) 
        {
            //Init components            
            healthScript = GetComponent<PlayerHealthScript>();
            inventoryUIManager = GetComponent<PlayerInventoryUIManagerScript>();
            playerThrowingScript = GetComponent<PlayerThrowableThrowingScript>();
            playerController = GetComponent<PlayerControllerScript>();

            inventory = new NetworkedVar<int[]>(new NetworkedVarSettings() { WritePermission = NetworkedVarPermission.OwnerOnly, ReadPermission = NetworkedVarPermission.Everyone }, new int[maxInventorySize]);
            for (int i = 0; i < maxInventorySize; i++)
            {
                inventory.Value[i] = -1;
            }
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
                    playerThrowingScript.canCharge = false;
                }
                else
                {
                    //Hide the cursor
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    playerThrowingScript.canCharge = true;
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
            if (AddItem(itemScript.itemID)) 
            {
                //If item was succsessfully added, then remove the gameobject
                Destroy(itemScript.gameObject);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    //Drops an item infront of the player
    private bool DropItem(int itemIndex) 
    {
        if (RemoveItemAtIndex(itemIndex)) 
        {
            //Spawn a new base item into the world
            GameObject itemObject = Instantiate(itemGameObject, cameraObject.position + cameraObject.forward * 5, Quaternion.identity);
            //Set the item's model
            itemObject.GetComponent<ItemScript>().SetItemModel(ItemsHandler.ID2Item(inventory.Value[itemIndex]).itemModel);
            return true;
        }
        else
        {
            //Could not drop the item
            return false;
        }
    }
    //Removes an item from the inventory
    public bool RemoveItem(int itemID) 
    {
        for (int i = 0; i < maxInventorySize; i++)
        {
            if (inventory.Value[i] == itemID) 
            {
                if (equipedItem == itemID)
                {
                    UnequipItem();
                }
                inventory.Value[i] = -1;
                inventoryUIManager.Deselect();
                UpdateUIInventory();
                return true;
            }
            else
            {
                //Could not remove the item in this iteration
            }
        }
        return false;
    }
    //Removes an item from the inventory using a specificed index
    public bool RemoveItemAtIndex(int index) 
    {
        if (inventory.Value[index] != -1)
        {
            if (equipedItem == inventory.Value[index])
            {
                UnequipItem();
            }
            inventory.Value[index] = -1;
            inventoryUIManager.Deselect();
            UpdateUIInventory();
            return true;
        }
        else
        {
            //Could not remove the item
        }        
        return false;
    }
    //Adds an item to the inventory
    private bool AddItem(int itemID)
    {
        bool changedInventory = false;
        for (int i = 0; i < maxInventorySize; i++)
        {
            if(inventory.Value[i] == -1) 
            {
                inventory.Value[i] = itemID;
                if(equipedItemIndex == i) 
                {
                    EquipItem(i);
                }
                changedInventory = true;
                break;
            }
        }
        if(changedInventory) UpdateUIInventory();
        return changedInventory;        
    }
    //Equips an item
    private void EquipItem(int itemIndex) 
    {
        if(inventory.Value[itemIndex] != -1) 
        {
            //Convert the inventory item into a equiped item
            Item newlyEquipedItem = ItemsHandler.ID2Item(inventory.Value[itemIndex]);
            inventoryUIManager.SelectItem(newlyEquipedItem);
            equipedItem = inventory.Value[itemIndex];
            equipedItemIndex = itemIndex;
            if(newlyEquipedItem is Throwable) 
            {
                //If the item is throwable, then set it as the current throwable item
                playerThrowingScript.selectedThrowableID = inventory.Value[itemIndex];
            }
            else
            {
                //Item is not a throwable, disable throwing
                playerThrowingScript.selectedThrowableID = -1;
            }
        }
        else
        {
            //we dont have that item, so set the equiped item as null
            equipedItem = -1;
        }
    }
    //Equip an item using a specific index
    public void EquipItemWithIndex(int itemIndex) 
    {
        if(itemIndex < inventory.Value.Length && itemIndex != -1) 
        {
            EquipItem(itemIndex);        
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
            if (ItemsHandler.ID2Item(equipedItem) is Throwable) 
            {
                //We unequiped a throwable, so make the player unable to throw
                playerThrowingScript.selectedThrowableID = -1;
            }
            equipedItem = -1;        
        }
    }
    //Consume a consumable item
    private bool ConsumeItem(int itemIndex) 
    {
        Item item = ItemsHandler.ID2Item(inventory.Value[itemIndex]);
        if(item is Consumable) 
        {
            //Add health to player
            Consumable consumable = (Consumable)item;
            if (healthScript.HealPlayer(consumable.healthRegeneration))
            {
                return RemoveItemAtIndex(itemIndex);
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
            if(inventory.Value[i] == -1) 
            {
                //The inventory doesnt have the correct items, so set the textures as blank
                textures[i] = null;
                continue;
            }
            else
            {
                if (ItemsHandler.ID2Item(inventory.Value[i]) != null)
                {
                    textures[i] = ItemsHandler.ID2Item(inventory.Value[i]).itemIcon;
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
