using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
//A script that handles communications between the Charachter Controller and other scripts. It allows us to move this gameObject to specified position
//TODO : Reprogram this boi. Networking support
//I might've made this work, not sure though
public class BotMovementScript : NetworkedBehaviour
{
    [Header("Movement")]
    public bool Move = true;//If the bot is able to move ?
    public float Speed;//How fast this bot is going to it's destination
    public float AirFriction;//The friction in air (Realisticly, it is close to 0)
    public float BaseFriction;//How fast the changes of velocity are
    public float RotationSmoothing;//How much to smooth between the current rotation and the target rotation

    [Header("Netorking")]
    public float MaxDistance = 0.1f;//The maximum distance the bot can get away from the actual server bot before snapping back
    public float ClientSmoothing = 15;//How much to smooth the bot's position and rotation on the clients

    #region Literal Hell : The Sequel    
    #region Client
    const float Gravity = 9.8f;//The gravity force applied to the bot that pushes it down (Using real world gravity acceleration)
    private Vector3 InputVelocity;//The velocity that we are using to move the bot
    private Vector3 Position;//The position of the bot
    private Quaternion Rotation;//The rotation of the bot
    private float Friction;//How fast the changes of velocity are (This isnt a networked var because we trust the cliant's velocity)
    private Vector3 Direction;//The direction from the current bot position to the destination
    private Vector3 Destination;//The position that we want this bot to go to
    private Vector3 WorldVelocity;//The world velocity of the bot (Rotation is taken account)
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
        Debug.DrawRay(transform.position, InputVelocity);//Local input velocity

        if (!IsServer) 
        {

            //This can be optimized by making the player go faster to the correct position the more they are away from it

            //Snap the bot back to the right position if they are too far away (Smoothed)
            if (Vector3.Distance(transform.position, Position) > MaxDistance) TeleportBotToPosition(Vector3.Lerp(transform.position, Position, ClientSmoothing * Time.deltaTime));

            //Snap the bot back to the right position if they aren't moving (Smoothed)
            if (InputVelocity.magnitude < 0.2f) TeleportBotToPosition(Vector3.Lerp(transform.position, Position, ClientSmoothing * Time.deltaTime));

            //Move the player on other clients
            //Transform the direction to world space on the clients
            cr.Move(InputVelocity * Time.deltaTime);

            //Rotate the bot on the clients (Smoothed)
            transform.rotation = Quaternion.Lerp(transform.rotation, Rotation, ClientSmoothing * Time.deltaTime);
            return;
        }

        //----------This code is only ran on the server/host----------\\

        #region Input
        //Calculate the direction from the destination position and 
        Direction = transform.position - Destination;
        Direction.y = 0;
        Direction.Normalize();

        //Set the world velocity (rotation is taken account for this one)
        WorldVelocity.x = 0; WorldVelocity.z = 1;
        WorldVelocity = transform.TransformDirection(WorldVelocity);

        //Smooth this to have like a acceleration effect and a sliding effect when friction is smaller
        //Only go forward, not left nor right.
        if (Move) 
        {
            InputVelocity.x = Mathf.Lerp(InputVelocity.x, WorldVelocity.x * Speed, Friction * Time.deltaTime);//Set inputVelocity X axis
            InputVelocity.z = Mathf.Lerp(InputVelocity.z, WorldVelocity.z * Speed, Friction * Time.deltaTime);//Set inputVelocity Z axis
        }
        else
        {
            //Stop the bot
            InputVelocity.x = Mathf.Lerp(InputVelocity.x, 0, Friction * Time.deltaTime);
            InputVelocity.z = Mathf.Lerp(InputVelocity.z, 0, Friction * Time.deltaTime);
        }

        //Smooth out the rotation of the bot
        //Invert the direction because the bot is looking backwards when it is not inverted
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-Direction), RotationSmoothing * Time.deltaTime);
        #endregion        
        #region Gravity
        if (cr.isGrounded) { InputVelocity.y = Mathf.Lerp(InputVelocity.y, 0, 2 * Time.deltaTime); }
        //Apply the gravity as an acceleration since we are in the air. Set the air friction since we are in the air
        else { InputVelocity.y -= Gravity * Time.deltaTime; Friction = AirFriction; }
        #endregion
        #region Updating
        //Move the character controller using the InputVelocity on the local client
        //Transform local velocity into world velocity (Take account the rotation of the bot)
        cr.Move(InputVelocity * Time.deltaTime);

        //Update the rotation/position on the clients

        //Update the bot position and velocity if it moved
        if (Position != transform.position) Position = transform.position; InvokeServerRpc(UpdateBotPositionOnServer, Position, InputVelocity, sendChannel);
        //Update the bot rotation if it changed
        if (Rotation != transform.rotation) Rotation = transform.rotation; InvokeServerRpc(UpdateBotRotationOnServer, Rotation, sendChannel);
        #endregion
    }
    #region Positioning
    //Moves the bot to a specified position with a constant speed (Only on server)
    public void MoveToPosition(Vector3 NewPosition) 
    { 
        if (!IsServer) return;
        Destination = NewPosition;
    }
    //Teleports the bot to a certain position
    private void TeleportBotToPosition(Vector3 position)
    {
        cr.Move(position - transform.position);//Move using velocity
    }
    #endregion
    #region Networking
    //Updates the bot's position and velocity on the server
    [ServerRPC]
    private void UpdateBotPositionOnServer(Vector3 position, Vector3 velocity) 
    {
        //Update on the server
        Position = position;
        InputVelocity = velocity;

        InvokeClientRpcOnEveryone(UpdateBotPositionOnClient, position, velocity, sendChannel);
    }
    //Updates the bot's position and velocity on the clients
    [ClientRPC]
    private void UpdateBotPositionOnClient(Vector3 position, Vector3 velocity) 
    {
        //Update on the client
        Position = position;
        InputVelocity = velocity;
    }

    //Updates the bot's rotation on the server
    [ServerRPC]
    private void UpdateBotRotationOnServer(Quaternion rotation)
    {
        //Update on the server
        Rotation = rotation;

        InvokeClientRpcOnEveryone(UpdateBotRotationOnClient, rotation, sendChannel);
    }
    //Updates the bot's rotation on the clients
    [ClientRPC]
    private void UpdateBotRotationOnClient(Quaternion rotation)
    {
        //Update on the client
        Rotation = rotation;
    }
    #endregion
    //When the bots hits something
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y < 0.9) return;//Discard the collision if it wasn't under the bot
        Friction = BaseFriction;//Since we are on the ground, reset the friction (The friction changes if we are in air)
        //If we hit a PhysicsObject
        if (hit.gameObject.GetComponent<PhysicsObjectScript>())
        {
            //Override the current friction
            Friction = hit.gameObject.GetComponent<PhysicsObjectScript>().Friction;
        }
    }
}
