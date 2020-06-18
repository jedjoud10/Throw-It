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
    public Canvas inventoryCanvas;//The whole inventory canvas
    public RawImage[] itemIcons;//The item icons
    public UIInventoryItemIconScript[] itemIconScripts;
    public TMP_Text itemName;//The name of the current equipped item
    public TMP_Text itemDescription;//Description of the current eqquiped item
    public TMP_Text itemCustomDescription;//Custom description if the item is a child item class
    public RawImage hotbar1, hotbar2, hotbar3, hotbar4;
    private PlayerInventoryScript inventoryScript;//The inventory handling
    private int selectedItemIconSwapperIndex = -1;//The new selected item that we will swap the old selected item into
    private Vector2 selectedItemIconSwaperPosition;//The position of the selected item
    public RectTransform selectedItemIconIndicator;//yes.
    public GameObject[] hotbarEquipedItemSelectors;
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
        if (selectedItemIconSwapperIndex != -1)//We are currently selecting an item
        {
            Vector2 diff = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - selectedItemIconSwaperPosition;
            selectedItemIconIndicator.sizeDelta = new Vector2(diff.magnitude / inventoryCanvas.scaleFactor - 10, 10);
            selectedItemIconIndicator.pivot = new Vector2(0, 0.5f);
            selectedItemIconIndicator.position = selectedItemIconSwaperPosition;
            float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            selectedItemIconIndicator.rotation = Quaternion.Euler(0, 0, angle);
        }
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
        inventoryCanvas.gameObject.SetActive(!inventoryCanvas.gameObject.activeSelf);
        if (inventoryCanvas.gameObject.activeSelf) 
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
        inventoryCanvas.gameObject.SetActive(visible);
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
    //Set the equipedItemIndicator correctly
    public void SetEquipedItemIndex(int itemIndex) 
    {
        for (int i = 0; i < hotbarEquipedItemSelectors.Length; i++)
        {
            hotbarEquipedItemSelectors[i].SetActive(i == itemIndex);//Turn on the correct selector and turn off all the other ones
        }
    }
    //When the user clicks on an item icon
    public void ClickItemIcon(int itemIconIndex, Vector2 pos) 
    {
        if (selectedItemIconSwapperIndex == -1) 
        { 
            selectedItemIconSwapperIndex = itemIconIndex;
            selectedItemIconSwaperPosition = pos;
            selectedItemIconIndicator.gameObject.SetActive(true);
        }
        else
        {
            //We have both indexes, we can swap
            inventoryScript.SwapItems(selectedItemIconSwapperIndex, itemIconIndex);
            selectedItemIconSwaperPosition = Vector2.zero;
            selectedItemIconIndicator.gameObject.SetActive(false);
            selectedItemIconSwapperIndex = -1;//Reset
        }
    }

}
