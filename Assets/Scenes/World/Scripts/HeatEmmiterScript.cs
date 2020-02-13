using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//An object emmiting heat
public class HeatEmmiterScript : MonoBehaviour
{
    public float heatEmmision;//How much heat this object emmits
    public float maxEmmisionDistance;//Max distance that we can emmit heat to
    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindObjectOfType<TemperatureScript>().CheckHeatEmmiters();//Check heat emmiters since we were created
    }
    private void OnDestroy()
    {
        GameObject.FindObjectOfType<TemperatureScript>().CheckHeatEmmiters();//Check heat emmiters since we aer going to get destroyed
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
