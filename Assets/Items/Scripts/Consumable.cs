using UnityEngine;
using System.Collections;
//A throwable item
[System.Serializable]
public class Consumable : Item
{
    public int healthRegeneration;//How much to heal the player
    public int temperatureRegeneration;//How much to warm the player
}
