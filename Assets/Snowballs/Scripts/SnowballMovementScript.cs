using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Controls the behaviour of the snowball movement and collisions
public class SnowballMovementScript : MonoBehaviour
{
    [Header("Settings")]
    public float Size;//The size of the snowball from the start
    public float Speed;//How fast does the snowball gets pushed from the start
    public int Damage;//The base damage the snowball can do
    private Rigidbody rigidBody;//The rigidbody of the snowball
    // Start is called before the first frame update
    public void InitSnowball(float _Speed)//Multiply our base values by those arguments
    {
        #region Setup Settings
        rigidBody = GetComponent<Rigidbody>();//Sets the rigidbody to our own
        rigidBody.AddForce(rigidBody.transform.forward * Speed * _Speed);//Pushes the snowball in the direction it is currently heading. Multiply the speed by the _speed argument so we can change how fast we can throw it in the SnowballThrowingScript.cs script
        transform.localScale *= Size;//Multiplies the current scale of the snowball by the Size variable.
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
