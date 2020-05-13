using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEditor;
//A pathfinder that uses the A* pathfinding algorithm and it is multithreaded
//Will refact and optimize this code later on
public class AStarPathfinder : MonoBehaviour
{
    [Header("Main settings")]
    public int maxIterations;//The max number of iterations that you are allowed
    [Header("Grid settings")]
    public int _gridsizeX = 0;//The max number of nodes in the X direction
    public int _gridsizeY = 0;//The max number of nodes in the Y direction
    public float _gridScale;//The scale of the grid
    public Vector2 _offset;//Offset in 2d direction
    [Range(1, 10)]
    public int Resolution = 1;//How much detail can we allow
    [Header("Collisions")]

    public Collider terrainCollider;//Walkable terrain the ai can use
    public float waterHeight;//Water height
    public float obstacleAvoidanceRadius;//Radius to add to avoid more obstacles
    public float maxSlope;//The maximum height difference between nodes to be considered a walkable node

    public GizmoMode gizmoMode;//How to use gizmos
    //The box size to use for displaying gizmos
    public float gizmoSize;

    private AStarNode endNode;//The node of the end object
    private List<Vector2Int> directions = new List<Vector2Int>();//Allowed directions to search for neighbours
    private List<List<AStarNode>> overallPaths = new List<List<AStarNode>>(0);//All pathes that we calculated
    private List<AStarNode> exploredNodes = new List<AStarNode>(0);
    private AStarNode[,] _nodes;//Nodes that are gong to be fed to Threaded functions
    private int gridsizeX, gridsizeY;//Private gridsize
    private float gridScale;//Private gridscale
    private List<Thread> botThreads = new List<Thread>();//Threads so we can call bots to repathfind if map has changed
    private List<BotPathfinderScript> botPathfinders = new List<BotPathfinderScript>();//List of bots's pathfinders so we can call them when map has changed
    private Vector2 offset;//Private offset so we can change it without getting weird results

