using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
//Controls the behaviour of the throwable item movement and collisions
[RequireComponent(typeof(ThrowablePropertiesScript))]
public class ThrowableMovementScript : NetworkedBehaviour
{
    private int damage;//The base damage the throwable can do
    private float rigidbodyForce;//Force applied to every physics object when we hit it
    private Rigidbody rigidBody;//The rigidbody of the object
    private float damageVelocityWeight = 1;//How much the velocity changes the damage
    private Vector3 lastVelocity;//Last velocity measurement since OnCollisionEnter is called one frame after physics, thius giving us weird yeeting of the physics parts
    ThrowablePropertiesScript properties;//Properties for this throwable
    // Start is called before the first frame update
    //Inits a throwable with a player nickname string
    public void InitThrowable(float speedFactor)
    {
        #region Setup properities
        properties = GetComponent<ThrowablePropertiesScript>();//Gets properities from script
        float Speed = properties.speed;//Use one time float since we wont reuse this float later on
        damage = properties.damage;

        //Setup variables from ThrowableProperities script
        Vector3 angularVelocity = properties.angularVelocity;
        rigidbodyForce = properties.rigidbodyForce;
        damageVelocityWeight = properties.damageVelocityWeight;

        #endregion
        #region Setup Rigidbody
        rigidBody = GetComponent<Rigidbody>();//Sets the rigidbody to our own
        rigidBody.AddForce(rigidBody.transform.forward * Speed * speedFactor);//Pushes the throwable in the direction it is currently heading. Multiply the speed by the _speed argument so we can change how fast we can throw it in the ThrowableThrowingScript.cs script
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
            damage *= Mathf.RoundToInt(lastVelocity.magnitude * damageVelocityWeight);//Take account velocity to damage, so if the object is fast, it does more damage
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
                PlayerHealthScript playerHealthScript = otherobject.GetComponent<PlayerHealthScript>();
                if (hitPlayerNickname == properties.owner)//Uh-oh
                {
                    playerHealthScript.DamagePlayer(damage, properties.throwableType.ToString(), RandomPlayerMessages.SuicideDeathMessage(hitPlayerNickname));
                }
                else
                {
                    //Use the correct death messages if it wasnt a suicide
                    switch (properties.throwableType)
                    {
                        case ThrowableType.snowball:
                            playerHealthScript.DamagePlayer(damage, "snowball", RandomPlayerMessages.Throwable_SnowballDeathMessage(hitPlayerNickname, properties.owner));
                            break;

                        default:
                            break;
                    }
                }
            }
            if (otherobject.GetComponent<BotPhysicsScript>() != null) otherobject.GetComponent<BotPhysicsScript>().DamageJoint(lastVelocity * rigidbodyForce, rigidBody.position, damage);
        }
        Destroy(gameObject);//Destroys the object
    }
    //Update method
    private void Update()
    {
        if(IsServer) lastVelocity = rigidBody.velocity;//Set last velocity because of yeeting bug
        /*
         We have the yeeting bug because the OnCollisionEnter is called one frame after the physics calcualtion are, so when the throwable hits a physics part its 
         actually one frame late, and it has time to rebounce in that frame, so the velocity we were getting before was the velocity of the throwable after one 
         physics frame, which is after it bounces of a little. That is the cause of the yeeting weird direction problem
        */
    }
}
