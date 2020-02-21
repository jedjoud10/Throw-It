using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Obstalce for the A* pathfinding algorithm
public class PathfindObstacle : MonoBehaviour
{
    [HideInInspector]
    public Vector3 Position;//The position of this gameObject transform
    public Vector3 Bounds;//Bounds of the box
    public Vector3 Offset;//The offset of the bounds
    // Start is called before the first frame update
    void Start()
    {
        Position = transform.position;//Init position
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Offset, Bounds);
    }
}
