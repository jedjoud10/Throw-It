using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using UnityEngine;
//Controls the camera and movement of the player from keyboard and mouse (V2)
//TODO : Make the local client snap back to the correct position
public class PlayerControllerScript : NetworkedBehaviour
{
    [Header("Control")]
    public bool controllable;//Can the player move/rotate using the input ?
    public float mouseSensivity = 1.0f;//How much to scale the mouse input values before passing them to the playeRotation/cameraRotation
    public float walkingSpeed, sprintingSpeed;//All the possible FOVs based on the state of the player
    public float jump;//How much the player can jump    
    public float factorSmoothingSpeed;//How fast a Factor (WalkingFactor or SprintingFactor) goes to it's correct value
    public float airFriction;//The friction in air (Realisticly, it is close to 0)
    public float baseFriction;//How fast the changes of velocity are
    [Header("Netorking")]
    public float maxDistance = 0.1f;//The maximum distance the player on other clients can get away from the actual local player before snapping back
    public float clientSmoothing = 15;//How much to smooth other player's position, rotation and head rotation
    [Header("Objects")]
    public Camera playerCamera;//The camera (Disabled on non-local players)
    public GameObject headObject;//The head model (Disabled on the local player)
    public GameObject playerObject;//The player model (Disabled on the local player)
    [Header("FOV")]
    //All the possible FOVs based on the state of the player
    public float idleFOV;
    public float walkingFOV;
    public float sprintingFOV;

    #region Literal Hell - Don't open
    #region Local player stuff
    private float playerRotationY;//The Y rotation of the player
    private float cameraRotationX;//The X rotation of the camera
    private Vector3 inputVelocity;//The velocity that we are using to move the player
    private Vector2 inputData;//The input velocity plane (x, z) from the keyboard
    private Vector3 position;//The position of the player
    private bool jumping;//If the local player is jumping
    const float gravity = 9.8f;//The gravity force applied to the player that pushes them down (Using real world gravity acceleration)
    private float friction;//How fast the changes of velocity are (This isnt a networked var because it also changes on the server, so no need to make a ServerRPC)
    private float walkingFactor, sprintingFactor;//Two factors for the player to smoothly transition between values
    private float speed;//The current speed smoothed from WalkingSpeed and SprintingSpeed
    private Vector3 worldVelocity;//The world velocity of the player (Rotation is taken account)
    #endregion
    private CharacterController cr;//The character controller that will move the player based on velocity
    const string sendChannel = "UnreliableOrdered";//The channel where we are going to send the player data
    private Transform playerSpawn;//The position that this player will spawn at
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        //Setup the character controller
        cr = GetComponent<CharacterController>();

        //Set the default player position
        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform;

        //Activate camera on this local player only
        playerCamera.gameObject.SetActive(IsLocalPlayer);
        if (IsLocalPlayer)
        {
            //Disable player models but the shadows are still active

            playerObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            headObject.transform.GetChild(0).GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            if (FindObjectOfType<GameConfigHandlerScript>().instance.currentGameConfig.FastRendering)//Rendering bug when forward rendering and ShadowsOnly, so we need to disable the mesh complitely
            {
                headObject.transform.GetChild(0).GetComponent<MeshRenderer>().materials = new Material[0];
                playerObject.GetComponent<MeshRenderer>().materials = new Material[0];
            }
        }
        //Hide cursor and lock it
        Cursor.visible = false; Cursor.lockState = CursorLockMode.Locked;

