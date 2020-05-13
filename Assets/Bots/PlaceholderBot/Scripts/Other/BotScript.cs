using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Parent script that will be inherited from other scripts

//Auto add scripts if they are not already on the gameobject

//TODO : Networking support
[RequireComponent(typeof(BotMovementScript))]
[RequireComponent(typeof(BotHealthScript))]
public class BotScript : NetworkedBehaviour
{
    //Internal variables for children classes to use
    public BotMovementScript movementScript;
    public BotBobbingScript bobbingScript;
    public BotHealthScript healthScript;
    protected bool isDead;//When the bot is dead
    // Start is called before the first frame update
    virtual public void Start()
    {
        healthScript.botScript = this;//Init bot script for health script
    }

    // Update is called once per frame
    virtual public void Update()
    {
        
    }
    //Called when bot dies
    virtual public void OnBotDeath() 
    {
        //Slow down movement and bobbing
        movementScript.Move = false;
        if(bobbingScript != null) bobbingScript.applybobbing = false;

        isDead = true;//well, he is dead
    }
    //Called when bot gets damaged
    virtual public void OnBotDamage(int damage, int newHealth) 
    {
    }
}
