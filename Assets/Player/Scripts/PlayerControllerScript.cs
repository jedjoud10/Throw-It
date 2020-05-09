using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using UnityEngine;
//Controls the camera and movement of the player from keyboard and mouse (V2)
public class PlayerControllerScript : NetworkedBehaviour
{
    [Header("Control")]
    public float MouseSensivity = 1.0f;//How much to scale the mouse input values before passing them to the playeRotation/cameraRotation
    public float WalkingSpeed, SprintingSpeed;//All the possible FOVs based on the state of the player
    public float Jump;//How much the player can jump    
    public float FactorSmoothingSpeed;//How fast a Factor (WalkingFactor or SprintingFactor) goes to it's correct value
    public float AirFriction;//The friction in air (Realisticly, it is close to 0)
    public float BaseFriction;//How fast the changes of velocity are
    [Header("Netorking")]
    public float MaxDistance = 0.1f;//The maximum distance the player on other clients can get away from the actual local player before snapping back
    public float ClientSmoothing = 15;//How much to smooth other player's position, rotation and head rotation
    [Header("Objects")]
    public Camera playerCamera;//The camera (Disabled on non-local players)
    public GameObject HeadObject;//The head model (Disabled on the local player)
    public GameObject PlayerObject;//The player model (Disabled on the local player)
    [Header("FOV")]
    //All the possible FOVs based on the state of the player
    public float IdleFOV;
    public float WalkingFOV;
    public float SprintingFOV;

    #region Literal Hell - Don't open
    #region Local player stuff
    private float localPlayerRotationY;//The Y rotation of the player on the local machine
    private float localCameraRotationX;//The X rotation of the camera on the local machine
    private Vector3 InputVelocity;//The velocity that we are using to move the player
    private Vector2 InputData;//The input velocity plane (x, z) from the keyboard
    private bool Jumping;//If the local player is jumping
    private CharacterController cr;//The character controller that will move the player based on velocity
    const float Gravity = 9.8f;//The gravity force applied to the player that pushes them down (Using real world gravity acceleration)
    private float Friction;//How fast the changes of velocity are (This isnt a networked var because it also changes on the server, so no need to make a ServerRPC)
    private float WalkingFactor, SprintingFactor;//Two factors for the player to smoothly transition between values
    private float Speed;//The current speed smoothed from WalkingSpeed and SprintingSpeed
    #endregion
    #region Networking
    private NetworkedVarFloat ServerPlayerRotationY = new NetworkedVarFloat(0.0f);//The Y rotation (Left-Right) of the player on the server
    private NetworkedVarFloat ServerCameraRotationX = new NetworkedVarFloat(0.0f);//The X rotation (Up-Down) of the camera on the server
    private NetworkedVarVector3 ServerPlayerPosition = new NetworkedVarVector3(Vector3.zero);//The position that we want this clients's player to be at
    private NetworkedVarVector3 ServerPlayerInputVelocity = new NetworkedVarVector3(Vector3.zero);//The input velocity of the player on the server
    private NetworkedVarBool ServerPlayerJumping = new NetworkedVarBool(false);//If the player is jumping on the server
    #endregion
    private Transform playerSpawn;//The position that this player will spawn at
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        //Setup the character controller
        cr = GetComponent<CharacterController>();

        //Set the default player position
        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform;

        if (IsLocalPlayer)
        {
            //Disable player models
            HeadObject.SetActive(false);
            PlayerObject.SetActive(false);
        }
        else
        {
            //Disable camera on other players
            playerCamera.gameObject.SetActive(false);
        }
        //Hide cursor and lock it
        Cursor.visible = false; Cursor.lockState = CursorLockMode.Locked;

