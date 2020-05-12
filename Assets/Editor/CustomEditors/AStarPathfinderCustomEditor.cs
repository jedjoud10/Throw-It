using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//Our own custom inspector for the flood fill class !!
[CustomEditor(typeof(AStarPathfinder))]
public class AStarPathfinderCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AStarPathfinder pathfinder = (AStarPathfinder)target;
        if (GUILayout.Button("Calculate grid"))
        {
            pathfinder.MakeTerrainGrid();//Recalculate map with button
        }
    }
}
