using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A script that spawns items
public class ItemSpawnerScript : NetworkedBehaviour
{
    public int itemID;//The item's ID to spawn
    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {            
            GameObject itemObject = Instantiate(ItemsHandler.itemBase, transform.position, transform.rotation);
            //Set the item's model
            itemObject.GetComponent<ItemScript>().itemID = itemID;
            itemObject.GetComponent<NetworkedObject>().Spawn();
            InvokeClientRpcOnEveryone(SpawnItemOnClient, itemID, itemObject);
        }
    }
    //Spawn an item on the clients
    [ClientRPC]
    private void SpawnItemOnClient(int itemID, GameObject itemObject)
    {
        itemObject.GetComponent<ItemScript>().itemID = itemID;
    }
}


