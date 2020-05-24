using UnityEngine;
using System.Collections;
//A throwable item
[CreateAssetMenu(fileName = "New Throwable", menuName = "Inventory/Create new Throwable")]
public class Consumable : Item
{
    public int healthRegeneration;//How much to heal the player
}
