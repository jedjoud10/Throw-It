using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Handles the health and death of bot
public class BotHealthScript : MonoBehaviour
{
    [HideInInspector]
    public BotScript botScript;//The script for our bot
    public int MaxHealth;//Maximum health
    public int Health;//Current health
    public float DelayDeath = 3.5f;//Delay before dying
    // Start is called before the first frame update
    void Start()
    {
        Health = MaxHealth;//Setup health 
    }
    //Called from snowballs to damage bot
    public void DamageBot(int damage) 
    {
        Health -= damage;
        if(Health <= 0)//Bot is dead 
        {
            Death();
        }
    }
    //Death after time everyone
    public void Death() 
    {
        if (gameObject == null) return;
        Destroy(gameObject, DelayDeath);//Fast and chunky way to destroy bot after delay
        botScript.Death();
    }
}
