using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//Our own custom inspector for the flood fill class !!
[CustomEditor(typeof(FloodFillPathfinder))]
public class FloodFillPathfinderCustomEditor : Editor
{    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FloodFillPathfinder pathfinder = (FloodFillPathfinder)target;
        if (GUILayout.Button("Calculate map"))
        {
            pathfinder.RecalculateMap();//Recalculate map with button
        }
    }
}
