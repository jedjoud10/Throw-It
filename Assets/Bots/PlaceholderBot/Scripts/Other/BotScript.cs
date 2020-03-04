using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Parent script that will be inherited from other scripts

//Auto add scripts if they are not already on the gameobject
[RequireComponent(typeof(BotMovementScript))]
[RequireComponent(typeof(BotHealthScript))]
public class BotScript : MonoBehaviour
{
    //Internal variables for children classes to use
    public BotMovementScript movementScript;
    public BotBobbingScript bobbingScript;
    public BotHealthScript healthScript;
    private protected bool alive = true;//If the bot is alive so we can use the Update function to do stuff only when the bot is alive or vice-versa
    // Start is called before the first frame update
    public virtual void Start()
    {
        healthScript.botScript = this;//Init bot script for health script
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }
    //Called when bot dies
    public virtual void Death() 
    {
        //Slow down movement and bobbing
        movementScript.move = false;
        bobbingScript.applybobbing = false;
        alive = false;
    }
}
