using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BotScript))]
//A scripts that handles communication with a certain type of pathfinders and the bot movement script
//TODO : Networking support and optimizations
public class BotPathfinderScript : NetworkedBehaviour
{
    public float MinDistance;//Minimum distance we can get to a destination point before setting the next one as our current destination point
    private Vector3[] DestinationPoints;//The points that the bot will go to (It is an array because it is faster than a list)
    private Vector3 CurrentDestinationPoint;//The current destination point that the bot is heading to
    private BotScript bot;//The bot script for this specific bot
    private AStarPathfinder pathfinder;//The global pathfinder for this current scene
    // Start is called before the first frame update
    void Start()
    {
        bot = GetComponent<BotScript>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < DestinationPoints.Length; i++)
        {
            if(Vector3.Distance(DestinationPoints[i], transform.position) < MinDistance && i != DestinationPoints.Length - 1) { CurrentDestinationPoint = DestinationPoints[i+1]; }//Set the current destination point
        }
        bot.movementScript.MoveToPosition(CurrentDestinationPoint);//Move to the correct position
    }

    //Set the destination points that the bot will go to
    public void SetDestinationPoints(List<Vector3> points) 
    {
        DestinationPoints = points.ToArray();//List to array
    }
    //Recalculate the bot's path
}
