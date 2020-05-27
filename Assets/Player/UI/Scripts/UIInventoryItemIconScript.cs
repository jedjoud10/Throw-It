using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryItemIconScript : MonoBehaviour
{
    const float normalSize = 90;//The delta size of this item icon when it is not hovered
    const float hoveredSize = 125;//The delta size of this item icon when it is hovered
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
    }
    //When the item icon has been hovered by the mouse
    public void ItemIconHover() 
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
    //Start dragging this ui object
    public void StartDragItemIcon() 
    {
        UIManager.StartDrag(itemIndex);
    }
    //Stop dragging and replace this item icon with the "drag" one
    public void StopDragItemIcon() 
    {
        UIManager.StopDrag(itemIndex);
    }
}
