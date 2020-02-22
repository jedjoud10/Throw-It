using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Handles communications between multiple scripts and classes
public class WorldManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<AStarPathfinder>().MakeTerrainGrid();//Init base terrain
        Invoke("WorldUpdate", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    //Called when map has changed
    public void WorldUpdate() 
    {
        //Recalculates every bots's path and updates pathfinding grid
        BotPathfinderScript[] pathfinders = FindObjectsOfType<BotPathfinderScript>();
        FindObjectOfType<AStarPathfinder>().MakeGrid();//Recalculate grid
        for (int i = 0; i < pathfinders.Length; i++)
        {            
            pathfinders[i].FindPath();
        }


    }
}