        //Reset the player so they spawn at the PlayerSpawnPoint
        ResetPlayer();
    }
    // Update is called once per frame
    void Update()
    {
        //Debug
        Debug.DrawRay(transform.position, InputVelocity);

        //If we aren't the local client 
        if (!IsLocalPlayer)
        {
            //Move this player object on the other clients using the velocity that the server gave us
            InputVelocity = ServerPlayerInputVelocity.Value;
            
            //Snap the player back to the right position if they are too far away (Smoothed)
            if (Vector3.Distance(transform.position, ServerPlayerPosition.Value) > MaxDistance) MovePlayerToPosition(Vector3.Lerp(transform.position, ServerPlayerPosition.Value, ClientSmoothing * Time.deltaTime));

            //Snap the player back to the right position if they aren't moving (Smoothed)
            if (InputVelocity.magnitude < 0.2f) MovePlayerToPosition(Vector3.Lerp(transform.position, ServerPlayerPosition.Value, ClientSmoothing * Time.deltaTime));
            
            //Move the player on other clients
            cr.Move(InputVelocity * Time.deltaTime);

            //Rotate the player on the other clients (Smoothed)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, ServerPlayerRotationY.Value, 0), ClientSmoothing * Time.deltaTime);
            //Rotate the player head on other clients (Smoothed)
            HeadObject.transform.localRotation = Quaternion.Lerp(HeadObject.transform.localRotation, Quaternion.Euler(ServerCameraRotationX.Value, 0, 0), ClientSmoothing * Time.deltaTime);

            return;
        }

        //----------This code is only ran on the local client machine----------\\

        #region Input
        //------Input from keyboard and mouse------\\

        //Read the input
        Jumping = Input.GetAxis("Jump") > 0.0f;//Determine if the player is jumping
        InputData.x = Input.GetAxis("LeftRight");//Set input X axis
        InputData.y = Input.GetAxis("ForwardBackward");//Set input Y axis

        //Inverse transform this when we try to Lerp from the current InputVelocity to the InputData
        //Because the InputVelocity is world-space and the InputData is local-space
        InputVelocity = transform.InverseTransformDirection(InputVelocity);

        //Move the player forward/backward/left/right using the keyboard
        //Smooth this to have like a acceleration effect and a sliding effect when friction is smaller
        InputVelocity.x = Mathf.Lerp(InputVelocity.x, InputData.x * Speed, Friction * Time.deltaTime);//Set inputVelocity X axis 
        InputVelocity.z = Mathf.Lerp(InputVelocity.z, InputData.y * Speed, Friction * Time.deltaTime);//Set inputVelocity Y axis
        //Rotate the player left and right
        localPlayerRotationY += Input.GetAxis("Mouse X") * MouseSensivity;
        //Rotate the camera up and down
        localCameraRotationX -= Input.GetAxis("Mouse Y") * MouseSensivity;
        //Clamp the head rotation because necks can absolutely bend over infinitely
        localCameraRotationX = Mathf.Clamp(localCameraRotationX, -90, 90);
        #endregion
        #region Walking/Sprinting factors
        //------Walking/Sprinting factors------\\

        //WalkingFactor smoothly goes to 1 when we are walking
        WalkingFactor = Mathf.Lerp(WalkingFactor, (InputData.magnitude > 0.0f) ? 1 : 0, FactorSmoothingSpeed * Time.deltaTime);
        //SprintingFactor smoothly goes to 1 when we are sprinting
        SprintingFactor = Mathf.Lerp(SprintingFactor, (Input.GetAxis("Sprint") > 0.0f) ? 1 : 0, FactorSmoothingSpeed * Time.deltaTime);

        //Set the new calculated speed from the SprintingFactor
        Speed = Mathf.Lerp(WalkingSpeed, SprintingSpeed, SprintingFactor);//Only sprinting factor this time

        //Set the new calculated FOV from the WalkingFactor and SprintingFactor
        playerCamera.fieldOfView = Mathf.Lerp(IdleFOV, Mathf.Lerp(WalkingFOV, SprintingFOV, SprintingFactor), WalkingFactor);
        #endregion
        #region Gravity
        //------Gravity------\\

        //Make the player jump when we press the "Jump" button    
        //Smooth the InputVelociy.y because when we hit the ground the player has a "bouncing" effect so smoothing it makes it more natural
        if (cr.isGrounded) { InputVelocity.y = Mathf.Lerp(InputVelocity.y, 0, 2 * Time.deltaTime); if (Jumping) InputVelocity.y = Jump; }
        //Apply the gravity as an acceleration if we are in the air. Set the air friction since we are in air
        else { InputVelocity.y -= Gravity * Time.deltaTime; Friction = AirFriction; }
        #endregion
        #region Updating
        //------Updating the player------\\

        //Transform local velocity into world velocity (Take account the rotation of the player)
        InputVelocity = transform.TransformDirection(InputVelocity);

        //Move the character controller using the InputVelocity on the local client
        cr.Move(InputVelocity * Time.deltaTime);

        //Rotate the player on the local client
        transform.rotation = Quaternion.Euler(0, localPlayerRotationY, 0);

        //Rotate the camera on the local client
        playerCamera.transform.localRotation = Quaternion.Euler(localCameraRotationX, 0, 0);

        //HeadObject.transform.localRotation = playerCamera.transform.localRotation;//This is useless since the local player never sees their head, but eh why not

        //Send that data to the server so it can relay it to the other clients
        //Send the velocity, position and if the player is jumping or not
        //When the player is moving, send the data each frame
        if (InputVelocity.magnitude > 0) InvokeServerRpc(UpdatePlayerPositionOnServer, InputVelocity, transform.position, Jumping);
        //When the player isn't moving, send the data each 5 frames
        else if (Time.frameCount % 5 == 0) { InvokeServerRpc(UpdatePlayerPositionOnServer, InputVelocity, transform.position, Jumping); }
        //Only update the rotation if the mouse moved
        if (Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.0f) InvokeServerRpc(UpdatePlayerRotationsOnServer, localPlayerRotationY, localCameraRotationX);
        #endregion
    }
    #region Networking
    //Update the player position and velocity on the server
    [ServerRPC]
    private void UpdatePlayerPositionOnServer(Vector3 velocity, Vector3 position, bool jumping)
    {
        //---Set server variables---\\
        //TODO: Check if the player is hacking
        if (true)
        {
            ServerPlayerInputVelocity.Value = velocity;
            ServerPlayerPosition.Value = position;
            ServerPlayerJumping.Value = jumping;
        }
    }
    //Update the player rotation and camera rotation on the server
    [ServerRPC]
    private void UpdatePlayerRotationsOnServer(float playerRotationY, float cameraRotationX)
    {
        //It doesnt matter if the player is cheating or not when setting the rotations
        ServerPlayerRotationY.Value = playerRotationY;
        ServerCameraRotationX.Value = cameraRotationX;
    }
    #endregion
    #region Player positioning
    //Resets the player position and velocity
    public void ResetPlayer()
    {
        //Reset the position
        MovePlayerToPosition(playerSpawn.position);

        InputVelocity = Vector3.zero;
    }
    //Moves the player to a certain position
    private void MovePlayerToPosition(Vector3 position)
    {
        cr.Move(position - transform.position);//Move using velocity
    }
    #endregion
    //When the player hits something
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y < 0.9) return;//Discard the collision if it wasn't under the player
        Friction = BaseFriction;//Since we are on the ground, reset the friction (The friction changes if we are in air)
        //If we hit a PhysicsObject
        if (hit.gameObject.GetComponent<PhysicsObjectScript>())
        {
            //Override the current friction
            Friction = hit.gameObject.GetComponent<PhysicsObjectScript>().Friction;
        }
    }
}