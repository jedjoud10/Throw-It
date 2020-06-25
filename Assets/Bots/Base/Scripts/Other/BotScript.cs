using MLAPI;
using MLAPI.NetworkedVar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Parent script that will be inherited from other scripts for bot creation
//Auto add scripts if they are not already on the gameobject
[RequireComponent(typeof(BotMovementBaseMethodScript))]
[RequireComponent(typeof(BotHealthScript))]
public class BotScript : NetworkedBehaviour
{
    //Internal variables for children classes to use
    public BotMovementBaseMethodScript botMovementScript;
    public EntityMovementScript entityMovementScript;
    public BotBobbingScript bobbingScript;
    public BotHealthScript healthScript;
    protected NetworkedVarBool isDead;//When the bot is dead
    public float rotationOffsetX;//gotta rethink and redprogram this later
    public float delayDeath = 3.5f;//Delay before dying
    // Start is called before the first frame update
    virtual public void Start()
    {
        botMovementScript = GetComponent<BotMovementBaseMethodScript>();
        entityMovementScript = GetComponent<EntityMovementScript>();
        healthScript = GetComponent<BotHealthScript>();
        //Init this instance for bot scripts
        healthScript.botScript = this;
        botMovementScript.botScript = this;
    }

    // Update is called once per frame
    virtual public void Update()
    {
        
    }
    //Called when bot dies (Only executed on server)
    virtual public void OnBotDeath() 
    {
        if (!IsServer) return;
        Destroy(gameObject, delayDeath);//Fast and chunky way to destroy bot after delay
        //Slow down movement and bobbing
        entityMovementScript.apply = false;
        bobbingScript.applybobbing = false;

        isDead.Value = true;//well, he is dead
    }
    //Called when bot gets damaged (Only executed on server)
    virtual public void OnBotDamage(int damage, int newHealth) 
    {
    }
}
