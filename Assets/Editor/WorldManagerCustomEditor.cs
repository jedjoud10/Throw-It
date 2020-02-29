using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(WorldManager))]
public class WorldManagerCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WorldManager worldmanager = (WorldManager)target;
        if (GUILayout.Button("Update World Map"))
        {
            worldmanager.WorldUpdate();
        }
    }
}
