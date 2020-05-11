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

    [Header("Netorking")]
    public float MaxDistance = 0.1f;//The maximum distance the bot can get away from the actual server bot before snapping back
    public float ClientSmoothing = 15;//How much to smooth the bot's position and rotation on the clients

    #region Literal Hell : The Sequel
    #region Networking
    private NetworkedVarVector3 ServerBotVelocity = new NetworkedVarVector3(Vector3.zero);//The velocity of the bot (On the server)
    private NetworkedVarVector3 ServerBotPosition = new NetworkedVarVector3(Vector3.zero);//The current position of the bot (On the server)
    #endregion
    #region Client
    const float Gravity = 9.8f;//The gravity force applied to the bot that pushes it down (Using real world gravity acceleration)
    private Vector3 InputVelocity;//The velocity that we are using to move the bot
    private float Friction;//How fast the changes of velocity are (This isnt a networked var because we trust the cliant's velocity)
    private Vector3 Direction;//The direction from the current bot position to the destination
    #endregion
    private CharacterController cr;//The CharacterController for this bot
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        cr = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {        
        if (!IsServer) 
        {
            //Move the bot on the clients using the velocity that the server gave us
            InputVelocity = ServerBotVelocity.Value;

            //This can be optimized by making the player go faster to the correct position the more they are away from it

            //Snap the bot back to the right position if they are too far away (Smoothed)
            if (Vector3.Distance(transform.position, ServerBotPosition.Value) > MaxDistance) TeleportBotToPosition(Vector3.Lerp(transform.position, ServerBotPosition.Value, ClientSmoothing * Time.deltaTime));

            //Snap the bot back to the right position if they aren't moving (Smoothed)
            if (InputVelocity.magnitude < 0.2f) TeleportBotToPosition(Vector3.Lerp(transform.position, ServerBotPosition.Value, ClientSmoothing * Time.deltaTime));

            //Move the player on other clients
            //Transform the direction to world space on the clients
            cr.Move(transform.TransformDirection(InputVelocity) * Time.deltaTime);
            return;
        }

        //----------This code is only ran on the server/host----------\\

        #region Input
        //Smooth this to have like a acceleration effect and a sliding effect when friction is smaller
        //Only go forward, not left nor right.
        InputVelocity.z = Mathf.Lerp(InputVelocity.z, Direction.z * Speed, Friction * Time.deltaTime);//Set inputVelocity Y axis

        //----We can set NetworkedVars because we are executing as server----\\
        //Set the server velocity of the bot (local space)
        ServerBotVelocity.Value = InputVelocity;
        //Set the server position of the bot
        ServerBotPosition.Value = transform.position;
        #endregion
        #region Gravity
        if (cr.isGrounded) { InputVelocity.y = Mathf.Lerp(InputVelocity.y, 0, 2 * Time.deltaTime); }
        //Apply the gravity as an acceleration since we are in the air. Set the air friction since we are in the air
        else { InputVelocity.y -= Gravity * Time.deltaTime; Friction = AirFriction; }
        #endregion
        #region Updating
        //Move the character controller using the InputVelocity on the local client
        //Transform local velocity into world velocity (Take account the rotation of the player)
        cr.Move(transform.TransformDirection(InputVelocity) * Time.deltaTime);

        transform.rotation = Quaternion.LookRotation(Direction);
        #endregion
    }
    //Moves the bot to a specified position with a constant speed (Only on server)
    public void MoveToPosition(Vector3 NewPosition) 
    { 
        if (!IsServer) return;
        Direction = (transform.position - NewPosition);//nOOOOO YOU CANT JUST NOT NORMALIZE THE INPUT VECTOR.   hehe unnormalized vector go Y E S
        Direction.y = 0;//Reset the Y axis because it would mess up the normalized vector
        Direction = Direction.normalized;//Set the new destination
    }
    //Teleports the bot to a certain position
    private void TeleportBotToPosition(Vector3 position)
    {
        cr.Move(position - transform.position);//Move using velocity
    }

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
