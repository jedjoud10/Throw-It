using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script only used for bot "ScrapBot"

//Auto add scripts if they are not already on the gameobject
[RequireComponent(typeof(BotMovementScript))]
public class ScrapBotScript : BotScript
{
    private BotStraightPathScript straightpathScript;//Goes straight to the destination specified
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
