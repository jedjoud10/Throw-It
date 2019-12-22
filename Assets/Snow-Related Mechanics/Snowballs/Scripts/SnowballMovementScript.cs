using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Controls the behaviour of the snowball movement and collisions
[RequireComponent(typeof(SnowballProperities))]
public class SnowballMovementScript : MonoBehaviour
{
    private int Damage;//The base damage the snowball can do
    private Rigidbody rigidBody;//The rigidbody of the snowball
    // Start is called before the first frame update
    public void InitSnowball(float _Speed)//Multiply our base values by those arguments
    {
        #region Setup properities
        SnowballProperities properities = GetComponent<SnowballProperities>();//Gets properities from script

        properities.InitSnowball();//Init snowball properities
        float Speed = properities.Speed;//Use one time float since we wont reuse this float later on
        Vector3 AngularVelocity = properities.AngularVelocity;
        Damage = properities.Damage;

        #endregion
        #region Setup Rigidbody
        rigidBody = GetComponent<Rigidbody>();//Sets the rigidbody to our own
        rigidBody.AddForce(rigidBody.transform.forward * Speed * _Speed);//Pushes the snowball in the direction it is currently heading. Multiply the speed by the _speed argument so we can change how fast we can throw it in the SnowballThrowingScript.cs script
        rigidBody.transform.eulerAngles = AngularVelocity;//Set rotation
        rigidBody.AddTorque(AngularVelocity);//Add angular velocity force
        #endregion
    }
    //When we hit an object (Ex. : Player, Snowman, Ground)
    private void OnCollisionEnter(Collision collision)
    {
        Damage *= Mathf.RoundToInt(rigidBody.velocity.magnitude);//Take account velocity to damage, so if the snowball is fast, it does more damage
        //Enter collision code handling

        Destroy(gameObject);//Destroys the snowball
    }
}
