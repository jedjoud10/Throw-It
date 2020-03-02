using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script for a physics part of bot. Gets yeeted when hit
public class BotPhysicsScript : MonoBehaviour
{
    public BotScript botscript;//The script for this bot
    public int damageThreshold;//Minimum damage this part can receive before disconnecting from the bot
    public float decayTime;//Time before the part and bot gets destroyed
    private Joint joint;//The joint of this part to the bot
    private Rigidbody rb;//The rigidbody of this part

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();//Get the rigidbody
        joint = GetComponent<Joint>();//Get joint component
    }
    //Removes joint from this part
    public void RemoveJoint(Vector3 force, Vector3 position, int Damage) 
    {        
        if (joint != null && Damage > damageThreshold)//Check if this part can get yeeted if damage is big enough
        {
            Destroy(joint);//remove the joint
            transform.parent = null;
            rb.AddForce(force);//Add force to our rigidbody to make it go  Y E E T
            Debug.DrawRay(position, force, Color.black, 5.0f);
            if (botscript.healthScript.gameObject != null)
            {
                botscript.healthScript.Death(decayTime);
            }
            Destroy(gameObject, decayTime);
            //When the bot stops moving because its head got yeeted
            if (gameObject.name == "Head")
            {
                botscript.movementScript.move = false; //Disable movement
                botscript.bobbingScript.applybobbing = false;//Disable bobbing
            }
        }
    }
}
