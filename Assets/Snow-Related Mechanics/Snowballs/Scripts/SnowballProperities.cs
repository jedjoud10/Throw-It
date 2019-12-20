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
    [Header("Randomness")]
    [Range(0, 2)]
    public float Speed_Randomnes;//How much randomness to apply to speed
    [Range(0, 2)]
    public float Size_Randomnes;//How much randomness to apply to speed
    [Range(0, 2)]
    public float Damage_Randomnes;//How much randomness to apply to speed
    //Randomizes the values
    private void RandomizeValues() 
    {
        Speed += Random.Range(-Speed_Randomnes, Speed_Randomnes) * Speed;
        Size += Random.Range(-Size_Randomnes, Size_Randomnes) * Size;
        //Round to int since damage is int
        Damage += Mathf.RoundToInt(Random.Range(-Damage_Randomnes, Damage_Randomnes) * Damage);
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
