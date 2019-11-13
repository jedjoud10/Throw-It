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
    public float pointthreshold = 0.1f;//The threshold of the distance from us to the reach point to change point index float
    public float RefreshPathRate = 2.0f;//How much delay is there between each get path tests
    // Start is called before the first frame update
    void Start()
    {
        pathfinder = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<FloodFillPathfinder>();//Search the whole scene for the gameobject that holds the pathfinder script and set it as our own pathfinder
        FindPath();//Try to get path
        botMovementScript = GetComponent<BotMovementScript>();//Set movement script to our own
    }

    // Update is called once per frame
    void Update()
    {
        #region Path points loops
        if (points.Count != 0)
        {
            if (Vector3.Distance(transform.position, currentpoint) < pointthreshold)//Check distance and threshold
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
    public void FindPath() //Method that can be called late so we are sure we called it after the calculations
    {        
        points = pathfinder.FindPathFloodFill(transform.position, pathfinder.endPoint.position, this);//Pathfind
        if (points.Count == 0)//Repeat pathfinding until calculations are done
        {
            Invoke("FindPath", RefreshPathRate);//Repeat with the refresh rate
        }
        else
        {
            currentpoint = points[0];//Set the base pos
            currentpointindex = 0;//Reset index since we found new path
        }
    }
}
