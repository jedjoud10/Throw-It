using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Well uhh... a networked rigidbody yes
public class NetworkedRigidbodyScript : NetworkedBehaviour
{
    public TransmitRigidbodyDataMode transmitData = TransmitRigidbodyDataMode.None;//Can the owner of this rigidbody update it's state on the server and on other clients ?
    public bool applyData = false;//Should the clients apply the data the server gave them ?
    public float positionSmoothing = 15;//How much to smooth the position the server gave us
    public float rotationSmoothing = 15;//How much to smooth the rotation the server gave us

    private Rigidbody rb;//The rigidbody of this part
    public enum TransmitRigidbodyDataMode 
    {
        Server, Client, None
    }
    //State of the rigidbody on the clients
    private Vector3 position, velocity, angularVelocity = Vector3.zero;
    private Quaternion rotation = Quaternion.identity;
    const string sendChannel = "UnreliableOrdered";//The channel where we are going to send the rigidbody data
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();//Get the rigidbody
        if ((IsOwner && transmitData == TransmitRigidbodyDataMode.Client) || (IsServer && transmitData == TransmitRigidbodyDataMode.Server))
        {
            applyData = false;//No need to apply the data when we are the sender
        }
    }

    // FixedUpdate is called each physics timestep
    void FixedUpdate()
    {
        //Server owned
        if (IsServer && transmitData == TransmitRigidbodyDataMode.Server)
        {
            InvokeClientRpcOnEveryone(UpdateRigidbodyStateOnClient, rb.position, rb.rotation, sendChannel); //Send the new state to the clients
        }
        //Client owned
        if (IsOwner && transmitData == TransmitRigidbodyDataMode.Client) 
        {
            InvokeServerRpc(UpdateRigidbodyStateOnServer, rb.position, rb.rotation, sendChannel);
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
    [ServerRPC]
    //Update the state of the rigidbody on the server (called from client)
    private void UpdateRigidbodyStateOnServer(Vector3 _position, Quaternion _rotation) 
    {
        InvokeClientRpcOnEveryone(UpdateRigidbodyStateOnClient, _position, _rotation, sendChannel); //Send the new state to the clients
    }
    //Update the state of the rigidbody on the clients (Called from server)
    [ClientRPC]
    private void UpdateRigidbodyStateOnClient(Vector3 _position, Quaternion _rotation)
    {
        position = _position;
        //velocity = _velocity;
        rotation = _rotation;
        //angularVelocity = _angularVelocity;
    }
}
