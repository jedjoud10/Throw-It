using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Well uhh... a networked rigidbody yes.
public class NetworkedRigidbodyScript : NetworkedBehaviour
{
    public TransmitStateDataMode transmitData = TransmitStateDataMode.None;//What authority type this object is
    public bool applyData = false;//Should the clients apply the data the server gave them ?
    public float positionSmoothing = 15;//How much to smooth the position the server gave us
    public float rotationSmoothing = 15;//How much to smooth the rotation the server gave us

    private Rigidbody rb;//The rigidbody of this part
    //How to send the data to the clients ?
    
    //State of the rigidbody on the clients
    private Vector3 position, velocity, angularVelocity = Vector3.zero;
    private Quaternion rotation = Quaternion.identity;
    const string sendChannel = "UnreliableOrdered";//The channel where we are going to send the rigidbody data
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();//Get the rigidbody
        if ((IsOwner && transmitData == TransmitStateDataMode.Client) || (IsServer && transmitData == TransmitStateDataMode.Server))
        {
            applyData = false;//No need to apply the data when we are the sender
        }
    }

    // FixedUpdate is called each physics timestep
    void FixedUpdate()
    {
        //Server owned
        if (IsServer && transmitData == TransmitStateDataMode.Server)
        {
            InvokeClientRpcOnEveryone(UpdateRigidbodyStateOnClient, rb.position, rb.rotation, rb.velocity, rb.angularVelocity, sendChannel); //Send the new state to the clients
        }
        //Client owned
        if (IsOwner && transmitData == TransmitStateDataMode.Client) 
        {
            InvokeServerRpc(UpdateRigidbodyStateOnServer, rb.position, rb.rotation, rb.velocity, rb.angularVelocity, sendChannel);
        }

        if (IsClient && rotation.IsValid() && applyData)
        {
            //Apply the data that the server gave us (smoothed)
            rb.position = Vector3.Lerp(rb.position, position, positionSmoothing * Time.fixedDeltaTime);
            rb.rotation = Quaternion.Lerp(rb.rotation, rotation, rotationSmoothing * Time.fixedDeltaTime);
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }
    [ServerRPC]
    //Update the state of the rigidbody on the server (called from client)
    private void UpdateRigidbodyStateOnServer(Vector3 _position, Quaternion _rotation, Vector3 _velocity, Vector3 _angularVelocity) 
    {
        InvokeClientRpcOnEveryone(UpdateRigidbodyStateOnClient, _position, _rotation, _velocity, _angularVelocity, sendChannel); //Send the new state to the clients
    }
    //Update the state of the rigidbody on the clients (Called from server)
    [ClientRPC]
    private void UpdateRigidbodyStateOnClient(Vector3 _position, Quaternion _rotation, Vector3 _velocity, Vector3 _angularVelocity)
    {
        position = _position;
        rotation = _rotation;
        velocity = _velocity;
        angularVelocity = _angularVelocity;
    }
}
//How the NetworkedTransform and the NetworkedRigidbody send their data
public enum TransmitStateDataMode
{
    Server, Client, None
}
