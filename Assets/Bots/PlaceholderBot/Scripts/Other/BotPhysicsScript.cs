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
    public float positionSmoothing = 15;//How much to smooth the position the server gave us
    public float rotationSmoothing = 15;//How much to smooth the rotation the server gave us

    //State of the rigidbody on the clients
    private Vector3 position, velocity, angularVelocity = Vector3.zero;
    private Quaternion rotation = Quaternion.identity;
    private NetworkedVarBool detachedFromBot = new NetworkedVarBool(false);//If this physics rigidbody got it's joint removed

    const string sendChannel = "UnreliableOrdered";

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();//Get the rigidbody
        joint = GetComponent<Joint>();//Get joint component
    }
    // FixedUpdate is called each physics timestep
    void FixedUpdate()
    {        
        if (IsServer && detachedFromBot.Value) 
        { 
            InvokeClientRpcOnEveryone(UpdateRigidbodyStateOnClient, rb.position, rb.rotation, sendChannel); //Send the new state to the clients
        }

        if(IsClient && detachedFromBot.Value && rotation.IsValid())
        {
            //Apply the data that the server gave us (smoothed)
            rb.position = Vector3.Lerp(rb.position, position, positionSmoothing * Time.fixedDeltaTime);
            rb.rotation = Quaternion.Lerp(rb.rotation, rotation, rotationSmoothing * Time.fixedDeltaTime);            
            //rb.velocity = velocity;
            //rb.angularVelocity = angularVelocity;
        }
    }
    //TODO: Make this a separate component
    //Update the state of the rigidbody on the clients
    [ClientRPC]
    private void UpdateRigidbodyStateOnClient(Vector3 _position, Quaternion _rotation) 
    {
        position = _position;
        //velocity = _velocity;
        rotation = _rotation;
        //angularVelocity = _angularVelocity;
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
