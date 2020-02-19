using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Node for flood fill pathfinder
public class Node
{
    public bool IsWalkable;//Is this node walkable ?
    public Vector3 WorldPosition;//The world position of the node
    public int Iteration;//How far are we from the main node ?
    public int X;//The x value in the node array
    public int Y;//The x value in the node array
    public Node(bool _IsWalkable, Vector3 _WorldPosition, int _Iteration, int _X, int _Y) 
    {
        IsWalkable = _IsWalkable;
        WorldPosition = _WorldPosition;
        Iteration = _Iteration;
        X = _X;
        Y = _Y;
    }
}
