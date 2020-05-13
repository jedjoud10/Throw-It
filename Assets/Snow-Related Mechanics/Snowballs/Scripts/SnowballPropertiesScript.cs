using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Holds information for the snowball (ex : size, damage, speed) and might randomize them
public class SnowballPropertiesScript : NetworkedBehaviour
{
    [Header("Properities")]
    [HideInInspector]
    public float speed;//Speed force applied at start
    [HideInInspector]
    public float size;//Size of snowball
    [HideInInspector]
    public int damage;//Damage applied to someone/something when it collides with the snowball
    [HideInInspector]
    public float rigidbodyForce;//Force applied to every physics object when we hit it
    [HideInInspector()]
    public Vector3 angularVelocity;//The angular velocity of the snowball at throw 

    [Header("Randomness")]
    //Speed force applied at start
    public Vector2 speedRandomness;//How much randomness to apply to speed
    //Size of snowball
    public Vector2 sizeRandomness;//How much randomness to apply to speed
    //Damage applied to someone/something when it collides with the snowball
    public Vector2 damageRandomness;//How much randomness to apply to speed

    public float damageVelocityWeight;//How much the velocity changes the damage
    public float lifetime;//time the snowball is allowed to exist
    public float angularVelocityRange;//How much randomness to apply to angular velocity
    public Vector2 rigidbodyForceRange;//How much randomness to apply to rigidbody hit force
    //Randomizes the values
    private void RandomizeValues() 
    {
        //Randomize
        speed = Random.Range(speedRandomness.x, speedRandomness.y);
        size = Random.Range(sizeRandomness.x, sizeRandomness.y);
        angularVelocity = Random.insideUnitSphere * angularVelocityRange;//Random vector for angular velocity
        rigidbodyForce = Random.Range(rigidbodyForceRange.x, rigidbodyForceRange.y);
        //Round to int since damage is int
        damage = Mathf.RoundToInt(Random.Range(damageRandomness.x, damageRandomness.y));
    }
    //Set snowball values
    public void SetValues(float _speed, float _size, Vector3 _angularVelocity, float _rigidbodyForce, int _damage) 
    {
        //Set new variables using the struct
        speed = _speed;
        size = _size;
        angularVelocity = _angularVelocity;
        rigidbodyForce = _rigidbodyForce;
        damage = _damage;
    }
    //Init snowball
    public void InitSnowball(bool randomize)//Called from other scripts to init some properities and change them in some way. Also calles other stuff other from properities
    {
        if (randomize)
        {
            RandomizeValues();//Randomize snowball values            
        }
        SetSnowballWorldProperities();
        Destroy(gameObject, lifetime);//Destroy snowball if lifetime is excedeed
    }
    //Set snowball game values from variables (Ex : size for local size)
    private void SetSnowballWorldProperities() 
    {
        transform.localScale = new Vector3(size, size, size);//Set world scale with size variable
    }
}
