using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A script that spawns items
public class ItemSpawnerScript : NetworkedBehaviour
{
    public string itemID;//The item's ID to spawn
    public bool randomized;//Should the item ID be randomized at start ?
    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {            
            GameObject itemObject = Instantiate(ItemsManager.itemBase, transform.position, transform.rotation);
            //Set the item's model
            itemObject.GetComponent<ItemScript>().itemID.Value = randomized ? ItemsManager.RandomItemID() : itemID;
            itemObject.GetComponent<NetworkedObject>().Spawn();
        }
    }
}


