using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryItemIconScript : MonoBehaviour
{
    const float normalSize = 100;//The delta size of this item icon when it is not hovered
    const float hoveredSize = 120;//The delta size of this item icon when it is hovered
    private float currentSize;//The current ItemIcon size;
    private float targetSize;//The size we want this item icon to be at
    const float smoothing = 5;//How fast to go to the desired icon size?
    private RectTransform rectTransform;
    public PlayerInventoryScript inventoryScript;//The inventory script of the player
    public PlayerInventoryUIManagerScript UIManager;
    public int itemIndex;//The index of this item icon holder
    public void InitItemIcon() 
    {
        rectTransform = GetComponent<RectTransform>();
        targetSize = normalSize;
        #region no.
        EventTrigger.Entry starthover, stophover, click;

        starthover = new EventTrigger.Entry();
        starthover.eventID = EventTriggerType.PointerEnter;
        starthover.callback.AddListener((eventData) => { ItemIconStartHover(); });

        stophover = new EventTrigger.Entry();
        stophover.eventID = EventTriggerType.PointerExit;
        stophover.callback.AddListener((eventData) => { ItemIconStopHover(); });

        click = new EventTrigger.Entry();
        click.eventID = EventTriggerType.PointerClick;
        click.callback.AddListener((eventData) => { ItemIconClick(); });

        EventTrigger trigger = GetComponent<EventTrigger>();
        trigger.triggers.Add(starthover);
        trigger.triggers.Add(stophover);
        trigger.triggers.Add(click);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        currentSize = Mathf.Lerp(currentSize, targetSize, smoothing * Time.deltaTime);
        rectTransform.sizeDelta = new Vector2(currentSize, currentSize);//Update the item icon size
    }
    //Reset the size of the item icon when opening the inventory
    public void ResetItemIconSize() 
    {
        currentSize = 0;
        rectTransform.sizeDelta = new Vector2(currentSize, currentSize);//Update the item icon size
    }
    //When the item icon has been hovered by the mouse
    public void ItemIconStartHover() 
    {              
        targetSize = hoveredSize;
        UIManager.ShowItemData(ItemsHandler.ID2Item(inventoryScript.GetItemID(itemIndex)));
    }
    //When the item icon stopped being hovered by the mouse
    public void ItemIconStopHover() 
    {
        //Stop the hovering
        targetSize = normalSize;
        UIManager.HideItemData();
    }
    //When the user clicks on an item icon
    private void ItemIconClick() 
    {
        Debug.Log("Clicked on itemIcon " + itemIndex);        
        UIManager.ClickItemIcon(itemIndex, RectTransformUtility.WorldToScreenPoint(null, rectTransform.position));
    }
}
