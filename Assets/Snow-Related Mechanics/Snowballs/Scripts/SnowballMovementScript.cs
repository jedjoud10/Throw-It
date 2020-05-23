using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
//Controls the behaviour of the snowball movement and collisions
[RequireComponent(typeof(SnowballPropertiesScript))]
public class SnowballMovementScript : NetworkedBehaviour
{
    private int damage;//The base damage the snowball can do
    private float rigidbodyForce;//Force applied to every physics object when we hit it
    private Rigidbody rigidBody;//The rigidbody of the snowball
    private float damageVelocityWeight = 1;//How much the velocity changes the damage
    private Vector3 lastVelocity;//Last velocity measurement since OnCollisionEnter is called one frame after physics, thius giving us weird yeeting of the physics parts
    SnowballPropertiesScript properties;//Properties for this snowball
    // Start is called before the first frame update
    //Inits a snowball with a player nickname string
    public void InitSnowball(float speedFactor)
    {
        #region Setup properities
        properties = GetComponent<SnowballPropertiesScript>();//Gets properities from script
        float Speed = properties.speed;//Use one time float since we wont reuse this float later on
        damage = properties.damage;

        //Setup variables from SnowballProperities script
        Vector3 angularVelocity = properties.angularVelocity;
        rigidbodyForce = properties.rigidbodyForce;
        damageVelocityWeight = properties.damageVelocityWeight;

        #endregion
        #region Setup Rigidbody
        rigidBody = GetComponent<Rigidbody>();//Sets the rigidbody to our own
        rigidBody.AddForce(rigidBody.transform.forward * Speed * speedFactor);//Pushes the snowball in the direction it is currently heading. Multiply the speed by the _speed argument so we can change how fast we can throw it in the SnowballThrowingScript.cs script
        rigidBody.transform.eulerAngles = angularVelocity;//Set rotation
        rigidBody.AddTorque(angularVelocity);//Add angular velocity force
        #endregion
    }
    //When we hit an object (Ex. : Player, Snowman, Ground)
    //TODO: Make client-prediction for the collisions
    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            damage *= Mathf.RoundToInt(lastVelocity.magnitude * damageVelocityWeight);//Take account velocity to damage, so if the snowball is fast, it does more damage
            GameObject otherobject = collision.gameObject;//The colision gameobject  
            //---Collision code handling---\\
            if (otherobject.GetComponent<BotHealthScript>() != null)
            {
                //Damage the hit bot
                otherobject.GetComponent<BotHealthScript>().DamageBot(damage);
            }
            if (otherobject.GetComponent<PlayerHealthScript>() != null)
            {
                //Damage player
                string hitPlayerNickname = otherobject.GetComponent<PlayerConfigScript>().nickname.Value;
                otherobject.GetComponent<PlayerHealthScript>().DamagePlayer(damage, "Snowball", RandomPlayerDeathMessages.RandomSnowballDeathMessage(hitPlayerNickname, properties.owner));
            }
            if (otherobject.GetComponent<BotPhysicsScript>() != null) otherobject.GetComponent<BotPhysicsScript>().DamageJoint(lastVelocity * rigidbodyForce, rigidBody.position, damage);
        }
        Destroy(gameObject);//Destroys the snowball
    }
    //Update method
    private void Update()
    {
        if(IsServer) lastVelocity = rigidBody.velocity;//Set last velocity because of yeeting bug
        /*
         We have the yeeting bug because the OnCollisionEnter is called one frame after the physics calcualtion are, so when the snowball hits a physics part its 
         actually one frame late, and it has time to rebounce in that frame, so the velocity we were getting before was the velocity of the snowball after one 
         physics frame, which is after it bounces of a little. That is the cause of the yeeting weird direction problem
        */
    }
}
