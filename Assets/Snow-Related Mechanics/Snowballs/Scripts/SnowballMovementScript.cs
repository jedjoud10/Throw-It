using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
//Controls the behaviour of the snowball movement and collisions
[RequireComponent(typeof(SnowballPropertiesScript))]
public class SnowballMovementScript : NetworkedBehaviour
{
    private int Damage;//The base damage the snowball can do
    private float RigidbodyForce;//Force applied to every physics object when we hit it
    private Rigidbody rigidBody;//The rigidbody of the snowball
    private float DamageVelocityWeight = 1;//How much the velocity changes the damage
    private Vector3 LastVelocity;//Last velocity measurement since OnCollisionEnter is called one frame after physics, thius giving us weird yeeting of the physics parts
    // Start is called before the first frame update
    public void InitSnowball(float _Speed, SnowballThrowingScript _throwingScript, bool randomize)//Multiply our base values by those arguments
    {
        #region Setup properities
        SnowballPropertiesScript properities = GetComponent<SnowballPropertiesScript>();//Gets properities from script
        properities.InitSnowball(randomize);//Init snowball properities
        float Speed = properities.Speed;//Use one time float since we wont reuse this float later on
        Damage = properities.Damage;

        //Setup variables from SnowballProperities script
        Vector3 AngularVelocity = properities.AngularVelocity;
        RigidbodyForce = properities.RigidbodyForce;
        DamageVelocityWeight = properities.DamageVelocityWeight;

        #endregion
        #region Setup Rigidbody
        rigidBody = GetComponent<Rigidbody>();//Sets the rigidbody to our own
        rigidBody.AddForce(rigidBody.transform.forward * Speed * _Speed);//Pushes the snowball in the direction it is currently heading. Multiply the speed by the _speed argument so we can change how fast we can throw it in the SnowballThrowingScript.cs script
        rigidBody.transform.eulerAngles = AngularVelocity;//Set rotation
        rigidBody.AddTorque(AngularVelocity);//Add angular velocity force
        #endregion
    }
    //When we hit an object (Ex. : Player, Snowman, Ground)
    //TODO: Make this collision handling client-trusted instead of handling it on the server
    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            Damage *= Mathf.RoundToInt(LastVelocity.magnitude * DamageVelocityWeight);//Take account velocity to damage, so if the snowball is fast, it does more damage
            GameObject otherobject = collision.gameObject;//The colision gameobject  
            //---Collision code handling---\\
            if (otherobject.GetComponent<BotHealthScript>() != null)
            {
                //Damage the hit bot
                otherobject.GetComponent<BotHealthScript>().DamageBot(Damage);
            }
            if (otherobject.GetComponent<PlayerHealthScript>() != null)
            {
                //Damage player
                otherobject.GetComponent<PlayerHealthScript>().DamagePlayer(Damage);
            }
            if (otherobject.GetComponent<BotPhysicsScript>() != null) otherobject.GetComponent<BotPhysicsScript>().RemoveJoint(LastVelocity * RigidbodyForce, rigidBody.position, Damage);
        }
        Destroy(gameObject);//Destroys the snowball
    }
    //Update method
    private void Update()
    {
        if(IsServer) LastVelocity = rigidBody.velocity;//Set last velocity because of yeeting bug
        /*
         We have the yeeting bug because the OnCollisionEnter is called one frame after the physics calcualtion are, so when the snowball hits a physics part its 
         actually one frame late, and it has time to rebounce in that frame, so the velocity we were getting before was the velocity of the snowball after one 
         physics frame, which is after it bounces of a little. That is the cause of the yeeting weird direction problem
        */
    }
}
