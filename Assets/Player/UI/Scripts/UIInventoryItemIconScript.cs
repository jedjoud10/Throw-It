using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryItemIconScript : MonoBehaviour
{
    const float normalSize = 90;//The delta size of this item icon when it is not hovered
    const float hoveredSize = 125;//The delta size of this item icon when it is hovered
    const float selectedSize = 150;//The delta size of this item icon when it is selected
    private float currentSize;//The current ItemIcon size;
    private float targetSize;//The size we want this item icon to be at
    const float smoothing = 5;//How fast to go to the desired icon size?
    private RectTransform rectTransform;
    private Toggle toggle;//The toggle for this item icon
    public PlayerInventoryScript inventoryScript;//The inventory script of the player
    public int itemIndex;//The index of this item icon holder
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        toggle = GetComponent<Toggle>();
        targetSize = normalSize;
    }

    // Update is called once per frame
    void Update()
    {
        currentSize = Mathf.Lerp(currentSize, targetSize, smoothing * Time.deltaTime);
        rectTransform.sizeDelta = new Vector2(currentSize, currentSize);//Update the item icon size
    }
    //When the item icon has been hovered by the mouse
    public void ItemIconHover() 
    {
        if (toggle != null)
        {
            if (!toggle.isOn)
            {                
                targetSize = hoveredSize;
            }
        }
    }
    //When the item icon stopped being hovered by the mouse
    public void ItemIconStopHover() 
    {
        if (toggle != null)
        {
            if (!toggle.isOn)
            {
                //Only stop the hovering if the item isnt selected
                targetSize = normalSize;
            }
        }
    }
    //Called when the user toggles this item icon
    public void ItemIconToggleChange(bool _toggle) 
    {
        if (_toggle) 
        {
            //The item is selected, so set its size properly
            targetSize = selectedSize;
            inventoryScript.EquipItemWithIndex(itemIndex);
        }
        else
        {
            //The item is not selected, so set its size properly
            targetSize = normalSize;
            inventoryScript.EquipItemWithIndex(-1);
        }
    }
}
