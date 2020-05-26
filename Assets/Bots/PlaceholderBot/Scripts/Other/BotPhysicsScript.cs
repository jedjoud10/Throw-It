using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script for a physics part of bot. Gets yeeted when hit
public class BotPhysicsScript : NetworkedBehaviour
{
    public BotScript botscript;//The script for this bot
    public int damageThreshold;//Minimum damage this part can receive before disconnecting from the bot
    public float decayTime;//Time before the part gets destroyed
    private Joint joint;//The joint of this part to the bot
    private Rigidbody rb;//The rigidbody of this part
    private NetworkedRigidbodyScript networkedRigidbody;//Script that handles networking for rigidbodies
    
    private NetworkedVarBool detachedFromBot = new NetworkedVarBool(false);//If this physics rigidbody got it's joint removed

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        joint = GetComponent<Joint>();
        networkedRigidbody = GetComponent<NetworkedRigidbodyScript>();
        networkedRigidbody.transmitApplyData = false;
    }   
    //Damages the joint (only executed on the server)
    public void DamageJoint(Vector3 force, Vector3 _position, int Damage) 
    {
        if (joint != null && Damage > damageThreshold)//Check if this part can get yeeted if damage is big enough
        {
            Destroy(joint);//remove the joint
            rb.isKinematic = false;//Make the head movable
            detachedFromBot.Value = true;
            transform.parent = null;
            rb.AddForce(force);//Add force to our rigidbody to make it go  Y E E T
            Debug.DrawRay(_position, force, Color.black, 5.0f);
            networkedRigidbody.transmitApplyData = true;
            Destroy(gameObject, decayTime);
            //When the bot stops moving because its head got yeeted :  
            //Bot : y am i ded now
            //Bot 2 : bro that's cringe
            //Armor bot : you guys are dying?
            //Factory guys : What no dont waste the fricking materials
            //Amalgam bot: don't worry I'm eating it
            if (gameObject.name == "Head") botscript.healthScript.DamageBot(99999);
            
            InvokeClientRpcOnEveryone(RemoveJointOnClient);//Replicate on clients
        }
    }
    //Removes joint from this part (only executed on the clients)
    //We dont need to pass any arguments because this only runs when the BotPhysics part was sucsessfully removed
    [ClientRPC]
    private void RemoveJointOnClient() 
    {
        Destroy(joint);//remove the joint
        transform.parent = null;
    }
}
