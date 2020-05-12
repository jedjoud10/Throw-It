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
    public int MaxHealth;//Maximum health
    public NetworkedVarInt Health ;//Current health
    public float DelayDeath = 3.5f;//Delay before dying
    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            Health.Value = MaxHealth;//Setup health 
        }
    }
    //Called from snowballs to damage bot
    public void DamageBot(int damage) 
    {
        if (!IsServer) return;
        Health.Value -= damage;
        botScript.OnBotDamage(damage, Health.Value);
        if(Health.Value <= 0)//Bot is dead 
        {
            Death();
        }
    }
    //Death after time everyone
    public void Death() 
    {
        if (gameObject == null) return;
        Destroy(gameObject, DelayDeath);//Fast and chunky way to destroy bot after delay
        botScript.OnBotDeath();
    }
}
