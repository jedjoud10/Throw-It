using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BotMovementScript))]
//Moves the bot using the BotMovementScript.cs in a straight line to the endPoint
public class BotStraightPathScript : MonoBehaviour
{
    private Transform endPoint;//The end position that we want to go
    // Start is called before the first frame update
    void Start()
    {
        endPoint = GameObject.FindGameObjectWithTag("Objective").transform;//Get objective from scene
        Invoke("InitEndPoint", 1.0f);//Call after delay
    }
    //So what we are doing is we are getting the BotMovementScript of this bot, then we tell it that we want to go to the end position. We call it with an invoke and with a delay because the position is the bots position at start
    private void InitEndPoint() 
    {
        GetComponent<BotMovementScript>().MoveToPosition(endPoint.position);
    }
}
