using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Obstalce for the A* pathfinding algorithm
public class PathfindObstacleScript : MonoBehaviour
{
    [HideInInspector]
    public Vector3 Position;//The position of this gameObject transform
    [HideInInspector]
    public Vector3 Bounds = new Vector3(0, 0, 0);//The actual bounds of the obstalce (scaled by the transform.scale)
    public Vector3 ObstacleBounds = new Vector3(0, 0, 0);//The bounds of this obstalce (unscaled)
    // Start is called before the first frame update
    void Start()
    {
        //Scale by transform.scale
        Bounds = Vector3.Scale(ObstacleBounds, transform.localScale);
        Position = transform.position;//Init position
    }
    private void OnDrawGizmos()
    {
        //Scale by transform.scale
        Bounds = Vector3.Scale(ObstacleBounds, transform.localScale);
        Gizmos.DrawWireCube(transform.position, Bounds);
    }
}
