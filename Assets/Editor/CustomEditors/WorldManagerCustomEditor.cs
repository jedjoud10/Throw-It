using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(WorldManagerScript))]
public class WorldManagerCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WorldManagerScript worldmanager = (WorldManagerScript)target;
        if (GUILayout.Button("Update World Map"))
        {
            worldmanager.WorldUpdate();
        }
    }
}
