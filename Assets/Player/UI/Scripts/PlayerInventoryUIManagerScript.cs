using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//UI manager but only for the inventory
public class PlayerInventoryUIManagerScript : MonoBehaviour
{
    public GameObject draggedItemIconPrefab;//The gameobject that we will instantiate when dragging an item
    private DraggedItemIconScript draggedItemIconInstance;//The instance of the draggedItemIconPrefab
    public GameObject inventoryCanvas;//The whole inventory canvas
    public RawImage[] itemIcons;//The item icons
    public UIInventoryItemIconScript[] itemIconScripts;
    public TMP_Text itemName;//The name of the current equipped item
    public TMP_Text itemDescription;//Description of the current eqquiped item
    public TMP_Text itemCustomDescription;//Custom description if the item is a child item class
    public RawImage hotbar1, hotbar2, hotbar3, hotbar4;
    private PlayerInventoryScript inventoryScript;//The inventory handling
    // Start is called before the first frame update
    void Start()
    {
        inventoryScript = GetComponent<PlayerInventoryScript>();
        SetInventoryVisibility(false);
        //Init all the item icons
        for (int i = 0; i < itemIconScripts.Length; i++)
        {
            itemIconScripts[i].InitItemIcon();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    //Deselects the current
    public void HideItemData() 
    {
        itemName.text = ""; itemDescription.text = ""; itemCustomDescription.text = "";
    }
    //Shows the data for a specific item
    public void ShowItemData(Item item) 
    {        
        if(item == null) 
        {
            itemName.text = ""; itemDescription.text = ""; itemCustomDescription.text = "";
            return;
        }
        itemName.text = item.itemName;
        itemDescription.text = item.itemDescription;
        if(item is Throwable) 
        {
            string customDescription = "";
            Throwable throwable = (Throwable)item;
            //Custom description
            customDescription = "Type: " + throwable.type + '\n';
            customDescription += "Speed randomness: " + throwable.speedRandomness + '\n';
            customDescription += "Size randomness: " + throwable.sizeRandomness + '\n';
            customDescription += "Damage randomness: " + throwable.damageRandomness;
            itemCustomDescription.text = customDescription;
            return;
        }
        if (item is Consumable)
        {
            string customDescription = "";
            Consumable consumable = (Consumable)item;
            //Custom description
            customDescription = "Health regeneration: " + consumable.healthRegeneration + '\n';
            customDescription += "Temperature regeneration: " + consumable.temperatureRegeneration;
            itemCustomDescription.text = customDescription;
            return;
        }
    }


    //Toggle the inventory
    public void ToggleInventory()
    {
        inventoryCanvas.SetActive(!inventoryCanvas.activeSelf);
        if (inventoryCanvas.activeSelf) 
        {
            HideItemData();//yes.
            //Reset the items when we open the inventory
            for (int i = 0; i < itemIconScripts.Length; i++)
            {
                itemIconScripts[i].ResetItemIconSize();
            }
        }
    }
    //Close the inventory
    public void SetInventoryVisibility(bool visible) 
    {
        inventoryCanvas.SetActive(visible);
    }
    //Set the icons for the items
    public void SetItemIcons(Texture[] textures) 
    {
        for (int i = 0; i < itemIcons.Length; i++)
        {
            itemIcons[i].texture = textures[i];
            if(textures[i] == null) 
            {
                //Dont show the item icon if the item is non existant
                itemIcons[i].color = Color.clear;
            }
            else
            {
                //Reset the icon color
                itemIcons[i].color = Color.white;
            }
            itemIconScripts[i].ItemIconStopHover();
        }
    }
    //Set the hotbar item icons
    public void SetHotbarIcons(Texture a, Texture b, Texture c, Texture d) 
    {
        hotbar1.texture = a; hotbar2.texture = b; hotbar3.texture = c; hotbar4.texture = d;

        //If the texture of each item icon in the hot bar is null, then just hide the item icon
        if (a == null) { hotbar1.color = Color.clear; } else { hotbar1.color = Color.white; }
        if (b == null) { hotbar2.color = Color.clear; } else { hotbar2.color = Color.white; }
        if (c == null) { hotbar3.color = Color.clear; } else { hotbar3.color = Color.white; }
        if (d == null) { hotbar4.color = Color.clear; } else { hotbar4.color = Color.white; }
    }
    //Start dragging from a UIInventoryItemIconScript
    public void StartDrag(int index) 
    {
        if (inventoryScript.GetItemID(index) != -1) //We cant drag a null item
        {
            return;
        }

        draggedItemIconInstance = Instantiate(draggedItemIconPrefab).GetComponent<DraggedItemIconScript>();
        draggedItemIconInstance.itemID = inventoryScript.GetItemID(index);
    }
    //Stop dragging an item (Replace the current item data with the draggable gameObject data)
    public void StopDrag(int index)
    {
        if(inventoryScript.GetItemID(index) != -1) //We cant replace an already existing item
        {
            return;
        }

        inventoryScript.ReplaceItem(index, draggedItemIconInstance.itemID);        
        Destroy(draggedItemIconInstance.gameObject);
    }
}
