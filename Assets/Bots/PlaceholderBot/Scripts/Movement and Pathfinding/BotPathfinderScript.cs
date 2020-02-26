using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BotScript))]
//A scripts that handles communication with a certain type of pathfinders and the bot movement script
public class BotPathfinderScript : MonoBehaviour
{
    private BotScript botScript;//The script that handles bots
    private AStarPathfinder pathfinder;//Our trusty flood fill pathfinder
    private List<Vector3> points = new List<Vector3>();//The points
    private int currentpointindex = 0;//The index of the point we are trying to reach
    private Vector3 currentpoint;//Our current point vector
    public float pointThreshold = 0.1f;//The threshold of the distance from us to the reach point to change point index float
    private Vector3 MyPos;//The position of the bot
    private Vector3 EndPos;//The position we are going to get to
    // Start is called before the first frame update
    void Start()
    {
        pathfinder = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<AStarPathfinder>();//Search the whole scene for the gameobject that holds the pathfinder script and set it as our own pathfinder
        botScript = GetComponent<BotScript>();//Set movement script to our own
        Invoke("FindPath", 1.0f);

        SetEndPosition(GameObject.FindGameObjectWithTag("Objective").transform.position);//Just a placeholder

        #region Position setting
        /*
         Setting the bot's position since we cannot call the position from other threads, and since the pathfinder might recall us to repathfind, we might get some errors. So that is why we put some variables holding some positions
        */
        MyPos = transform.position;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if(pathfinder != null) 
        { 
            #region Position setting
            /*
             Setting the bot's position and the end point's position since we cannot call their position from other threads, and since the pathfinder might recall us to repathfind, we might get some errors. So that is why we put some variables holding some positions
            */            

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
                botScript.movementScript.MoveToPosition(currentpoint);//Move to position passed to the botmovementscript
            }    
            #endregion
        }
    }
    //Sets the position we want to get to
    public void SetEndPosition(Vector3 _endPos) 
    {
        EndPos = _endPos;
    }
    //Only call (On each bot) when map has changed, and not repetedly so we can save on performence
    public void FindPath() //Find path using the A* pathfinder
    {
        if (pathfinder != null)
        {
            Debug.Log("Pathfind call for bot " + gameObject.name);
            MyPos = transform.position;
            pathfinder.Pathfind(MyPos, EndPos, this);//Pathfind
        }
    }
    public void SetNewPoints(List<Vector3> _points) //Settings of new points called from the threaded method
    {
        points = _points;//Sets new points
        currentpoint = points[0];//Set the base pos
        currentpointindex = 0;//Reset index since we found new path        
    }
    //Since the bot is going to be destroyed, remove it from queue so we dont call it again when map changes and when we do recalculations
    private void OnDestroy()
    {
        if (pathfinder != null)
        {
            pathfinder.RemoveFromQueue(this);
        }
    }
    private void OnDrawGizmos()
    {
        if (points != null && points.Count != 0)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (i < points.Count - 1)
                {
                    Debug.DrawLine(points[i], points[i + 1], Color.green);
                    Gizmos.DrawSphere(points[i], 0.5f);
                }
            }
        }
    }
}
