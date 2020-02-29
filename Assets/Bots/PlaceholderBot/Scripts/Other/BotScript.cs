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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
