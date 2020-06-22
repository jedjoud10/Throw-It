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
    private PlayerInventoryUIManagerScript inventoryUIManager;//Manages the UI for this inventory
    private PlayerControllerScript playerController;//Movement and rotation controller for this player
    private string equipedItem = "";//The current item the player is holding
    public int equipedItemIndex = 0;//The index of the currently equiped item in the inventory
    private NetworkedVar<string[]> inventory = new NetworkedVar<string[]>(new NetworkedVarSettings() { WritePermission = NetworkedVarPermission.OwnerOnly, ReadPermission = NetworkedVarPermission.Everyone });//What the player is currently holding in their inventory
    private PlayerHealthScript healthScript;//The health script of the player
    const int maxInventorySize = 16;//Maximum number of items that the player can hold
    private bool inventoryOpened;//If the UI for the inventory is visible
    private float mouseScrollWheelCounter = 0;//How much the user scrolled the wheel on their mouse
    const float mouseScrollWheelSensivity = 10;//Well yes

    //Item activation
    private PlayerThrowableThrowingScript playerThrowingScript;//How the player is gonna throws stuff
    // Start is called before the first frame update
    void Start()
    {
        if (IsLocalPlayer) 
        {
            inventory = new NetworkedVar<string[]>(new NetworkedVarSettings() { WritePermission = NetworkedVarPermission.OwnerOnly, ReadPermission = NetworkedVarPermission.Everyone }, new string[maxInventorySize]);
            for (int i = 0; i < maxInventorySize; i++)
            {
                inventory.Value[i] = "";
            }
            //Init components            
            healthScript = GetComponent<PlayerHealthScript>();

            playerThrowingScript = GetComponent<PlayerThrowableThrowingScript>();
            playerController = GetComponent<PlayerControllerScript>();

            //UI
            inventoryUIManager = GetComponent<PlayerInventoryUIManagerScript>();
            UpdateUIInventory();//Init ui
            inventoryUIManager.SetEquipedItemIndex(equipedItemIndex);//Init ui
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer) {  return;  }//Only on local player machine
        if (InputManager.GetKeyPress("PickupItem")) 
        {
            RaycastHit hit;//Result of raycast
            if (Physics.Raycast(cameraObject.position, cameraObject.forward, out hit, 3.2f))//Max distance is 6 units
            {
                if(hit.transform.GetComponent<ItemScript>() != null) 
                {
                    PickupItem(hit.transform.GetComponent<ItemScript>());
                }
            }
        }
        if (InputManager.GetKeyPress("DropItem"))
        {
            DropItem(equipedItemIndex);
        }
        //Show/hide inventory
        if (InputManager.GetKeyPress("ToggleInventory"))
        {
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
            }
            else
            {
                //Hide the cursor
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }       

        //Mouse scroll wheel input
        float newMouseScrollWheelCounter = mouseScrollWheelCounter + (Input.GetAxis("Mouse ScrollWheel") * -mouseScrollWheelSensivity);
        if (newMouseScrollWheelCounter != mouseScrollWheelCounter)//Detect change
        {
            mouseScrollWheelCounter = newMouseScrollWheelCounter;
            EquipItem(Mathf.RoundToInt(nfmod(mouseScrollWheelCounter, 4)));
            if(Mathf.RoundToInt(nfmod(mouseScrollWheelCounter, 4)) == 4) 
            {
                EquipItem(0);//bro cringe
            }
        }

        //"Activate" currently equiped item only when the inventory is closed
        if (InputManager.GetKey("ActivateEquipedItem") && !inventoryOpened)
        {
            Item currentItem = ItemsManager.ID2Item(equipedItem);
            if (currentItem is Throwable)
            {
                //Throw this item since its a throwable
                playerThrowingScript.StartChargingThrowable();
            }
            if (currentItem is Consumable)
            {
                //Mmmm yes consume
                ConsumeItem(equipedItemIndex);
            }
        }
        else
        {
            Item currentItem = ItemsManager.ID2Item(equipedItem);
            if (currentItem is Throwable)
            {
                playerThrowingScript.StopChargingThrowable();
            }
        }
    }
    //Modulo thingy thing from https://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
    float nfmod(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }
    #region GameObject management
    //Pick up an item
    private bool PickupItem(ItemScript itemScript) 
    {
        if(itemScript != null && itemScript.itemID.Value != "") 
        {
            if (AddItem(itemScript.itemID.Value)) 
            {
                //If item was succsessfully added, then remove the gameobject
                InvokeServerRpc(DestroyItemOnServer, itemScript.gameObject);
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
    //Destroys a gameobject on the server then on the clients
    [ServerRPC]
    private void DestroyItemOnServer(GameObject _gameObject) 
    {
        Destroy(_gameObject);
    }
    //Drops an item infront of the player
    private bool DropItem(int itemIndex) 
    {
        string itemID = inventory.Value[itemIndex];//Saving the item id since it will be destroyed in the inventory array
        if (RemoveItemAtIndex(itemIndex)) 
        {
            //Spawn a new base item into the world
            RaycastHit hit;//Result of raycast
            Vector3 spawnPosition = cameraObject.position + cameraObject.forward * 5;//Default value
            if (Physics.Raycast(cameraObject.position, cameraObject.forward, out hit, 5f))
            {
                spawnPosition = hit.point;
            }
            Quaternion spawnRotation = Quaternion.identity;
            InvokeServerRpc(SpawnItemOnServer, itemID, spawnPosition, spawnRotation);
            return true;
        }
        else
        {
            //Could not drop the item
            return false;
        }
    }
    //Spawn an item on the server
    [ServerRPC]
    private void SpawnItemOnServer(string itemID, Vector3 position, Quaternion rotation) 
    {
        GameObject itemObject = Instantiate(ItemsManager.itemBase, position, rotation);
        //Set the item's model
        itemObject.GetComponent<ItemScript>().itemID.Value = itemID;
        itemObject.GetComponent<NetworkedObject>().Spawn();
    }
    #endregion
    #region Item handling
    //Removes an item from the inventory
    public bool RemoveItem(string itemID) 
    {
        for (int i = 0; i < maxInventorySize; i++)
        {
            if (inventory.Value[i] == itemID) 
            {
                if (equipedItem == itemID)
                {
                    UnequipItem();
                }
                inventory.Value[i] = "";
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
        if (inventory.Value[itemIndex] != "")
        {
            if (equipedItem == inventory.Value[itemIndex])
            {
                UnequipItem();
            }
            inventory.Value[itemIndex] = "";
            inventoryUIManager.HideItemData();//Hide the item data when removing an item
            UpdateUIInventory();
            return true;
        }
        else
        {
            //Could not remove the item
            return false;
        }
    }
    //Adds an item to the inventory
    private bool AddItem(string itemID)
    {
        bool changedInventory = false;
        for (int i = 0; i < maxInventorySize; i++)
        {
            if(inventory.Value[i] == "") 
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
    //Swap two items using their itemIndexes
    public void SwapItems(int itemIndex, int itemIndex2) 
    {
        string oldItemID = inventory.Value[itemIndex];
        inventory.Value[itemIndex] = inventory.Value[itemIndex2];
        inventory.Value[itemIndex2] = oldItemID;
        UpdateUIInventory();
    }
    //Equips an item using a specific index
    public void EquipItem(int itemIndex) 
    {
        if(itemIndex != -1) 
        {
            //Convert the inventory item into a equiped item
            Item newlyEquipedItem = ItemsManager.ID2Item(inventory.Value[itemIndex]);
            equipedItem = inventory.Value[itemIndex];
            equipedItemIndex = itemIndex;

            //Set the new item for the item activators
            if(newlyEquipedItem is Throwable) 
            {
                //If the item is throwable, then set it as the current throwable item
                playerThrowingScript.selectedThrowableID = inventory.Value[itemIndex];
            }
            else { playerThrowingScript.selectedThrowableID = ""; }//Item is not a throwable, disable throwing
            inventoryUIManager.SetEquipedItemIndex(equipedItemIndex);//Update ui
        }
        else
        {
            equipedItem = "";
        }
    }
    //Un-equip an item
    private void UnequipItem() 
    {
        if (equipedItem != "") 
        {
            //Convert the equiped item into an inventory item
            if (ItemsManager.ID2Item(equipedItem) is Throwable) 
            {
                //We unequiped a throwable, so make the player unable to throw
                playerThrowingScript.selectedThrowableID = "";
            }
            equipedItem = "";        
        }
    }
    //Consume a consumable item
    private bool ConsumeItem(int itemIndex) 
    {
        Debug.Log("Consuming item at index: " + itemIndex);
        Item item = ItemsManager.ID2Item(inventory.Value[itemIndex]);
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
    //Gets an itemID from the inventory with an index
    public string GetItemID(int itemIndex) 
    {
        return inventory.Value[itemIndex];
    }
    #endregion
    //Update the inventory UI
    private void UpdateUIInventory() 
    {
        EquipItem(equipedItemIndex);//Re equip the current equiped item in case we changed something

        Texture[] textures = new Texture[maxInventorySize];
        for (int i = 0; i < textures.Length; i++)
        {
            if(inventory.Value[i] == "") 
            {
                //The inventory doesnt have the correct items, so set the textures as blank
                textures[i] = null;
                continue;
            }
            else
            {
                if (ItemsManager.ID2Item(inventory.Value[i]) != null)
                {
                    textures[i] = ItemsManager.ID2Item(inventory.Value[i]).itemIcon;
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
