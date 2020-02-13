using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Player temperature calculations
public class TemperatureScript : MonoBehaviour
{
    private float temperature;//Current body temperature
    private float outsideTemperature;//The current outside temperature
    public float heatDispersion;//How much the outside temperature influences our own body temperature
    private const float targetTemperature = 37.0f;//The temperature that we want to come close to
    public float targetTemperatureSpeed;//The speed at how much were going to get at targetTemperature from current temperature
    private Transform crystal;//De crystale
    private const float crystalTemperature = 37.0f;//The temperature that the crystal emmits at 0 distance
    public float maxCrystalDistanceFalloff;//Name is pretty self-explanatory
    private HeatEmmiterScript[] heatEmmiters;//Objects that emmit heat
    // Start is called before the first frame update
    void Start()
    {
        crystal = GameObject.FindGameObjectWithTag("Objective").transform;//Get crystal from tag
        CheckHeatEmmiters();
    }

    // Update is called once per frame
    void Update()
    {
        outsideTemperature = TemperatureCrystal() + TemperatureHeatEmmiters();//Calculate oustide temperature
        temperature = Mathf.Lerp(temperature, outsideTemperature, heatDispersion * Time.deltaTime);
        temperature = Mathf.Lerp(temperature, targetTemperature, targetTemperatureSpeed * Time.deltaTime);//Get closer to target temperature
    }
    //Temperature relative to distance to crystal
    private float TemperatureCrystal() 
    {
        return ((maxCrystalDistanceFalloff - Vector3.Distance(transform.position, crystal.position))/maxCrystalDistanceFalloff) * crystalTemperature;//Calculate temperature using distance to crystal. Normalize distance then multiply by heat of crystal
    }
    //Temperature relative to closest heat-emmiter
    private float TemperatureHeatEmmiters() 
    {
        float TempHeatEmmiters = 0;
        float playerDist;
        for(int i = 0; i < heatEmmiters.Length; i++) 
        {
            playerDist = Vector3.Distance(transform.position, heatEmmiters[i].transform.position);
            TempHeatEmmiters += heatEmmiters[i].maxEmmisionDistance - playerDist;
        }
        return TempHeatEmmiters;
    }
    //Check if there is any new heat emmiters
    public void CheckHeatEmmiters() 
    {
        heatEmmiters = GameObject.FindObjectsOfType<HeatEmmiterScript>();//Set new array of heat emmiters
    }
}
