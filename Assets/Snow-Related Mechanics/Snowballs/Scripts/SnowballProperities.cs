using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Holds information for the snowball (ex : size, damage, speed) and might randomize them
public class SnowballProperities : MonoBehaviour
{
    [Header("Properities")]
    public float Speed;//Speed force applied at start
    public float Size;//Size of snowball
    public int Damage;//Damage applied to someone/something when it collides with the snowball
    [HideInInspector()]
    public Vector3 AngularVelocity;//The angular velocity of the snowball at throw
    [Header("Randomness")]
    [Range(0, 2)]
    public float SpeedRandomness;//How much randomness to apply to speed
    [Range(0, 2)]
    public float SizeRandomness;//How much randomness to apply to speed
    [Range(0, 2)]
    public float DamageRandomness;//How much randomness to apply to speed
    [Range(0, 100)]
    public float AngularVelocityRange; 
    //Randomizes the values
    private void RandomizeValues() 
    {
        //Randomize
        Speed += Random.Range(-SpeedRandomness, SpeedRandomness) * Speed;
        Size += Random.Range(-SizeRandomness, SizeRandomness) * Size;
        AngularVelocity = Random.insideUnitSphere * AngularVelocityRange;//Random vector for angular velocity
        //Round to int since damage is int
        Damage += Mathf.RoundToInt(Random.Range(-DamageRandomness, DamageRandomness) * Damage);
    }
    //Init snowball
    public void InitSnowball() //Called from other scripts to init some properities and change them in some way. Also calles other stuff other from properities
    {
        RandomizeValues();//Randomize snowball values
        SetSnowballWorldProperities();
    }
    //Set snowball game values from variables (Ex : size for local size)
    private void SetSnowballWorldProperities() 
    {
        transform.localScale = new Vector3(Size, Size, Size);//Set world scale with size variable
    }
}
