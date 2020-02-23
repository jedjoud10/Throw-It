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
        StartCoroutine("WorldUpdate");
    }

    // Update is called once per frame
    void Update()
    {

    }
    //Called when map has changed
    public IEnumerator WorldUpdate() 
    {
        #region Bots path calculations/recalculations
        //Recalculates every bots's path and updates pathfinding grid
        BotPathfinderScript[] pathfinders = FindObjectsOfType<BotPathfinderScript>();
        FindObjectOfType<AStarPathfinder>().MakeGrid();//Recalculate grid
        yield return new WaitForSecondsRealtime(1.0f);
        for (int i = 0; i < pathfinders.Length; i++)
        {
            yield return new WaitForSecondsRealtime(1.0f);
            pathfinders[i].FindPath();
        }
        #endregion
    }
}
