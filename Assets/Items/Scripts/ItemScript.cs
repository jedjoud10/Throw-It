using MLAPI;
using MLAPI.NetworkedVar;
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
        //Init the item
        if (itemID.Value != -1)
        {
            SetItemModel(ItemsHandler.ID2Model(ItemsHandler.ID2Item(itemID.Value).itemModelID));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Sets the model for this item
    public void SetItemModel(GameObject model) 
    {
        Instantiate(model, itemModelHolder, false);
        GetComponent<MeshCollider>().sharedMesh = model.GetComponent<MeshFilter>().sharedMesh;
    }
}
