using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//A pathfinder that uses the A* pathfinding algorithm
public class AStarPathfinder : MonoBehaviour
{
    private AStarNode[,] nodes = new AStarNode[0,0];//Nodes
    [Header("Main settings")]
    [Tooltip("The object you are trying to reach")]
    public Transform endPoint;//The object you are trying to reach
    [Tooltip("The max number of iterations that you are allowed")]
    public int maxIterations;//The max number of iterations that you are allowed

    [Header("Grid settings")]
    [Tooltip("The max number of nodes in the X direction")]
    public int gridsizeX = 0;//The max number of nodes in the X direction
    [Tooltip("The max number of nodes in the Y direction (Z direction in WorldPosition)")]
    public int gridsizeY = 0;//The max number of nodes in the Y direction
    [Tooltip("The scale of the grid")]
    public float gridScale;//The scale of the grid
    [Tooltip("Pathfinder Offset")]
    public Vector2 offset;//Offset in 2d direction
    [Range(1, 10)]
    [Tooltip("How much detail can we allow")]
    public int Resolution = 1;//How much detail can we allow

    [Header("Collisions")]
    [Tooltip("The radius of the sphere to check collisions with other objects")]
    public float sphereRadiusBlockage;//The radius of the sphere to check collisions for
    [Tooltip("Is this walkable terrain ? (USE ONLY WHEN NOT USING WHEIGHTED POINTS)")]
    public Collider terrainCollider;//Walkable terrain the ai can use

    [Tooltip("How to use gizmos")]
    public GizmoMode gizmoMode;//How to use gizmos

    private AStarNode endNode;//The node of the end object
    private List<Vector2Int> directions = new List<Vector2Int>();//Allowed directions to search for neighbours
    private List<List<AStarNode>> overallPaths = new List<List<AStarNode>>(0);//All pathes that we calculated
    private List<AStarNode> exploredNodes = new List<AStarNode>(0);

