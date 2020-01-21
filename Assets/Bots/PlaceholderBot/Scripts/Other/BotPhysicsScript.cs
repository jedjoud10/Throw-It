using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script for a physics part of bot. Gets yeeted when hit
public class BotPhysicsScript : MonoBehaviour
{
    public BotMovementScript movement;//The movement of the bot
    public BotHealthScript healthScript;//Health script of bot
    public float decayTime;//Time before the part and bot gets destroyed
    private Joint joint;//The joint of this part to the bot
    private Rigidbody rb;//The rigidbody of this part
    public BotBobbingScript botBobbingScript;//Bot bobbing script that makes the bot go up and down

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();//Get the rigidbody
        joint = GetComponent<Joint>();//Get joint component
    }
    //Removes joint from this part
    public void RemoveJoint(Vector3 force, Vector3 position) 
    {
        if (joint != null)
        {
            Destroy(joint);//remove the joint
            transform.parent = null;
            rb.AddForce(force);//Add force to our rigidbody to make it go  Y E E T
            Debug.DrawRay(position, force, Color.black);
            if (healthScript.gameObject != null)
            {
                healthScript.Death(decayTime);
            }
            Destroy(gameObject, decayTime);
            //When the bot stops moving because its head got yeeted
            if (gameObject.name == "Head")
            {
                movement.move = false; //Disable movement
                botBobbingScript.applybobbing = false;//Disable bobbing
            }
        }
    }
}
