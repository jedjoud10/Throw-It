using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//UI manager but only for the inventory
public class PlayerInventoryUIManagerScript : MonoBehaviour
{
    public GameObject inventoryCanvas;//The whole inventory canvas
    public RawImage[] itemIcons;//The item icons
    public UIInventoryItemIconScript[] itemScripts;
    public Text itemName;//The name of the current equipped item
    public Text itemDescription;//Description of the current eqquiped item
    public Text itemCustomDescription;//Custom description if the item is a child item class
    // Start is called before the first frame update
    void Start()
    {
        HideInventory();
    }

    // Update is called once per frame
    void Update()
    {

    }
    //Deselects the current
    public void Deselect() 
    {
        itemName.text = ""; itemDescription.text = ""; itemCustomDescription.text = "";
    }
    //Select a specific item
    public void SelectItem(Item item) 
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
    //Open the inventory
    public void ShowInventory() 
    {
        inventoryCanvas.SetActive(true);
    }
    //Toggle the inventory
    public void ToggleInventory()
    {
        inventoryCanvas.SetActive(!inventoryCanvas.activeSelf);
    }
    //Close the inventory
    public void HideInventory() 
    {
        inventoryCanvas.SetActive(false);
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
            itemScripts[i].ItemIconStopHover();
        }
    }
}
