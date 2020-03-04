using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Adds more fog to the scene the more we are underwater
public class UnderwaterEffectScript : MonoBehaviour
{
    //Colors of fog in two types
    private Color normalColor;
    public Color waterColor;

    //Density of fog in two types
    private float normalDensity;
    public float waterDensity;
    private float currentDensity;
    public float baseDensity;//Density at the moment where you go underwater

    //Water settings
    private float waterHeight;
    public float densityMultiplier;

    private bool changedFog = false;//Used to make do-once function
    // Start is called before the first frame update
    void Start()
    {
        //Setup normal values
        waterHeight = FindObjectOfType<WorldManager>().waterHeight;
        normalColor = RenderSettings.fogColor;
        normalDensity = RenderSettings.fogDensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < waterHeight) //We are underwater (Not fully tho but still)
        {
            //Calculate density of water
            currentDensity = waterDensity * Mathf.Pow(((waterHeight - transform.position.y)/waterHeight), densityMultiplier) + baseDensity;
            //Set underwater fog settings
            RenderSettings.fogColor = waterColor;
            RenderSettings.fogDensity = currentDensity;
            changedFog = true;
        }
        else 
        {
            if (changedFog) 
            {
                changedFog = false;//Do-once
                //Set normal fog settings
                RenderSettings.fogColor = normalColor;
                RenderSettings.fogDensity = normalDensity;
            }
        }
    }
}
