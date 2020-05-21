using MLAPI;
using MLAPI.NetworkedVar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Parent script that will be inherited from other scripts for bot creation
//Auto add scripts if they are not already on the gameobject
[RequireComponent(typeof(BotMovementScript))]
[RequireComponent(typeof(BotHealthScript))]
public class BotScript : NetworkedBehaviour
{
    //Internal variables for children classes to use
    public BotMovementScript movementScript;
    public BotBobbingScript bobbingScript;
    public BotHealthScript healthScript;
    protected NetworkedVarBool isDead;//When the bot is dead
    public float rotationOffsetX;
    // Start is called before the first frame update
    virtual public void Start()
    {
        //Init this instance for bot scripts
        healthScript.botScript = this;
        movementScript.botScript = this;
    }

    // Update is called once per frame
    virtual public void Update()
    {
        
    }
    //Called when bot dies (Only executed on server)
    virtual public void OnBotDeath() 
    {
        if (!IsServer) return;
        //Slow down movement and bobbing
        movementScript.move = false;
        bobbingScript.applybobbing = false;

        isDead.Value = true;//well, he is dead
    }
    //Called when bot gets damaged (Only executed on server)
    virtual public void OnBotDamage(int damage, int newHealth) 
    {
    }
}
