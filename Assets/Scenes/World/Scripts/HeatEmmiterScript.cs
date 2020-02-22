using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//An object emmiting heat
public class HeatEmmiterScript : MonoBehaviour
{
    public float heatEmmision;//How much heat this object emmits
    public float maxEmmisionDistance;//Minimum distance that we emmit heat to
    public float minHeat;//Minimum heat at any point in space
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxEmmisionDistance);
    }
}
