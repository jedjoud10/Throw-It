using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Well uhh... a networked transform yes.
public class NetworkedTransformScript : NetworkedBehaviour
{
    public TransmitStateDataMode transmitData = TransmitStateDataMode.None;//What authority type this object is
    public bool applyData = false;//Should the clients apply the data the server gave them ?
    public bool localTransform = false;//Use the local transform instead of the global one
    public float positionSmoothing = 15;//How much to smooth the position the server gave us
    public float rotationSmoothing = 15;//How much to smooth the rotation the server gave us

    //State of the transform on the clients
    private Vector3 position = Vector3.zero;
    private Quaternion rotation = Quaternion.identity;
    const string sendChannel = "UnreliableOrdered";//The channel where we are going to send the rigidbody data

    // Start is called before the first frame update
    void Start()
    {
        if ((IsOwner && transmitData == TransmitStateDataMode.Client) || (IsServer && transmitData == TransmitStateDataMode.Server))
        {
            applyData = false;//No need to apply the data when we are the sender
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Server owned
        if (IsServer && transmitData == TransmitStateDataMode.Server)
        {
            InvokeClientRpcOnEveryone(UpdateTransformStateOnClient, localTransform ? transform.localPosition : transform.position, localTransform ? transform.localRotation : transform.rotation, sendChannel); //Send the new state to the clients
        }
        //Client owned
        if (IsOwner && transmitData == TransmitStateDataMode.Client)
        {
            InvokeServerRpc(UpdateTransformStateOnServer, transform.position, transform.rotation, sendChannel);
        }

        if (IsClient && rotation.IsValid() && applyData)
        {
            //Apply the data that the server gave us (smoothed)
            if (!localTransform)
            {
                transform.position = Vector3.Lerp(transform.position, position, positionSmoothing * Time.fixedDeltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSmoothing * Time.fixedDeltaTime);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, position, positionSmoothing * Time.fixedDeltaTime);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, rotation, rotationSmoothing * Time.fixedDeltaTime);
            }
        }
    }
    [ServerRPC]
    //Update the state of the transform on the server (called from client)
    private void UpdateTransformStateOnServer(Vector3 _position, Quaternion _rotation)
    {
        InvokeClientRpcOnEveryone(UpdateTransformStateOnClient, _position, _rotation, sendChannel); //Send the new state to the clients
    }
    //Update the state of the transform on the clients (Called from server)
    [ClientRPC]
    private void UpdateTransformStateOnClient(Vector3 _position, Quaternion _rotation)
    {
        position = _position;
        rotation = _rotation;
    }
}
