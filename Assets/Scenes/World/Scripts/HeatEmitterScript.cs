using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//An object emmiting heat
public class HeatEmitterScript : MonoBehaviour
{
    public float heatEmmission;//Heat emmision at distance 0 from this objec
    public float minHeat;//Minimum heat emmision : Mathf.Min(heatEmmission, minHeat)
    public float maxEmmisionDistance;//Maximum distance that we emmit heat from
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
