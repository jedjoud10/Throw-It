using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//When the player starts dragging an item, this gameobject gets instatiated and follows the mouse
//TODO: No.
public class DraggedItemIconScript : MonoBehaviour
{
    public int itemID;//The item id that we started with (When clicking on an item)
    private RectTransform rectTransform;//The transform for this UI thingy thing
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.anchoredPosition = Input.mousePosition;
    }
}
