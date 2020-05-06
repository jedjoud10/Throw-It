using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Holds information for the snowball (ex : size, damage, speed) and might randomize them
public class SnowballProperities : NetworkedBehaviour
{
    [Header("Properities")]
    [HideInInspector]
    public float Speed;//Speed force applied at start
    [HideInInspector]
    public float Size;//Size of snowball
    [HideInInspector]
    public int Damage;//Damage applied to someone/something when it collides with the snowball
    [HideInInspector]
    public float RigidbodyForce;//Force applied to every physics object when we hit it


    [Header("Randomness")]
    [HideInInspector()]
    public Vector3 AngularVelocity;//The angular velocity of the snowball at throw
    //Speed force applied at start
    public Vector2 SpeedRandomness;//How much randomness to apply to speed
    //Size of snowball
    public Vector2 SizeRandomness;//How much randomness to apply to speed
    //Damage applied to someone/something when it collides with the snowball
    public Vector2 DamageRandomness;//How much randomness to apply to speed

    public float DamageVelocityWeight;//How much the velocity changes the damage
    public float LifeTime;//time the snowball is allowed to exist
    public float AngularVelocityRange;//How much randomness to apply to angular velocity
    public Vector2 RigidbodyForceRange;//How much randomness to apply to rigidbody hit force
    //Randomizes the values
    private void RandomizeValues() 
    {
        //Randomize
        Speed = Random.Range(SpeedRandomness.x, SpeedRandomness.y);
        Size = Random.Range(SizeRandomness.x, SizeRandomness.y);
        AngularVelocity = Random.insideUnitSphere * AngularVelocityRange;//Random vector for angular velocity
        RigidbodyForce = Random.Range(RigidbodyForceRange.x, RigidbodyForceRange.y);
        //Round to int since damage is int
        Damage = Mathf.RoundToInt(Random.Range(DamageRandomness.x, DamageRandomness.y));
    }
    //Init snowball
    public void InitSnowball() //Called from other scripts to init some properities and change them in some way. Also calles other stuff other from properities
    {
        RandomizeValues();//Randomize snowball values
        SetSnowballWorldProperities();
        Destroy(gameObject, LifeTime);//Destroy snowball if lifetime is excedeed
    }
    //Set snowball game values from variables (Ex : size for local size)
    private void SetSnowballWorldProperities() 
    {
        transform.localScale = new Vector3(Size, Size, Size);//Set world scale with size variable
    }
}
