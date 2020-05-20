using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//Our own custom inspector for the flood fill class !!
[CustomEditor(typeof(AStarPathfinderScript))]
public class AStarPathfinderCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AStarPathfinderScript pathfinder = (AStarPathfinderScript)target;
        if (GUILayout.Button("Calculate grid"))
        {
            pathfinder.MakeTerrainGrid();//Recalculate map with button
        }
    }
}