    private Vector3 basePos;//Some base offset variable
    public enum GizmoMode
    {
        Grid, None
    }
    // Start is called before the first frame update
    void Start()
    {        
        //Init pathes
        overallPaths.Clear();
    }
    #region Grid & Base
    //Generate directions
    public void MakeDirections() 
    {
        //Straight lines cost us only 1 GCost
        directions.Add(new Vector2Int(0, 1));
        directions.Add(new Vector2Int(0, -1));
        directions.Add(new Vector2Int(-1, 0));
        directions.Add(new Vector2Int(1, 0));

        //Diagonals cost us 2 GCosts        
        directions.Add(new Vector2Int(1, 1));
        directions.Add(new Vector2Int(1, -1));
        directions.Add(new Vector2Int(-1, 1));
        directions.Add(new Vector2Int(1, -1));        
    }    
    //Generate nodes based off terrain to be able to use it later
    //Only run this at start of game
    public void MakeTerrainGrid() 
    {        
        MakeDirections();
        exploredNodes.Clear();//Reset debug
        gridsizeX = _gridsizeX * Resolution;//Make more nodes in X
        gridsizeY = _gridsizeY * Resolution;//Make more nodes int Y
        gridScale = _gridScale / Resolution;//Make scale less
        offset = _offset * Resolution;//Update offset
        basePos = new Vector3(offset.x * gridScale, 0, offset.y * gridScale) - new Vector3(gridScale * (gridsizeX-1) / 2, 0, gridScale * (gridsizeY-1) / 2);//Setting base offset
        Vector3 pos;//Make a reference of position for later nodes
        RaycastHit hit;//A hit so we can check if we hit the terrain collider
        bool walkable = false;//Is the current node walkable
        _nodes = new AStarNode[gridsizeX, gridsizeY];//Nodes we are going to give to the new threaded function
        float height = transform.position.y;//The y value of the transform.position
        for (int x = 0; x < gridsizeX; x++)//Loop of X
        {
            for (int y = 0; y < gridsizeY; y++)//Loop of Y
            {
                pos.x = gridScale * x + basePos.x;
                pos.z = gridScale * y + basePos.z;
                pos.y = height;
                if (Physics.Raycast(pos, Vector3.down * 10000000, out hit)) //The raycast with the return hit data
                {
                    pos = hit.point + Vector3.up;//Set new node position    
                }
                _nodes[x, y] = new AStarNode(walkable, pos, x, y);//Set the node at (X, Y) to the coresponding location
            }
        }
    }
    //The terrain obstacles have changed, so recalculate grid using multithreading
    private void MakeGridThread(PathfindObstacle[] obstacles) 
    {
        for (int x = 0; x < gridsizeX; x++)
        {
            for (int y = 0; y < gridsizeY; y++)
            {
                //Check if this node is walkable
                //If is it water ?
                if (_nodes[x, y].WorldPosition.y < waterHeight) 
                {
                    _nodes[x, y].IsWalkable = false;//cannot walk on water
                    continue;//Skiping since it is already not walkable
                }
                //Is it too steep ?
                if (GetSlope(_nodes[x, y], _nodes) > maxSlope) 
                {
                    _nodes[x, y].IsWalkable = false;//cannot walk since the node is too steep
                    continue;//Skiping since it is already not walkable
                }
                _nodes[x, y].IsWalkable = true;//Init state since we are doing another loop, so we must have like a rest/reset state
                for (int o = 0; o < obstacles.Length; o++) 
                {
                    if(DistanceBox(_nodes[x, y].WorldPosition, obstacles[o].Bounds, obstacles[o].Position, obstacleAvoidanceRadius)) 
                    {
                        _nodes[x, y].IsWalkable = false;
                        break;//We finished the task early since we found an obstacle already. No need to continue
                    }
                } 
            }
        }
        Debug.Log("Finished recalculating grid thread");
    }
    //Gets slope of specific node on grid based off y position value
    private float GetSlope(AStarNode node, AStarNode[,] nodes) 
    {
        List<float> heights = new List<float>();
        //Calculate max and min points from 4 neighbouring nodes
        heights.Add(GetNeighbour(node, 0, 1, nodes).WorldPosition.y);
        heights.Add(GetNeighbour(node, 0, -1, nodes).WorldPosition.y);
        heights.Add(GetNeighbour(node, 1, 0, nodes).WorldPosition.y);
        heights.Add(GetNeighbour(node, -1, 0, nodes).WorldPosition.y);

        return Mathf.Max(heights.ToArray()) - Mathf.Min(heights.ToArray());//Calculate change in altitude between highest point and lowest point and use that as slope value
    }
    //Called from outside scripts to recalulate the grid using multithreading
    public void MakeGrid() 
    {
        PathfindObstacle[] obstacles = FindObjectsOfType<PathfindObstacle>();
        Thread gridThread = new Thread(() => MakeGridThread(obstacles));
        gridThread.Start();
    }   
    //Get if point is inside box with bounds. boxBounds is full length of a side
    private bool DistanceBox(Vector3 pointPos, Vector3 boxBounds, Vector3 boxPos, float obstacleAvoindanceDistance) 
    {
        bool isInsideBox = false;//Init value

        pointPos -= boxPos;//Make origin at (0, 0)

        //Edges
        //Divide by 2.0 to get the extent and not full length
        bool x = Mathf.Abs(pointPos.x) < (boxBounds.x / 2.0f) + obstacleAvoindanceDistance;
        bool y = Mathf.Abs(pointPos.y) < (boxBounds.y / 2.0f) + obstacleAvoindanceDistance;
        bool z = Mathf.Abs(pointPos.z) < (boxBounds.z / 2.0f) + obstacleAvoindanceDistance;

        isInsideBox = x && z;
        return isInsideBox;
    }
    #endregion

