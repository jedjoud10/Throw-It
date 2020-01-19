using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Handles the health and death of bot
public class BotHealthScript : MonoBehaviour
{
    public int MaxHealth;//Maximum health
    public int Health;//Current health
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
            Death();//Death time
        }
    }
    //Death time everyone
    public void Death() 
    {
        Destroy(gameObject);//Fast and chunky way to destroy bot
    }
    //Death after time everyone
    public void Death(float time) 
    {
        Destroy(gameObject, time);//Fast and chunky way to destroy bot after delay
    }
}