    private Vector3 basePos;//Some base offset variable
    public enum GizmoMode
    {
        Grid, Path, ExploredNodes, None
    }
    // Start is called before the first frame update
    void Start()
    {
        gridsizeX *= Resolution;//Make more nodes in X
        gridsizeY *= Resolution;//Make more nodes int Y
        gridScale /= Resolution;//Make scale less

        //Make grid of points at start of game
        MakeGrid();
        //Init directions for neighbour node search
        MakeDirections();
    }
    //Generate directions
    public void MakeDirections() 
    {
        directions.Clear();

        //Only 4 directions (Straight lines)
        directions.Add(new Vector2Int(0, 1));
        directions.Add(new Vector2Int(0, -1));
        directions.Add(new Vector2Int(-1, 0));
        directions.Add(new Vector2Int(1, 0));
    }
    //Generate nodes and make properly the grid
    public void MakeGrid() 
    {
        MakeDirections();
        basePos = new Vector3(offset.x * gridScale, 0, offset.y * gridScale) - new Vector3(gridScale * (gridsizeX-1) / 2, 0, gridScale * (gridsizeY-1) / 2);//Setting base offset
        Vector3 pos;//Make a refference of position for later nodes
        nodes = new AStarNode[gridsizeX, gridsizeY];//Resize the grid array
        RaycastHit hit;//A hit so we can check if we hit the terrain collider
        for (int x = 0; x < gridsizeX; x++)//Loop of X
        {
            for (int y = 0; y < gridsizeY; y++)//Loop of Y
            {
                pos = new Vector3(gridScale * x + basePos.x, transform.position.y, gridScale * y + basePos.z);//Set the correct location for next line
                if (terrainCollider != null) //Cast rays from above, if it is terrain then set y so it is walkable part, if not, set y to be not walkable part
                {
                    if (Physics.Raycast(pos, Vector3.down * 10000000, out hit)) //The raycast with the return hit data
                    {
                        if (hit.collider == terrainCollider)//We hit the terrain, make that node walkable
                        {
                            pos.y = hit.point.y + sphereRadiusBlockage * 1.2f;//Multiplied by 1.2 to make it slighty above the ai
                        }
                        else//We did not hit the terrain, make that node unwalkable 
                        {
                            pos.y = hit.point.y;//Exact point of collision. This could be more optimized to directly tell it that it has failed
                        }
                    }
                }

                nodes[x, y] = new AStarNode(!Physics.CheckSphere(pos, sphereRadiusBlockage), pos, x, y);//Set the node at (X, Y) to the coresponding location
            }
        }
        endNode = NodeFromWorldPosition(endPoint.position);//Init end node after grid generation
    }
    //Get neighbouring node
    private AStarNode GetNeighbour(AStarNode a, Vector2Int direction) 
    {
        return nodes[Mathf.Clamp(a.X + direction.x, 0, gridsizeX - 1), Mathf.Clamp(a.Y + direction.y, 0, gridsizeY - 1)];
    }
    private AStarNode NodeFromWorldPosition(Vector3 pos)//Getting a node from grid from world position 
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
    //Get neighbour with lowest FCost by calculating costs
    private AStarNode GetMostPerfomentNode(AStarNode node, AStarNode startNode, AStarNode endNode, List<AStarNode> visitedNodes, List<AStarNode> nodesToBeVisited) 
    {
        int smallestFCost = int.MaxValue;//Smallest possible FCost for current neighbours
        int smallestHCost = int.MaxValue;//Smallest possible HCost when we have two best nodes with low FCosts
        AStarNode bestNode = nodesToBeVisited[0]; //The best node out of all neighbours (Node with lowest FCost)
        AStarNode currentNode;
        for (int i = 0; i < nodesToBeVisited.Count; i++)//Get neighbour in each direction
        {
            currentNode = nodesToBeVisited[i];//Get node to check lowest FCost with

            if(visitedNodes.Contains(currentNode)) continue;//Skip this neighbour since we already explored it           
            if(!currentNode.IsWalkable) continue;//Skip this neighbour since it is not a walkable node  
            //Calculate costs
            currentNode.GCost = ManhattanDistance(currentNode, startNode);
            currentNode.HCost = ManhattanDistance(currentNode, endNode);
            currentNode.FCost = currentNode.GCost + currentNode.HCost;

            if(currentNode.FCost < smallestFCost)//Select best node wich has lowest FCost
            {
                smallestFCost = currentNode.FCost;//Set the new FCost as the smallest FCost to make filter
                bestNode = currentNode;//Use node with lowest FCost value
            }
        }
        exploredNodes.Add(bestNode);
        return bestNode;
    }
    //Get manhattan distance from node A to node B
    private int ManhattanDistance(AStarNode a, AStarNode b) 
    {
        return (Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y))*10;//Manhattan distance between two points
    }
    //Get euclidean distance from node A to node B using pythagorean theorem
    private int EuclideanDistance(AStarNode a, AStarNode b) 
    {
        return Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(a.X - b.X, 2) + Mathf.Pow(a.Y - b.Y, 2))*10);//Euclidean distance between two points
    }
    //Get path for bot
    public List<AStarNode> Pathfind(Vector3 botPosition) 
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();//Using a stopwatch to get how much time did we spent calculating path
        stopwatch.Start();
        AStarNode startNode = NodeFromWorldPosition(botPosition);//Start node / bot position node
        List<AStarNode> visitedNodes = new List<AStarNode>(0);//Vistied nodes
        List<AStarNode> nodesToVisit = new List<AStarNode>(0);//Nodes to be visited
        AStarNode currentNode = startNode;//Current node we are visiting
        List<AStarNode> path = new List<AStarNode>(0);//The output path

        //Init lists
        nodesToVisit.Add(startNode);
        visitedNodes.Add(startNode);

        //First part of pathfinder. Get visited nodes with nodes FCosts
        //Debug.Log("PART 1 A* PATHFINDER");
        for (int i = 0; i < maxIterations; i++)
        {
            if(currentNode != endNode) 
            {
                for(int d = 0; d < directions.Count; d++) 
                {
                    nodesToVisit.Add(GetNeighbour(currentNode, directions[i]));
                }
                currentNode = GetMostPerfomentNode(currentNode, startNode, endNode, visitedNodes, nodesToVisit);//Recurse to get path
                //Debug.Log("Node : " + i + "  FCost : " + currentNode.FCost + " Position : " + "X : " + currentNode.X + "  Y : " + currentNode.Y);
                //Add current node so when we try to get neighbours we dont pass by this one twice
                visitedNodes.Add(currentNode);
                nodesToVisit.Clear();
            }
        }

        currentNode = endNode;//Start from end
        startNode = NodeFromWorldPosition(botPosition);
        visitedNodes.Clear();//Reset visisted nodes since we will be using it
        //Second part of pathfinder. Reversepath from endNode to get path using the neighbouring lowest FCost node
        //Debug.Log("PART 2 A* PATHFINDER");
        for (int i = 0; i < maxIterations; i++)
        {
            if (currentNode != startNode)
            {
                currentNode = currentNode.parent;
                //Debug.Log("Node : " + i + "  FCost : " + currentNode.FCost + " Position : " + "X : " + currentNode.X + "  Y : " + currentNode.Y);

                //Add current node to path
                path.Add(currentNode);
            }
        }
        overallPaths.Add(path);//We calculated one more path
        stopwatch.Stop();
        Debug.Log("Took " + stopwatch.ElapsedMilliseconds/1000.0f + " seconds to calculate a " + gridsizeX + "*" + gridsizeY + " map");
        return path;
    }
    private void OnDrawGizmos()
    {
        if (gizmoMode == GizmoMode.Grid)
        {
            Gizmos.DrawWireCube(new Vector3(offset.x * gridScale, 0, offset.y * gridScale), new Vector3(gridsizeX * gridScale, 1f, gridsizeY * gridScale));//Draw area of pathfinder
            foreach (var node in nodes)
            {
                if (node.IsWalkable)
                {
                    Handles.Label(node.WorldPosition, node.FCost.ToString());//Shows the iteration number ontop of the node
                    Gizmos.DrawCube(node.WorldPosition, new Vector3(sphereRadiusBlockage, sphereRadiusBlockage, sphereRadiusBlockage));//Visualizing each node who is walkable
                }
            }
        }
        if (endNode != null)
        {
            Gizmos.DrawWireSphere(endNode.WorldPosition, sphereRadiusBlockage);//Visualizing each node who is walkable
        }
        if (gizmoMode == GizmoMode.Path)
        {
            foreach (var path2 in overallPaths)
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
                        Handles.Label(path2[i].WorldPosition, path2[i].FCost.ToString());//Shows the iteration number ontop of the node
                    }
                }

            }
        }
        if (gizmoMode == GizmoMode.ExploredNodes)
        {
            foreach (var node in exploredNodes)
            {
                //Gizmos.DrawSphere(node.WorldPosition, sphereRadiusBlockage);
                Handles.Label(node.WorldPosition, node.FCost.ToString());//Shows the iteration number ontop of the node
            }
        }
    }
}