    #region Pathfinding
    //Get neighbouring node
    private AStarNode GetNeighbour(AStarNode a, Vector2Int direction, AStarNode[,] nodes) 
    {
        return nodes[Mathf.Clamp(a.X + direction.x, 0, gridsizeX - 1), Mathf.Clamp(a.Y + direction.y, 0, gridsizeY - 1)];
    }
    //Get neighbouring node (using int as direction)
    private AStarNode GetNeighbour(AStarNode a, int xOffset, int yOffset, AStarNode[,] nodes)
    {
        return nodes[Mathf.Clamp(a.X + xOffset, 0, gridsizeX - 1), Mathf.Clamp(a.Y + yOffset, 0, gridsizeY - 1)];
    }
    private AStarNode NodeFromWorldPosition(Vector3 pos, AStarNode[,] nodes)//Getting a node from grid from world position 
    {
        pos -= basePos;//Remove the base pos so we have correct mesurements with correct offset
        #region Snapping to grid
        pos.x = pos.x / gridScale;
        pos.z = pos.z / gridScale;
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.z);
        #endregion
        x = Mathf.Clamp(x, 0, gridsizeX - 1);//Clamping so the value cant be out of index
        y = Mathf.Clamp(y, 0, gridsizeY - 1);        
        return nodes[x, y];
    }
    //Get manhattan distance from node A to node B
    private int ManhattanDistance(AStarNode a, AStarNode b) 
    {
        return (Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y));//Manhattan distance between two points
    }
    //Get euclidean distance from node A to node B using pythagorean theorem
    private int EuclideanDistance(AStarNode a, AStarNode b) 
    {
        return Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(a.X - b.X, 2) + Mathf.Pow(a.Y - b.Y, 2))*10);//Euclidean distance between two points
    }
    //Called by external scripts and start mutlithreaded method
    public void Pathfind(Vector3 botPosition, Vector3 endPostition, BotPathfinderScript bot)
    {
        if (botPathfinders.Contains(bot))
        {
            RemoveFromQueue(bot);
        }
        Thread thread = new Thread(() => PathfindThread(botPosition, endPostition, bot));
        thread.Start();//Start the new thread
        botThreads.Add(thread);
        botPathfinders.Add(bot);        
    }  
    //Removes bot pathfidner from list since the bot is dead
    public void RemoveFromQueue(BotPathfinderScript bot) 
    {
        if (!botPathfinders.Contains(bot)) return;
        botThreads[botPathfinders.IndexOf(bot)].Abort();
        botThreads.RemoveAt(botPathfinders.IndexOf(bot));
        botPathfinders.Remove(bot);//de remov    
    }
    //Get path for bot. This is multithreaded
    private void PathfindThread(Vector3 botPosition, Vector3 endPostition, BotPathfinderScript bot) 
    {
        AStarNode[,] nodes = _nodes;
        if (nodes == null) return;
        endNode = NodeFromWorldPosition(endPostition, nodes);
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();//Using a stopwatch to get how much time did we spent calculating path
        stopwatch.Start();
        AStarNode startNode = NodeFromWorldPosition(botPosition, nodes);//Start node / bot position node
        List<AStarNode> visitedNodes = new List<AStarNode>(0);//Vistied nodes
        List<AStarNode> nodesToVisit = new List<AStarNode>(0);//Nodes to be visited
        AStarNode currentNode = startNode;//Current node we are visiting
        currentNode.GCost = 0;//Init GCost
        startNode.parent = null;
        List<AStarNode> path = new List<AStarNode>(0);//The output path
        int lowestFCost = int.MaxValue;//Lowest fcost when looping over nodes
        int lowestHCost = int.MaxValue;//Lowest hcost when looping over best nodes with same FCost
        int GCostMovePenalty = 1;//How much do we add GCost to the current node to calculate the neighbour's GCost

        AStarNode currentNeighbour;//The current neighbour node
        //First part of pathfinder. Get visited nodes with nodes's FCosts
        //Debug.Log("PART 1 A* PATHFINDER");
        visitedNodes.Add(currentNode);//Add start node so we dont pass by it again
        for (int i = 0; i < maxIterations; i++)
        {
            if(currentNode != endNode) 
            {
                lowestFCost = int.MaxValue;//Reset lowest FCost so we can do node filtering
                lowestHCost = int.MaxValue;//Reset lowest HCost so we can do second node filtering

                //Get neighbours of current node
                for (int d = 0; d < directions.Count; d++) //Each direction
                {
                    //Set GCost penalty to be 2 if we are in a diagonal direction and to be only 1 if we are in a straight direction
                    if (d > 3) GCostMovePenalty = 2;
                    else GCostMovePenalty = 1;                   
                    currentNeighbour = GetNeighbour(currentNode, directions[d], nodes);//Get neighbour for this node iteration with direction
                    if (visitedNodes.Contains(currentNeighbour)) continue;//Skip to next neighbour because we already visited the curernt one and we already passed by it
                    if (!currentNeighbour.IsWalkable) continue;//Skip to next neighbour since we cannot walk on this one
                    if(nodesToVisit.Contains(currentNeighbour)) //If this node is already going to be visited
                    {
                        //If the path that this node will take if it passes by our current node is shorter, then take that route
                        if (currentNode.GCost + GCostMovePenalty < currentNeighbour.GCost)
                        {
                            currentNeighbour.parent = currentNode;
                            //Recalculate costs
                            currentNeighbour.GCost = currentNode.GCost + GCostMovePenalty;//Calculate path legnth/GCost
                            currentNeighbour.FCost = currentNeighbour.GCost + currentNeighbour.HCost;//Calculate FCost
                        }
                    }
                    else
                    {
                        currentNeighbour.parent = currentNode;
                        currentNeighbour.GCost = currentNode.GCost + GCostMovePenalty;//Calculate path legnth/GCost
                        currentNeighbour.HCost = ManhattanDistance(currentNeighbour, endNode);//Calculate Heuristic Cost
                        currentNeighbour.FCost = currentNeighbour.GCost + currentNeighbour.HCost;//Calculate FCost
                        nodesToVisit.Add(currentNeighbour);                        
                    }
                }
                
                //Get best node
                for(int n = 0; n < nodesToVisit.Count; n++) //Loop over all nodes to find one with lowest fcost to visit
                {
                    if(nodesToVisit[n].FCost < lowestFCost)//Filter out node with lowest FCost
                    {
                        lowestFCost = nodesToVisit[n].FCost;
                        lowestHCost = nodesToVisit[n].HCost;
                        currentNode = nodesToVisit[n];//Lowest FCost node saved to variable for next iteration
                    }
                    else if(nodesToVisit[n].FCost == lowestFCost && nodesToVisit[n].HCost < lowestHCost) 
                    {
                        //Getting lowest HCost if two or more nodes have the same FCost
                        lowestHCost = nodesToVisit[n].HCost;
                        currentNode = nodesToVisit[n];//Lowest HCost node saved to variable for next iteration
                    }
                }

                //Do modifications after the loop because that loop is used just to get the best node, and not to change the properities of each node
                if (!exploredNodes.Contains(currentNode)) exploredNodes.Add(currentNode);
                visitedNodes.Add(currentNode);//Add the best node to visitedNodes so we dont pass by it again
                nodesToVisit.Remove(currentNode);//We are not going to revisit the best node
            }
            else 
            {
                Debug.Log("Final iteration is " + i);
                break;
            }
        }

        currentNode = endNode;//Start from end
        //Second part of pathfinder. Reversepath from endNode to get path using the neighbouring lowest FCost node
        //Debug.Log("PART 2 A* PATHFINDER");
        path.Add(endNode);
        for (int i = 0; i < maxIterations; i++)
        {
            if (currentNode.parent != null)
            {
                currentNode = currentNode.parent;
                //Add current node to path
                path.Add(currentNode);
            }
            else 
            {
                //Debug.Log("Final reverse iteration is " + i);
                break;
            }
        }
        
        overallPaths.Add(path);//We calculated one more path
        stopwatch.Stop();
        Debug.Log("Took " + stopwatch.ElapsedMilliseconds/1000.0f + " seconds to calculate path");
        bot.SetDestinationPoints(OptimizePath(TransformPathToPoints(path)));
    }
    //Transforms a path to 3D vector points
    private List<Vector3> TransformPathToPoints(List<AStarNode> path) 
    {
        List<Vector3> points = new List<Vector3>(0);//The 3D vector points
        for(int i = 0; i < path.Count; i++)
        {
            points.Add(path[i].WorldPosition);//Get world position of path nodes
        }
        points.Reverse();
        return points;
    }
    //Optimizes path
    private List<Vector3> OptimizePath(List<Vector3> points) 
    {
        //Method used :
        //Loop over all nodes, check current direction from last node to current,
        //and dedect if we changed directions, if we did, then add that current node
        //to the outPoints list
        List<Vector3> outPoints = new List<Vector3>();//Init output value
        Vector3 currentDirection;//Current direction from last point to current point
        Vector3 lastDirection = Vector3.zero;//Direction at the last iteration
        Vector3 lastPoint = points[0];//Point at last iteration to get direction from

        for(int i = 0; i < points.Count; i++) 
        {
            currentDirection = points[i] - lastPoint;//Calculate current direction
            currentDirection.y = 0.0f;
            if (Vector3.Distance(lastDirection, currentDirection) > 0.2f) outPoints.Add(lastPoint);//Add last point to final points
            lastDirection = currentDirection;//Init value for next iteration
            lastPoint = points[i];//Init last point for next iteration
        }
        outPoints.Add(points[points.Count - 1]);
        return outPoints;
    }
    #endregion

    private void OnDrawGizmos()
    {
        gridsizeX = _gridsizeX * Resolution;//Make more nodes in X
        gridsizeY = _gridsizeY * Resolution;//Make more nodes int Y
        gridScale = _gridScale / Resolution;//Make scale less
        offset = _offset * Resolution;//Update offset
        Gizmos.DrawWireCube(new Vector3(offset.x * gridScale, transform.position.y, offset.y * gridScale), new Vector3(gridsizeX * gridScale, 1f, gridsizeY * gridScale));//Draw area of pathfinder
        if (gizmoMode == GizmoMode.Grid && endNode != null)
        {
            Gizmos.DrawWireSphere(endNode.WorldPosition, 1.0f);
            if (_nodes == null) return;
            foreach (var node in _nodes)
            {
                if (node.IsWalkable)
                {
                    //Handles.Label(node.WorldPosition, node.Iteration.ToString());//Shows the iteration number ontop of the node
                    Gizmos.DrawCube(node.WorldPosition, new Vector3(gizmoSize, gizmoSize, gizmoSize));//Visualizing each node who is walkable
                }
            }
        }        
    }
}
