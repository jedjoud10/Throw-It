using UnityEngine;
using System.Collections;
//A throwable item
[CreateAssetMenu(fileName = "New Throwable", menuName = "Inventory/Create new Throwable")]
public class Throwable : Item
{
    public GameObject throwableGameObject;    
}
public enum ThrowableType
{
    snowball
}