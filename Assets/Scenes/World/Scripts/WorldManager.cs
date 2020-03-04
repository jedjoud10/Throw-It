using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Handles communications between multiple scripts and classes
public class WorldManager : MonoBehaviour
{
    public float waterHeight;//The height position of water
    private AStarPathfinder pathfinder;//Pathfinder used to pathfind bots's pathes
    // Start is called before the first frame update
    void Start()
    {
        pathfinder = FindObjectOfType<AStarPathfinder>();//Init base pathfinder
        if (pathfinder != null) pathfinder.MakeTerrainGrid();
        StartCoroutine("WorldUpdateCoroutine");
    }

    // Update is called once per frame
    void Update()
    {

    }
    //Called internally when map has changed
    private IEnumerator WorldUpdateCoroutine() 
    {
        #region Bots path calculations/recalculations
        //Recalculates every bots's path and updates pathfinding grid
        if (pathfinder != null)
        {
            BotPathfinderScript[] pathfinders = FindObjectsOfType<BotPathfinderScript>();
            FindObjectOfType<AStarPathfinder>().MakeGrid();//Recalculate grid
            yield return new WaitForSecondsRealtime(1.0f);
            for (int i = 0; i < pathfinders.Length; i++)
            {
                yield return new WaitForSecondsRealtime(1.0f);
                pathfinders[i].FindPath();
            }
        }
        #endregion
    }
    //Called externally by scripts to start coroutine to start map update
    public void WorldUpdate() 
    {
        StartCoroutine("WorldUpdateCoroutine");
    }
}
