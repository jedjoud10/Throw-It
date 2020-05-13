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
    public NetworkedVarInt Health;//Current health
    public NetworkedVarFloat HealthPercentage;//Curent health, but from 0 to 1
    public float DelayDeath = 3.5f;//Delay before dying
    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            //Setup health 
            Health.Value = MaxHealth;
            HealthPercentage.Value = 1;
        }
    }
    //Called from snowballs to damage bot
    public void DamageBot(int damage) 
    {
        if (!IsServer) return;
        Health.Value -= damage;
        HealthPercentage.Value =  (float) Health.Value / (float) MaxHealth;
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
