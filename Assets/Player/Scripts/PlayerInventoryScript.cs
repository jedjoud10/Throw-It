using MLAPI;
using MLAPI.NetworkedVar.Collections;
using MLAPI.NetworkedVar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.Messaging;
//The whole inventory for this player
//TODO: Networking support
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
    const int maxInventorySize = 16;//Maximum number of items that the player can hold
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
        if (!IsLocalPlayer) 
        {
            return;
        }
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
                if (inventoryOpened) 
                {
                    //Show the cursor
                    UpdateUIInventory();
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    playerThrowingScript.canCharge.Value = false;
                }
                else
                {
                    //Hide the cursor
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    playerThrowingScript.canCharge.Value = true;
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
                InvokeServerRpc(DestroyItemOnServer, OwnerClientId, itemScript.gameObject);
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
    //Destroys a gameobject on the server then on the clients (except the owner)
    [ServerRPC]
    private void DestroyItemOnServer(ulong clientID, GameObject _gameObject) { InvokeClientRpcOnEveryoneExcept(DestroyItemOnClient, clientID, _gameObject);  }
    //Destroy a gameobject on the clients
    [ClientRPC]
    private void DestroyItemOnClient(GameObject _gameObject) 
    {
        Destroy(_gameObject);
    }
    //Drops an item infront of the player
    private bool DropItem(int itemIndex) 
    {
        if (RemoveItemAtIndex(itemIndex)) 
        {
            //Spawn a new base item into the world
            Vector3 spawnPosition = cameraObject.position + cameraObject.forward * 5;
            Quaternion spawnRotation = Quaternion.identity;
            GameObject itemObject = Instantiate(itemGameObject, spawnPosition, spawnRotation);
            InvokeServerRpc(SpawnItemOnServer, OwnerClientId, inventory.Value[itemIndex], spawnPosition, spawnRotation);
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
    //Spawn an item on the server then on the clients (except the owner)
    [ServerRPC]
    private void SpawnItemOnServer(ulong clientID, int itemID, Vector3 position, Quaternion rotation) 
    {
        InvokeClientRpcOnEveryoneExcept(SpawnItemOnClient, clientID, itemID, position, rotation);
    }
    //Spawn an item on the clients
    [ClientRPC]
    private void SpawnItemOnClient(int itemID, Vector3 position, Quaternion rotation) 
    {
        GameObject itemObject = Instantiate(itemGameObject, position, rotation);
        //Set the item's model
        itemObject.GetComponent<ItemScript>().SetItemModel(ItemsHandler.ID2Item(inventory.Value[itemID]).itemModel);
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
                inventoryUIManager.HideItemData();//Hide the item data when removing an item
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
    public bool RemoveItemAtIndex(int itemIndex) 
    {
        if (inventory.Value[itemIndex] != -1)
        {
            if (equipedItem == inventory.Value[itemIndex])
            {
                UnequipItem();
            }
            inventory.Value[itemIndex] = -1;
            inventoryUIManager.HideItemData();//Hide the item data when removing an item
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
    //Replace an item (using its index) with a new itemID
    public void ReplaceItem(int itemIndex, int newItemID) 
    {
        inventory.Value[itemIndex] = newItemID;
    }
    //Equips an item using a specific index
    public void EquipItem(int itemIndex) 
    {
        if(itemIndex != -1) 
        {
            //Convert the inventory item into a equiped item
            Item newlyEquipedItem = ItemsHandler.ID2Item(inventory.Value[itemIndex]);
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
            equipedItem = -1;
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
    //Gets an itemID from the inventory with an index
    public int GetItemID(int itemIndex) 
    {
        return inventory.Value[itemIndex];
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
        inventoryUIManager.SetHotbarIcons(textures[0], textures[1], textures[2], textures[3]);
    }
}
