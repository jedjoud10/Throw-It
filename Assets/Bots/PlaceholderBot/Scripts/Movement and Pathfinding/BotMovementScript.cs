using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
//A script that handles communications between the Charachter Controller and other scripts. It allows us to move this gameObject to specified position
public class BotMovementScript : NetworkedBehaviour
{
    [Header("Movement")]
    public bool move = true;//If the bot is able to move ?
    public float speed;//How fast this bot is going to it's destination
    public float airFriction;//The friction in air (Realisticly, it is close to 0)
    public float baseFriction;//How fast the changes of velocity are
    public float rotationSmoothing;//How much to smooth between the current rotation and the target rotation

    [Header("Netorking")]
    public float maxDistance = 0.1f;//The maximum distance the bot can get away from the actual server bot before snapping back
    public float clientSmoothing = 15;//How much to smooth the bot's position and rotation on the clients

    #region Literal Hell : The Sequel    
    #region Client
    const float gravity = 9.8f;//The gravity force applied to the bot that pushes it down (Using real world gravity acceleration)
    private Vector3 inputVelocity;//The velocity that we are using to move the bot
    private Vector3 position;//The position of the bot
    private Quaternion rotation;//The rotation of the bot
    private float friction;//How fast the changes of velocity are (This isnt a networked var because we trust the cliant's velocity)
    private Vector3 direction;//The direction from the current bot position to the destination
    private Vector3 destination;//The position that we want this bot to go to
    private Vector3 worldVelocity;//The world velocity of the bot (Rotation is taken account)
    #endregion
    private CharacterController cr;//The CharacterController for this bot
    const string sendChannel = "UnreliableOrdered";//The channel where we are going to send the bot data
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        cr = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, inputVelocity);//Local input velocity

        if (!IsServer) 
        {

            //This can be optimized by making the player go faster to the correct position the more they are away from it

            //Snap the bot back to the right position if they are too far away (Smoothed)
            if (Vector3.Distance(transform.position, position) > maxDistance) TeleportBotToPosition(Vector3.Lerp(transform.position, position, clientSmoothing * Time.deltaTime));

            //Snap the bot back to the right position if they aren't moving (Smoothed)
            if (inputVelocity.magnitude < 0.2f) TeleportBotToPosition(Vector3.Lerp(transform.position, position, clientSmoothing * Time.deltaTime));

            //Move the player on other clients
            //Transform the direction to world space on the clients
            cr.Move(inputVelocity * Time.deltaTime);

            //Rotate the bot on the clients (Smoothed)
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, clientSmoothing * Time.deltaTime);
            return;
        }

        //----------This code is only ran on the server/host----------\\

        #region Input
        //Calculate the direction from the destination position and 
        direction = transform.position - destination;
        direction.y = 0;
        direction.Normalize();

        //Set the world velocity (rotation is taken account for this one)
        worldVelocity.x = 0; worldVelocity.z = 1;
        worldVelocity = transform.TransformDirection(worldVelocity);

        //Smooth this to have like a acceleration effect and a sliding effect when friction is smaller
        //Only go forward, not left nor right.
        if (move) 
        {
            inputVelocity.x = Mathf.Lerp(inputVelocity.x, worldVelocity.x * speed, friction * Time.deltaTime);//Set inputVelocity X axis
            inputVelocity.z = Mathf.Lerp(inputVelocity.z, worldVelocity.z * speed, friction * Time.deltaTime);//Set inputVelocity Z axis
        }
        else
        {
            //Stop the bot
            inputVelocity.x = Mathf.Lerp(inputVelocity.x, 0, friction * Time.deltaTime);
            inputVelocity.z = Mathf.Lerp(inputVelocity.z, 0, friction * Time.deltaTime);
        }

        //Smooth out the rotation of the bot
        //Invert the direction because the bot is looking backwards when it is not inverted
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-direction), rotationSmoothing * Time.deltaTime);
        #endregion        
        #region Gravity
        if (cr.isGrounded) { inputVelocity.y = Mathf.Lerp(inputVelocity.y, 0, 2 * Time.deltaTime); }
        //Apply the gravity as an acceleration since we are in the air. Set the air friction since we are in the air
        else { inputVelocity.y -= gravity * Time.deltaTime; friction = airFriction; }
        #endregion
        #region Updating
        //Move the character controller using the InputVelocity on the local client
        //Transform local velocity into world velocity (Take account the rotation of the bot)
        cr.Move(inputVelocity * Time.deltaTime);

        //Update the rotation/position on the clients

        //Update the bot position and velocity if it moved
        if (position != transform.position) position = transform.position; InvokeServerRpc(UpdateBotPositionOnServer, position, inputVelocity, sendChannel);
        //Update the bot rotation if it changed
        if (rotation != transform.rotation) rotation = transform.rotation; InvokeServerRpc(UpdateBotRotationOnServer, rotation, sendChannel);
        #endregion
    }
    #region Positioning
    //Moves the bot to a specified position with a constant speed (Only on server)
    public void MoveToPosition(Vector3 _position) 
    { 
        if (!IsServer) return;
        destination = _position;
    }
    //Teleports the bot to a certain position
    private void TeleportBotToPosition(Vector3 _position)
    { 
        transform.position = (_position);//Move using velocity
        Physics.SyncTransforms();
    }
    #endregion
    #region Networking
    //Updates the bot's position and velocity on the server
    [ServerRPC]
    private void UpdateBotPositionOnServer(Vector3 _position, Vector3 _velocity) 
    {
        //Update on the server
        position = _position;
        inputVelocity = _velocity;

        InvokeClientRpcOnEveryone(UpdateBotPositionOnClient, _position, _velocity, sendChannel);
    }
    //Updates the bot's position and velocity on the clients
    [ClientRPC]
    private void UpdateBotPositionOnClient(Vector3 _position, Vector3 _velocity) 
    {
        //Update on the client
        position = _position;
        inputVelocity = _velocity;
    }

    //Updates the bot's rotation on the server
    [ServerRPC]
    private void UpdateBotRotationOnServer(Quaternion _rotation)
    {
        //Update on the server
        rotation = _rotation;

        InvokeClientRpcOnEveryone(UpdateBotRotationOnClient, _rotation, sendChannel);
    }
    //Updates the bot's rotation on the clients
    [ClientRPC]
    private void UpdateBotRotationOnClient(Quaternion _rotation)
    {
        //Update on the client
        rotation = _rotation;
    }
    #endregion
    //When the bots hits something
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y < 0.9) return;//Discard the collision if it wasn't under the bot
        friction = baseFriction;//Since we are on the ground, reset the friction (The friction changes if we are in air)
        //If we hit a PhysicsObject
        if (hit.gameObject.GetComponent<PhysicsObjectScript>())
        {
            //Override the current friction
            friction = hit.gameObject.GetComponent<PhysicsObjectScript>().friction;
        }
    }
}
