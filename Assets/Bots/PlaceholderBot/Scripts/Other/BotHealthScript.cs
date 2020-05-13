using MLAPI;
using MLAPI.NetworkedVar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Handles the health and death of bot
public class BotHealthScript : NetworkedBehaviour
{
    [HideInInspector]
    public BotScript botScript;//The script for our bot
    public int maxHealth;//Maximum health
    public NetworkedVarInt health;//Current health
    public NetworkedVarFloat healthPercentage;//Curent health, but from 0 to 1
    public float delayDeath = 3.5f;//Delay before dying
    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            //Setup health 
            health.Value = maxHealth;
            healthPercentage.Value = 1;
        }
    }
    //Called from snowballs to damage bot
    public void DamageBot(int damage) 
    {
        if (!IsServer) return;
        health.Value -= damage;
        healthPercentage.Value =  (float) health.Value / (float) maxHealth;
        botScript.OnBotDamage(damage, health.Value);
        if(health.Value <= 0)//Bot is dead 
        {
            Death();
        }
    }
    //Death after time everyone
    public void Death() 
    {
        if (gameObject == null) return;
        Destroy(gameObject, delayDeath);//Fast and chunky way to destroy bot after delay
        botScript.OnBotDeath();
    }
}