        //Reset the player so they spawn at the PlayerSpawnPoint
        ResetPlayer();
    }
    // Update is called once per frame
    void Update()
    {

        //If we aren't the local client 
        if (!IsLocalPlayer)
        {

            //Snap the player back to the right position if they are too far away (Smoothed)
            if (Vector3.Distance(transform.position, position) > maxDistance) MovePlayerToPosition(Vector3.Lerp(transform.position, position, clientSmoothing * Time.deltaTime));

            //Snap the player back to the right position if they aren't moving (Smoothed)
            if (inputVelocity.magnitude < 0.2f) MovePlayerToPosition(Vector3.Lerp(transform.position, position, clientSmoothing * Time.deltaTime));

            //Move the player on other clients
            cr.Move(inputVelocity * Time.deltaTime);

            //Rotate the player on the other clients (Smoothed)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, playerRotationY, 0), clientSmoothing * Time.deltaTime);
            //Rotate the player head on other clients (Smoothed)
            headObject.transform.localRotation = Quaternion.Lerp(headObject.transform.localRotation, Quaternion.Euler(cameraRotationX, 0, 0), clientSmoothing * Time.deltaTime);

            return;
        }

        //----------This code is only ran on the local client machine----------\\

        #region Input

        //Read the input
        if (controllable)
        {
            inputData.x = 0;
            //Movement left / right
            if (InputManager.GetKey("Left")) 
            {
                inputData.x = -1;
            }
            else if(InputManager.GetKey("Right"))
            {
                inputData.x = 1;
            }
            inputData.y = 0;
            //Movement backwards / forwards
            if (InputManager.GetKey("Backward"))
            {
                inputData.y = -1;
            }
            else if (InputManager.GetKey("Forward"))
            {
                inputData.y = 1;
            }
            jumping = InputManager.GetKey("Jump");
        }
        else
        {
            inputData.x = 0; inputData.y = 0; jumping = false;
        }
        //Set the world velocity (rotation is taken account for this one)
        worldVelocity.x = inputData.x; worldVelocity.z = inputData.y;
        worldVelocity = transform.TransformDirection(worldVelocity);
        worldVelocity.Normalize();

        //Move the player forward/backward/left/right using the keyboard
        //Smooth this to have like a acceleration effect and a sliding effect when friction is smaller
        inputVelocity.x = Mathf.Lerp(inputVelocity.x, worldVelocity.x * speed, friction * Time.deltaTime);//Set inputVelocity X axis 
        inputVelocity.z = Mathf.Lerp(inputVelocity.z, worldVelocity.z * speed, friction * Time.deltaTime);//Set inputVelocity Z axis
        Debug.DrawRay(transform.position, inputVelocity);
        if (controllable)
        {
            //Rotate the player left and right
            playerRotationY += Input.GetAxis("Mouse X") * mouseSensivity;
            //Rotate the camera up and down
            cameraRotationX -= Input.GetAxis("Mouse Y") * mouseSensivity;
        }
        //Clamp the head rotation because necks can absolutely bend over infinitely
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);
        #endregion
        #region Walking/Sprinting factors

        //WalkingFactor smoothly goes to 1 when we are walking
        walkingFactor = Mathf.Lerp(walkingFactor, (inputData.magnitude > 0.0f) ? 1 : 0, factorSmoothingSpeed * Time.deltaTime);
        //SprintingFactor smoothly goes to 1 when we are sprinting
        sprintingFactor = Mathf.Lerp(sprintingFactor, InputManager.GetKey("Sprint") ? 1 : 0, factorSmoothingSpeed * Time.deltaTime);

        //Set the new calculated speed from the SprintingFactor
        speed = Mathf.Lerp(walkingSpeed, sprintingSpeed, sprintingFactor);//Only sprinting factor this time

        //Set the new calculated FOV from the WalkingFactor and SprintingFactor
        playerCamera.fieldOfView = Mathf.Lerp(idleFOV, Mathf.Lerp(walkingFOV, sprintingFOV, sprintingFactor), walkingFactor);
        #endregion
        #region Gravity

        //Make the player jump when we press the "Jump" button    
        //Smooth the InputVelociy.y because when we hit the ground the player has a "bouncing" effect so smoothing it makes it more natural
        if (cr.isGrounded) { inputVelocity.y = Mathf.Lerp(inputVelocity.y, 0, 2 * Time.deltaTime); if (jumping) inputVelocity.y = jump; }
        //Apply the gravity as an acceleration if we are in the air. Set the air friction since we are in air
        else { inputVelocity.y -= gravity * Time.deltaTime; friction = airFriction; }
        #endregion
        #region Updating
        //Move the character controller using the InputVelocity on the local client
        cr.Move(inputVelocity * Time.deltaTime);

        //Rotate the player on the local client
        transform.rotation = Quaternion.Euler(0, playerRotationY, 0);
        //Rotate the camera on the local client
        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);

        headObject.transform.localRotation = playerCamera.transform.localRotation;

        //Send that data to the server so it can relay it to the other clients
        //Send the velocity, position and if the player is jumping or not
        //When the player is moving, send the data each frame
        if (inputVelocity.sqrMagnitude > 0) InvokeServerRpc(UpdatePlayerPositionOnServer, inputVelocity, transform.position, OwnerClientId, sendChannel);
        //When the player isn't moving, send the data each 5 frames
        else if (Time.frameCount % 10 == 0) { InvokeServerRpc(UpdatePlayerPositionOnServer, inputVelocity, transform.position, OwnerClientId, sendChannel); }
        //Only update the rotation if the mouse moved
        if (Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.0f) InvokeServerRpc(UpdatePlayerRotationsOnServer, playerRotationY, cameraRotationX, OwnerClientId, sendChannel);
        #endregion
    }
    #region Networking
    //Update the player position and velocity on the server
    [ServerRPC]
    private void UpdatePlayerPositionOnServer(Vector3 _velocity, Vector3 _position, ulong clientID)
    {
        //Update on the server
        inputVelocity = _velocity;
        position = _position;

        InvokeClientRpcOnEveryoneExcept(UpdatePlayerPositionOnClient, clientID, _velocity, _position, sendChannel);
    }
    [ClientRPC]
    //Update the player position and velocity on the clients
    private void UpdatePlayerPositionOnClient(Vector3 _velocity, Vector3 _position) 
    {
        //Update on the clients
        inputVelocity = _velocity;
        position = _position;
    }
    //Update the player rotation and camera rotation on the server
    [ServerRPC]
    private void UpdatePlayerRotationsOnServer(float _playerRotationY, float _cameraRotationX, ulong clientID)
    {
        //It doesnt matter if the player is cheating or not when setting the rotations
        playerRotationY = _playerRotationY;
        cameraRotationX = _cameraRotationX;

        InvokeClientRpcOnEveryoneExcept(UpdatePlayerRotationsOnClient, clientID, _playerRotationY, _cameraRotationX, sendChannel);
    }
    //Update the player rotation and camera rotation on the clients
    [ClientRPC]
    private void UpdatePlayerRotationsOnClient(float _playerRotationY, float _cameraRotationX)
    {
        playerRotationY = _playerRotationY;
        cameraRotationX = _cameraRotationX;
    }
    #endregion
    #region Player positioning
    //Resets the player position and velocity
    public void ResetPlayer()
    {
        //Reset the position
        MovePlayerToPosition(playerSpawn.position);

        inputVelocity = Vector3.zero;
    }
    //Moves the player to a certain position
    private void MovePlayerToPosition(Vector3 _position)
    {
        transform.position = _position;
        Physics.SyncTransforms();
    }
    #endregion
    //When the player hits something
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y < 0.9) return;//Discard the collision if it wasn't under the player
        friction = baseFriction;//Since we are on the ground, reset the friction (The friction changes if we are in air)
        //If we hit a PhysicsObject
        if (hit.gameObject.GetComponent<PhysicsObjectScript>() != null)
        {
            //Override the current friction
            friction = hit.gameObject.GetComponent<PhysicsObjectScript>().friction;
        }
    }
}