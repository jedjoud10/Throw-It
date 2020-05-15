using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BotScript))]
//A scripts that handles communication with a certain type of pathfinders and the bot movement script
public class BotPathfinderScript : NetworkedBehaviour
{
    public float minDistance = 0.3f;//Minimum distance we can get to a destination point before setting the next one as our current destination point
    private Vector3[] destinationPoints;//The points that the bot will go to (It is an array because it is faster than a list)
    private Vector3 currentDestinationPoint;//The current destination point that the bot is heading to
    private BotScript bot;//The bot script for this specific bot
    private AStarPathfinder pathfinder;//The global pathfinder for this current scene
    private Vector3 objectivePosition;//The end position we want this bot to pathfind to
    // Start is called before the first frame update
    void Start()
    {
        bot = GetComponent<BotScript>();
        bot.movementScript.move = false;//Make the bot not able to move until we find a valid path
        objectivePosition = GameObject.FindGameObjectWithTag("Objective").transform.position;
        pathfinder = FindObjectOfType<AStarPathfinder>();
    }

    // Update is called once per frame
    void Update()
    {   
        if (destinationPoints != null)
        {
            for (int i = 0; i < destinationPoints.Length; i++)
            {
                if (Vector3.Distance(destinationPoints[i], transform.position) < minDistance && i != destinationPoints.Length - 1) { currentDestinationPoint = destinationPoints[i + 1]; }//Set the current destination point
            }
        }
        bot.movementScript.MoveToPosition(currentDestinationPoint);//Move to the correct position
    }

    //Set the destination points that the bot will go to
    public void SetDestinationPoints(List<Vector3> points) 
    {
        destinationPoints = points.ToArray();//List to array
        bot.movementScript.move = true;//Let the bot move since we have a valid path
    }
    //Recalculate the bot's path
    public void Pathfind()
    {
        if (pathfinder == null) return;//Bro cringe
        Debug.Log("Pathfind call for bot : " + gameObject.name);
        pathfinder.Pathfind(transform.position, objectivePosition, this);
    }
    private void OnDrawGizmos()
    {
        if (destinationPoints != null)
        {
            for (int i = 0; i < destinationPoints.Length; i++)
            {
                Gizmos.DrawSphere(destinationPoints[i], 1);
            }
        }        
    }
}
