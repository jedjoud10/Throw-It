using MLAPI;
using MLAPI.NetworkedVar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script that is applied to items on the ground
public class ItemScript : NetworkedBehaviour
{
    public NetworkedVarInt itemID;//The item data associated with this item
    public Transform itemModelHolder;//The holder for the instantiated model
    // Start is called before the first frame update
    void Start()
    {
        //Init the item on the server and client
        UpdateItem();//Init item
    }

    public void UpdateItem()
    {
        if (itemID.Value != -1)
        {
            SetItemModel(ItemsManager.ID2Item(itemID.Value).itemModel);
        }
    }

    //Sets the model for this item
    public void SetItemModel(GameObject model) 
    {
        Instantiate(model, itemModelHolder, false);
        GetComponent<MeshCollider>().sharedMesh = model.GetComponent<MeshFilter>().sharedMesh;
    }
}
