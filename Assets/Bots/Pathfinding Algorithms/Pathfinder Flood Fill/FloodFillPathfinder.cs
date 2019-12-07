using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEditor;
//Multithreaded pathfinder that uses the flood fill algorithm
public class FloodFillPathfinder : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    [Tooltip("Can we log debug messages ?")]
    public bool UseDebug;
    [Tooltip("How to use gizmos")]
    public GizmoMode gizmoMode;//How to use gizmos
    [Tooltip("The max number of threads that this pathfinder can use when bots call for their path")]
    public int MaxNumThread;//The max number of threads that this pathfinder can use when bots call for their path
    [Range(1, 10)]
    [Tooltip("How much detail can we allow")]
    public int Resolution = 1;//How much detail can we allow
    [Tooltip("The object you are trying to reach")]
    public Transform endPoint;//The object you are trying to reach
    [Tooltip("The max iterations that you are allowed while reverse pathfinding the path")]
    public int maxIterationReversePath;//The max iterations that you are allowed while reverse pathfinding the path
    [Tooltip("The max number of iterations that you are allowed")]
    public int maxIterations;//The max number of iterations that you are allowed
    [Tooltip("Can we use diagonals for neighbour search ?")]
    public bool useDiagonals;//Can we use diagonals for neighbour search ?
    [Tooltip("Can we move using the diagonals ?")]
    public bool moveDiagonals;//Can we move using the diagonals ?
    private List<Vector3> Directions = new List<Vector3>();//The directions that you can use while searching neighbours
    private Node endNode;//The node of the end object
    [Tooltip("The max number of nodes in the X direction")]
    public int gridsizeX = 0;//The max number of nodes in the X direction
    [Tooltip("The max number of nodes in the Y direction (Z direction in WorldPosition)")]
    public int gridsizeY = 0;//The max number of nodes in the Y direction
    [Tooltip("The scale of the grid")]
    public float gridScale;//The scale of the grid
    [Tooltip("The radius of the sphere to check collisions with other objects")]
    public float sphereRadiusBlockage;//The radius of the sphere to check collisions for
    #endregion
    [Header("Weighting")]
    #region Weighting Settings
    [Tooltip("Can we use gameobjects for our node weighting?")]
    public bool useDraggablePoints;//Can we use gameobjects for our node weighting ?
    [Tooltip("How smooth is the weighting based off distance between the nodes and the closest weight")]
    public float Smoothness = 1;//How smooth is the weighting based off distance between the nodes and the closest weight
    [Tooltip("How much offset should we add to the weight")]
    public float Weightoffset = 0;//How much offset should we add to the weight
    [Tooltip("The nodes in the editor for weighting the closest nodes")]
    public List<GameObject> GameobjectNodes;//The nodes in the editor for weighting the closest nodes
    #endregion
    #region Private Variables
    private Node[,] nodes = new Node[0, 0];//The grid array
    private Node[,] finalnodes = new Node[0, 0];//The final grid array after pathfinding
    private Vector3 basePos;//Some base offset variable
    private List<Node> path = new List<Node>();//The path calculated
    private List<List<Node>> pathes = new List<List<Node>>();//All the pathes that bots have called upon us
    private Thread calculationThread;//The thread that will be used for calculations
    private bool finishedCalculations;//Bool that tells us if the calculation thread has finished its work
    private List<Node> currentlyExploringNodes;//The current nodes that are being controlled
    private int BiggestNumNode = 0;//The biggest iteration count foreach node in calculated grid
    private List<BotPathfinderScript> bots = new List<BotPathfinderScript>();//The bots that called this pathfinder class 
    private Thread[] reversecalculationthreads = new Thread[0];//The threads for each bot to use for reverse calculations
    public enum GizmoMode 
    {
        Explored_Node, Grid, Path, None
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        gridsizeX *= Resolution;//Make more nodes in X
        gridsizeY *= Resolution;//Make more nodes int Y
        gridScale /= Resolution;//Make scale less
        reversecalculationthreads = new Thread[MaxNumThread];//Setup the thread array
        RecalculateMap();//Calculate everything from start
    }
    public void RecalculateMap() //Method called when the map has changed (Ex : Player has placed a brick)
    {
        SetDirections(useDiagonals);
        pathes = new List<List<Node>>();//Reset pathes
        basePos = endPoint.position - new Vector3(gridsizeX * gridScale, 0, gridsizeY * gridScale) / 2;//Setting base offset
        basePos -= new Vector3(gridScale, gridScale, gridScale) / 2;//Offset it one node so the end point is only one node and not in the middle of two nodes
        MakeGrid();//Make the grid of nodes
        endNode = NodeFromWorldPosition(endPoint.position);//Get end node from position of end object
        endNode.Iteration = 0;
        Node startNode = NodeFromWorldPosition(transform.position);
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();//Using a stopwatch to get how much time did we spend calculating pathes
        calculationThread = new Thread(() => CalculateNodes(endNode, startNode));
        calculationThread.Start();//Start the thread and start calculating
        if (!Debug.isDebugBuild)
        {
            UseDebug = false;//Always set the use debug if we are in a build who isnt a debug build
        }
        #region Recalculations of pathes
        if (bots.Count > 1)
        {
            bots.RemoveAll(bot => bot == null);//Remove null objects
        }
        foreach (var bot in bots)
        {
            if (bot != null)//Recheck if we are not null
            {
                bot.FindPath();
            }
        }
        #endregion
    }
    public void FindPathFloodFill(Vector3 pos, Vector3 endpos, BotPathfinderScript botscript)//A method to be called from bots when they need they'r path to be found 
    {
        Debug.Log(reversecalculationthreads.Length);
        for (int i = 0; i < reversecalculationthreads.Length; i++)//Loops over all avaible threads
        {
            Thread thread = reversecalculationthreads[i];//Reference for helping me out
            if (thread == null)
            {
                thread = new Thread(() => FindPathFloodFillThread(pos, endpos, botscript));//Init the thread
                thread.Start();//Start the thread
                return; //We have nothing else to do so we can return
            }            
            else if(!thread.IsAlive)//Checks available threads
            {
                thread = new Thread(() => FindPathFloodFillThread(pos, endpos, botscript));//Init the thread
                thread.Start();//Start the thread
                return; //We have nothing else to do so we can return
            }
        }        
    }
    private void FindPathFloodFillThread(Vector3 pos, Vector3 endpos, BotPathfinderScript botscript) //This method will handles the reverse pathfinding in other threads
    {
        /*
         Every bot that wants its path calculated and if the grid has not been calculated yet, it will add it to the bots list and wait until the grid is done so we can call them all to calculate pathes
        */         
        if (!bots.Contains(botscript))//Doing this to avoid duplicates
        {
            bots.Add(botscript);//Adding the bot script to the list of bots, so when the map changes we can recall every path
        }
        List<Vector3> endpathpoints = new List<Vector3>();//The output of point the bot is going to
        Node myendnode;//A end node for this method only because we are multithreaded 
        List<Node> mypath;//Make a path specific for this method only since again, we are multithreaded
        if (finishedCalculations)
        {
            myendnode = NodeFromWorldPosition(endpos);//Get end node from position of end object
            myendnode.Iteration = 0;
            mypath = GetPath(NodeFromWorldPosition(pos), myendnode);//Finding path
            foreach (var node in mypath)//Change each node to point in 3D space for bot to move to
            {
                endpathpoints.Add(node.WorldPosition);//Add point to point list to return
            }
            pathes.Add(mypath);//Add the path to the total pathes for gizmos
        }
        else
        {
            if (UseDebug)
            {
                Debug.Log("Didnt caclulate path yet !");//Still didnt calculate each node's number
            }
        }
        botscript.SetNewPoints(endpathpoints);//Set new points of bot
    }
    private void SetDirections(bool diagonals) 
    {
        Directions.Clear();
        Directions.Add(Vector3.forward * gridScale);
        Directions.Add(Vector3.forward * -gridScale);
        Directions.Add(Vector3.right * gridScale);
        Directions.Add(Vector3.right * -gridScale);
        if (diagonals)
        {
            Directions.Add(Vector3.Lerp(Vector3.forward, Vector3.right, 0.5f) * gridScale * 2);
            Directions.Add(Vector3.Lerp(-Vector3.forward, Vector3.right, 0.5f) * gridScale * 2);
            Directions.Add(Vector3.Lerp(Vector3.forward, -Vector3.right, 0.5f) * gridScale * 2);
            Directions.Add(Vector3.Lerp(-Vector3.forward, -Vector3.right, 0.5f) * gridScale * 2);
        }
    }
    public void MakeGrid()//Make the grid based on the size of X and Y and DetailPrecision
    {
        Vector3 pos;//Make a refference of position for later nodes
        nodes = new Node[gridsizeX, gridsizeY];//Resize the grid array
        float dist;//A float var if we ever use weights. We will use this var as mult for the weight, the more further away, the weights are less
        for (int x = 0; x < gridsizeX; x++)//Loop of X
        {
            for (int y = 0; y < gridsizeY; y++)//Loop of Y
            {
                pos = new Vector3(gridScale * x + basePos.x, transform.position.y, gridScale * y + basePos.z);//Set the correct location for next line
                if (useDraggablePoints)//Lerp the current pos to the closest point, only in y axis for the moment
                {
                    dist = Vector3.Distance(pos, PosOfClosest(GameobjectNodes, pos));
                    pos.y = Vector3.Lerp(PosOfClosest(GameobjectNodes, pos), pos, dist / Smoothness + Weightoffset).y;//Apply weight
                }
                nodes[x, y] = new Node(!Physics.CheckSphere(pos, sphereRadiusBlockage), pos, 0, x, y);//Set the node at (X, Y) to the coresponding location
            }
        }     
    }
    private Vector3 PosOfClosest(List<GameObject> listofpoints, Vector3 startpoint)//Get the position of the closest point
    {
        Vector3 pos = Vector3.zero;//Final output position
        float smallestDistance = Mathf.Infinity;//A smallest dist float to keep track
        foreach (var point in listofpoints)
        {
            if (Vector3.Distance(point.transform.position, startpoint) < smallestDistance)//Get the CLOSEST point
            {
                smallestDistance = Vector3.Distance(point.transform.position, startpoint);
                pos = point.transform.position;
            }
        }
        return pos;
    }
    public Node NodeFromWorldPosition(Vector3 pos)//Getting a node from grid from world position 
    {
        pos -= basePos;//Remove the base pos so we have correct mesurements with correct offset
        #region Snapping to grid
        pos.x = pos.x / gridScale;
        pos.z = pos.z / gridScale;
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.z);
        #endregion
        x = Mathf.Clamp(x , 0, gridsizeX - 1);//Clamping so the value cant be out of index
        y = Mathf.Clamp(y , 0, gridsizeY - 1);
        return nodes[x, y];
    }
    private void OnDrawGizmos()//Debuging gizmos
    {
        if (gizmoMode == GizmoMode.Grid)
        {
            Gizmos.DrawCube(endPoint.position - new Vector3(gridScale, 0, gridScale), new Vector3(gridsizeX * gridScale, 1f, gridsizeY * gridScale));//Draw area of pathfinder
            foreach (var node in nodes)
            {
                if (node.IsWalkable)
                {
                    //Handles.Label(node.WorldPosition, node.Iteration.ToString());//Shows the iteration number ontop of the node
                    Gizmos.color = new Color((float)node.Iteration / (float)BiggestNumNode, (float)node.Iteration / (float)BiggestNumNode, (float)node.Iteration / (float)BiggestNumNode);//Sets our grayscale color to represent out iteration count
                    Gizmos.DrawSphere(node.WorldPosition, sphereRadiusBlockage);//Visualizing each node who is walkable
                }
            }
        }
        if (gizmoMode == GizmoMode.Path)
        {
            foreach (var path2 in pathes)
            {
                if (path2.Count == 0)
                {
                    return;
                }
                for (int i = 0; i < path2.Count; i++)
                {
                    if (i < path2.Count - 1)
                    {
                        Debug.DrawLine(path2[i].WorldPosition, path2[i + 1].WorldPosition, Color.green);
                        Gizmos.DrawSphere(path2[i].WorldPosition, sphereRadiusBlockage);
                    }
                }

            }        
        }
        if (gizmoMode == GizmoMode.Explored_Node)
        {
            foreach (var node in currentlyExploringNodes)
            {
                if (node.IsWalkable)
                {
                    //Handles.Label(node.WorldPosition, node.Iteration.ToString());//Shows the iteration number ontop of the node
                    Gizmos.color = new Color((float)node.Iteration / (float)BiggestNumNode, (float)node.Iteration / (float)BiggestNumNode, (float)node.Iteration / (float)BiggestNumNode);//Sets our grayscale color to represent out iteration count
                    Gizmos.DrawSphere(node.WorldPosition, sphereRadiusBlockage);//Visualizing each node who is walkable
                }
            }
        }
        if (nodes.GetLength(0) > 0)
        {
            Gizmos.DrawWireSphere(NodeFromWorldPosition(transform.position).WorldPosition , sphereRadiusBlockage + 0.1f);//Visualizing the node of the bot
        }
    }
    private List<Node> GetNeighbouringNodes(Node currentNode)//Get neighbouring nodes of node which are not obstacles 
    {
        List<Node> outnodes = new List<Node>();//The output list of nodes
        Node node;//Just a node var for help
        foreach (var dir in Directions)//Get each direction possible
        {
            node = NodeFromWorldPosition(dir + currentNode.WorldPosition);//Sets the node var for checking if walkable
            if (node.IsWalkable)
            {
               outnodes.Add(node);
            }           
        }
        return outnodes;
    }
    private List<Node> GetPath(Node startNode, Node endNode) 
    {
        #region Pathfinding
        Node currentLoopNode = startNode;//The current node you are currently for the reverse pathfinding
        List<Node> neighbouringNodes = GetNeighbouringNodes(currentLoopNode);//Using the neighbouringNodes variable as advantage to store neighbours
        int lowestCost = 99999;//The lowest cost of each neighbour of currntLoopNode
        Node currentLoopNodeHolder = startNode;//A temporary holder of the node in revers pathfinding
        int i2 = 0;//Placeholder for loop
        List<Node> pathOfNodes = new List<Node>();
        while (currentLoopNode != endNode && i2 < maxIterationReversePath)//While we are not finished
        {
            foreach (var node in neighbouringNodes)//Get each neighbour of current node
            {
                if (node.Iteration < lowestCost)//Get the closest neighbour to end node
                {
                    lowestCost = node.Iteration;
                    currentLoopNodeHolder = node;
                }
            }
            pathOfNodes.Add(currentLoopNode);//Adds the node to get out a list of nodes as path points
            currentLoopNode = currentLoopNodeHolder;//Set the new node to the closest neighbour
            neighbouringNodes = GetNeighbouringNodes(currentLoopNode);//Recalculate neighbours
            i2++;//Adds one to the placeholder as loop number
            if (UseDebug)
            {
                Debug.Log("Iteration reverse : " + i2);
            }
        }
        pathOfNodes.Add(endNode);//Add the end node since the while loop dosent add it itself
        pathOfNodes = SimplifyPath(pathOfNodes);//Simplifying path !!
        return pathOfNodes;//Getting out the path!!
        #endregion
    }
    private void CalculateNodes(Node endNode, Node startNode) 
    {
        #region Calculation of node iterations
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();//Using a stopwatch to get how much time did we spend calculating pathes
        stopwatch.Start();
        finishedCalculations = false;//Just started calculating
        finalnodes = new Node[gridsizeX, gridsizeY];//Resize the final grid array
        List<Node> totalNodes = new List<Node>();//All the nodes in total
        List<Node> oldNodes = new List<Node>();//The list of selected nodes
        List<Node> oldNodesHolder = new List<Node>();//A list of nodes that we will change only at the end of iteration
        oldNodes.Add(endNode);//Add the end node so we can propagate from it
        totalNodes.Add(endNode);
        List<Node> neighbouringNodes;//Neighbouring nodes of a specific current node
        bool hasfinished = false;//Check if we got the start node
        for (int i = 0; i < maxIterations; i++)
        {
            foreach (var node in oldNodes)//Foreach node that we got discovered
            {
                if (node.Iteration == i)
                {
                    neighbouringNodes = GetNeighbouringNodes(node);
                    foreach (var neighbourNode in neighbouringNodes)//Get neighbours of current node
                    {
                        if (!totalNodes.Contains(neighbourNode))//Check if we dont already have the node in our list
                        {
                            totalNodes.Add(neighbourNode);//Adds the new node so we dont overwrite old nodes
                            neighbourNode.Iteration = i + 1;//Sets the number of each neighbouring node to the correct iteration number
                            oldNodesHolder.Add(neighbourNode);
                            finalnodes[neighbourNode.X, neighbourNode.Y] = neighbourNode;
                            //Debug.DrawLine(node.WorldPosition, neighbourNode.WorldPosition, Color.black, 5.0f);
                            if (neighbourNode == startNode)
                            {
                                hasfinished = true;
                            }
                        }
                    }
                }
            }
            currentlyExploringNodes = oldNodesHolder;
            oldNodes = new List<Node>(oldNodesHolder);//I hope that this function will REMOVE every node not like the .Clear() lies. Adds the current nodes to the old nodes
            oldNodesHolder = new List<Node>();//Clear the buffer       
            if (UseDebug)
            {
                Debug.Log("Iteration : " + i + " Nodes : " + oldNodes.Count + 1);
                
            }
            if (hasfinished)
            {
                break;
            }            
        }
        foreach (var node in nodes)
        {
            if (node.Iteration > BiggestNumNode)//Checks if we have the biggest iteration count
            {
                BiggestNumNode = node.Iteration;//Sets the biggset iteration count
            }
        }
        finishedCalculations = true;//Finished calculating
        stopwatch.Stop();
        if (UseDebug)
        {
            Debug.Log("Time spent on calculating pathes : " + stopwatch.ElapsedMilliseconds / 1000.0f);
        }
        foreach (var bot in bots)//Call every bot to tell them that we recalculated
        {
            bot.FindPath();//Recall the pathfinding to call us back again to pathfind the new setted grid pathes
        }
        #endregion
    }
    private List<Node> SimplifyPath(List<Node> startnodes)//Simplifies the path of nodes by looping over them 
    {
        Vector3 lastdirection = Vector3.zero;//The last direction of last node
        List<Node> outnodes = new List<Node>();//The output nodes
        Vector3 currentpos;//Our current node position without counting Y axis
        Vector3 lastpos;//The last node position without counting Y axis
        for (int i = 0; i < startnodes.Count; i++)//Use a for loop so we can see the last node
        {
            if (i > 1)//Checks if our index is more than 1 so we dont get a -1 index
            {
                #region Set current pos
                currentpos = startnodes[i].WorldPosition;//Sets the currentpos to the current node so we can compare
                currentpos.y = 0f;//Reset Y axis so we dont compare from it
                #endregion
                #region Set old pos
                lastpos = startnodes[i - 1].WorldPosition;//Sets the oldpos to the old node so we can compare
                lastpos.y = 0;//Reset Y axis so we dont compare from it
                #endregion
                if (lastdirection != currentpos - lastpos)//Dedect if we have changed direction
                {
                    outnodes.Add(startnodes[i]);//Adds this node to the output nodes
                }
                lastdirection = currentpos - lastpos;//Sets our direction based off the change in position from last node
            }
        }
        if (!outnodes.Contains(startnodes[startnodes.Count - 1]))//Checks if we have the last point so we dont overwrite it
        {
            outnodes.Add(startnodes[startnodes.Count - 1]);//Adds at least the end points so we dont get stuck in front of it
        }
        return outnodes;
    }
}
