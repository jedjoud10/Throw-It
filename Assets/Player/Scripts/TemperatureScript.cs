using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Player temperature calculations and UI temeprature stuff
public class TemperatureScript : MonoBehaviour
{
    private float temperature;//Current body temperature
    private float outsideTemperature;//The current outside temperature
    public float heatDispersion;//How much the outside temperature influences our own body temperature
    private const float targetTemperature = 37.0f;//The temperature that we want to come close to
    public float targetTemperatureSpeed;//The speed at how much were going to get at targetTemperature from current temperature
    private const float minTemperature = 35.5f;//Minimum temperature before we start getting damage
    private const int damageHypothermia = 10;//Damage you receive when your temperature is below minTemperature (Hyporthermia)
    private float timeInHypothermia = 0;//Our current time in hyporthermia. Will reset if value exceeds hypothermiaRese
    private const float hypothermiaReset = 1.0f;//If our timeInHypothermia is larger then this, then reset timeInHypothermia to 0 and apply damage
    private Transform crystal;//De crystale
    private const float waterTemperature = -100.0f;//How much temperature to remove from outsideTemperature when we are in water
    public float waterLevel;//The water level
    private HeatEmmiterScript[] heatEmmiters = new HeatEmmiterScript[0];//Objects that emmit heat
    public Text temperatureText;//The text for showing temperature
    public Slider temperatureBar;//Temperature bar

    private HealthScript healthScript;//health script of player
    // Start is called before the first frame update
    void Start()
    {
        //Set temperatures at normal level since we dont want to die in the first seconds of the game
        temperature = targetTemperature;
        outsideTemperature = targetTemperature;

        crystal = GameObject.FindGameObjectWithTag("Objective").transform;//Get crystal from tag
        healthScript = GetComponent<HealthScript>();//Init health script
        heatEmmiters = new HeatEmmiterScript[0];
        CheckHeatEmmiters();
        DelayedCheckHeatEmmiters(5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        outsideTemperature = TemperatureHeatEmmiters() + TemperatureWater();//Calculate oustide temperature
        temperature = Mathf.Lerp(temperature, outsideTemperature, heatDispersion * Time.deltaTime);
        temperature = Mathf.Lerp(temperature, targetTemperature, targetTemperatureSpeed * Time.deltaTime);//Get closer to target temperature
        temperatureText.text = "Temperature : " + temperature.ToString("F2");
        temperatureBar.value = Mathf.InverseLerp(minTemperature, targetTemperature, temperature);//set temperature bar
        if(temperature < minTemperature) //Hypothermia
        {
            timeInHypothermia += Time.deltaTime;//Update timeInHyperthermia
            if(timeInHypothermia > hypothermiaReset)//Reset timeInHypothermia
            {
                timeInHypothermia = 0;
                healthScript.Damage(damageHypothermia);//Deal damage at this precice interval
            }            
        }
    }
    //Temperature relative to closest heat-emmiter. This also includes the crystal as a heat emmiter
    private float TemperatureHeatEmmiters() 
    {
        float TempHeatEmmiters = 0;
        float playerDist;
        for(int i = 0; i < heatEmmiters.Length; i++) 
        {
            playerDist = Vector3.Distance(transform.position, heatEmmiters[i].transform.position);
            TempHeatEmmiters += Mathf.Max((heatEmmiters[i].maxEmmisionDistance - playerDist) / heatEmmiters[i].maxEmmisionDistance, heatEmmiters[i].minHeat) * heatEmmiters[i].heatEmmision;//Add temperature of all close heating objects
        }
        return TempHeatEmmiters;
    }
    //Temperature relative to if we are in water
    private float TemperatureWater() 
    {
        return Mathf.Min((waterLevel - transform.position.y) / waterLevel * waterTemperature, 0);//Water coldness clamped between -Infinity to 0 so we cant get heat from water if we are above, only remove heat if we are below
    }
    //Check if there is any new heat emmiters
    public void CheckHeatEmmiters() 
    {
        heatEmmiters = GameObject.FindObjectsOfType<HeatEmmiterScript>();//Set new array of heat emmiters
    }
    //Delayed check heat emmiters
    public void DelayedCheckHeatEmmiters(float delay) 
    {
        Invoke("CheckHeatEmmiters", delay);
    }
    void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            float space = 15;
            float offset = space * 16;
            GUI.Box(new Rect(0, offset, 250, space * 10), "");
            GUI.Label(new Rect(0, offset, 500, 100), "TemperatureScript : ");
            GUI.Label(new Rect(10, offset + space * 1, 500, 100), "Temperatures : ");
            GUI.Label(new Rect(30, offset + space * 2, 500, 100), "Outside Temperature :" + outsideTemperature.ToString("F3"));
            GUI.Label(new Rect(30, offset + space * 3, 500, 100), "Body Temperature :" + temperature.ToString("F3"));
            GUI.Label(new Rect(30, offset + space * 4, 500, 100), "Time in hypothermia :" + timeInHypothermia.ToString("F3"));
            GUI.Label(new Rect(30, offset + space * 6, 500, 100), "Current Water Temp :" + TemperatureWater().ToString("F3"));
            GUI.Label(new Rect(30, offset + space * 7, 500, 100), "Current HeatEmmiters Temp :" + TemperatureHeatEmmiters().ToString("F3"));
            GUI.Label(new Rect(30, offset + space * 8, 500, 100), "HeatEmmiters Ammount :" + heatEmmiters.Length);
        }
    }
    //Debugging GUI stuff

}
