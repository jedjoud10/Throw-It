using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Obstalce for the A* pathfinding algorithm
public class PathfindObstacle : MonoBehaviour
{
    [HideInInspector]
    public Vector3 Position;//The position of this gameObject transform
    public Vector2 Bounds = new Vector2(0, 0);//Radius to avoid this obstalce
    // Start is called before the first frame update
    void Start()
    {
        Position = transform.position;//Init position
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(Bounds.x, 2.0f, Bounds.y));
    }
}
