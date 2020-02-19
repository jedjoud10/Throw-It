using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A node for the A* pathfinding algorithm
public class AStarNode
{
    public bool IsWalkable;//Is this node walkable ?
    public Vector3 WorldPosition;//The world position of the node

    public int GCost;//Distance from current node to start node
    public int HCost;//Heuristic, distance estimated from current node to end node
    public int FCost;//GCost + HCost
    public AStarNode parent;//Parent node

    public int X;//The x value in the node array
    public int Y;//The x value in the node array
    public AStarNode(bool _IsWalkable, Vector3 _WorldPosition, int _X, int _Y)
    {
        //Init node
        IsWalkable = _IsWalkable;
        WorldPosition = _WorldPosition;
        //Init FCost to max value since the pathfinder will try to go to lowest fcost node, even though it was not visited yet
        FCost = int.MaxValue;
        X = _X;
        Y = _Y;
    }
}
