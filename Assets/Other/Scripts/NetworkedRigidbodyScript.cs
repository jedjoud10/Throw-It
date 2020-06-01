using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Well uhh... a networked rigidbody yes
public class NetworkedRigidbodyScript : NetworkedBehaviour
{
    public bool transmitData = false;//Should the server transmit the data
    public bool applyData = false;//Should the clients the data the server gave them ?
    public float positionSmoothing = 15;//How much to smooth the position the server gave us
    public float rotationSmoothing = 15;//How much to smooth the rotation the server gave us

    private Rigidbody rb;//The rigidbody of this part
    //State of the rigidbody on the clients
    private Vector3 position, velocity, angularVelocity = Vector3.zero;
    private Quaternion rotation = Quaternion.identity;
    const string sendChannel = "UnreliableOrdered";//The channel where we are going to send the rigidbody data
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();//Get the rigidbody
    }

    // FixedUpdate is called each physics timestep
    void FixedUpdate()
    {
        if (IsServer && transmitData)
        {
            InvokeClientRpcOnEveryone(UpdateRigidbodyStateOnClient, rb.position, rb.rotation, sendChannel); //Send the new state to the clients
        }

        if (IsClient && rotation.IsValid() && applyData)
        {
            //Apply the data that the server gave us (smoothed)
            rb.position = Vector3.Lerp(rb.position, position, positionSmoothing * Time.fixedDeltaTime);
            rb.rotation = Quaternion.Lerp(rb.rotation, rotation, rotationSmoothing * Time.fixedDeltaTime);
            //rb.velocity = velocity;
            //rb.angularVelocity = angularVelocity;
        }
    }
    //Update the state of the rigidbody on the clients
    [ClientRPC]
    private void UpdateRigidbodyStateOnClient(Vector3 _position, Quaternion _rotation)
    {
        position = _position;
        //velocity = _velocity;
        rotation = _rotation;
        //angularVelocity = _angularVelocity;
    }
}
