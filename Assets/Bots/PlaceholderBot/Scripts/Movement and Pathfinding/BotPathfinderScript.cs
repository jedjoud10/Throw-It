using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BotMovementScript))]
//A scripts that handles communication with the floodfill pathfinder and the bot movement script
public class BotPathfinderScript : MonoBehaviour
{
    private BotMovementScript botMovementScript;//The script that handles gravity and movement
    [Tooltip("Our trusty flood fill pathfinder gameobject")]
    private FloodFillPathfinder pathfinder;//Our trusty flood fill pathfinder
    private List<Vector3> points = new List<Vector3>();//The points
    private int currentpointindex = 0;//The index of the point we are trying to reach
    private Vector3 currentpoint;//Our current point vector
    public float pointThreshold = 0.1f;//The threshold of the distance from us to the reach point to change point index float
    private Vector3 MyPos;//The position of the bot
    private Vector3 EndPos;//The position of the things that we are going for
    // Start is called before the first frame update
    void Start()
    {
        pathfinder = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<FloodFillPathfinder>();//Search the whole scene for the gameobject that holds the pathfinder script and set it as our own pathfinder
        Invoke("FindPath", 1.0f);//Try to get path after delay so we are sure to get it correctly
        botMovementScript = GetComponent<BotMovementScript>();//Set movement script to our own
        #region Position setting
        /*
         Setting the bot's position and the end point's position since we cannot call their position from other threads, and since the pathfinder might recall us to repathfind, we might get some errors. So that is why we put some variables holding some positions
        */
        EndPos = pathfinder.endPoint.position;
        MyPos = transform.position;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region Position setting
        /*
         Setting the bot's position and the end point's position since we cannot call their position from other threads, and since the pathfinder might recall us to repathfind, we might get some errors. So that is why we put some variables holding some positions
        */
        EndPos = pathfinder.endPoint.position;
        MyPos = transform.position;
        #endregion
        #region Path points loops
        if (points.Count != 0)
        {
            if (Vector3.Distance(transform.position, currentpoint) < pointThreshold)//Check distance and threshold
            {
                if (currentpointindex < points.Count - 1)//Not to get error out of index
                {
                    currentpointindex++;//Add one to index
                }
                currentpoint = points[Mathf.Clamp(currentpointindex, 0, points.Count - 1)];//Clamping the value for no out of range errors
            }
            currentpoint.y = transform.position.y;
            botMovementScript.MoveToPosition(currentpoint);//Move to position passed to the botmovementscript
        }
        #endregion
    }
    //Only call (On each bot) when map has changed, and not repetedly so we can save on performence
    public void FindPath() //Method that can be called late so we are sure we called it after the calculations
    {        
        pathfinder.FindPathFloodFill(MyPos, EndPos, this);//Pathfind
    }
    public void SetNewPoints(List<Vector3> _points) //Settings of new points called from the threaded method
    {
        points = _points;//Sets new points
        if (points.Count == 0)//Repeat pathfinding until calculations are done
        {
            //Invoke("FindPath", RefreshPathRate);//Repeat with the refresh rate
        }
        else
        {
            currentpoint = points[0];//Set the base pos
            currentpointindex = 0;//Reset index since we found new path
        }
    }
}
