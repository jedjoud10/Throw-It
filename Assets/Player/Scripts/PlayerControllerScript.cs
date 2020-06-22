using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using UnityEngine;
[RequireComponent(typeof(EntityMovementScript))]
//Controls the camera and movement of the player from keyboard and mouse (V3)
//TODO : Make the local client snap back to the correct position
public class PlayerControllerScript : NetworkedBehaviour
{
    [Header("Control")]
    public bool controllable;//Can the player move/rotate using the input ?
    public float mouseSensivity = 1.0f;//How much to scale the mouse input values before passing them to the playeRotation/cameraRotation
    public float walkingSpeed, sprintingSpeed;//All the possible FOVs based on the state of the player
    public float jump;//How much the player can jump        
    public float factorSmoothingSpeed;//How fast a Factor (WalkingFactor or SprintingFactor) goes to it's correct value
    [Header("Objects")]
    public Camera playerCamera;//The camera (Disabled on non-local players)
    public GameObject headObject;//The head model (Disabled on the local player)
    public GameObject playerObject;//The player model (Disabled on the local player)
    [Header("FOV")]
    //All the possible FOVs based on the state of the player
    public float idleFOV;
    public float walkingFOV;
    public float sprintingFOV;
    [Header("Networking")]
    //public float positionSmoothing = 15;//How much to smooth the position the server gave us
    public float rotationSmoothing = 15;//How much to smooth the rotation the server gave us

    #region Literal Hell - Don't open
    private EntityMovementScript movementScript;
    private Rigidbody rb;//The rigidbody of this player
    private float playerRotationY;//The Y rotation of the player
    private float cameraRotationX;//The X rotation of the camera
    private Vector3 worldVelocity;//The velocity that we want the player to be at
    private float walkingFactor, sprintingFactor;//Two factors for the player to smoothly transition between values
    private float speed;//The current speed smoothed from WalkingSpeed and SprintingSpeed
    const string sendChannel = "UnreliableOrdered";//The channel where we are going to send the player data
    private float cameraRotationXServer;//Data from server
    private bool wantsToJump;//If the player is holding the jump button or not
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movementScript = GetComponent<EntityMovementScript>();
        //Activate camera on this local player only
        playerCamera.gameObject.SetActive(IsLocalPlayer);
        //Let only the local player control itself
        movementScript.apply = IsLocalPlayer;
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
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer) 
        {
            cameraRotationX = Mathf.Lerp(cameraRotationX, cameraRotationXServer, rotationSmoothing * Time.deltaTime);
            playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);
            headObject.transform.localRotation = playerCamera.transform.localRotation;
            return;
        };

        //This code is only ran on the local client machine

        #region Walking/Sprinting factors

        //WalkingFactor smoothly goes to 1 when we are walking
        walkingFactor = Mathf.Lerp(walkingFactor, (worldVelocity.magnitude > 0.0f) ? 1 : 0, factorSmoothingSpeed * Time.deltaTime);
        //SprintingFactor smoothly goes to 1 when we are sprinting
        sprintingFactor = Mathf.Lerp(sprintingFactor, InputManager.GetKey("Sprint") ? 1 : 0, factorSmoothingSpeed * Time.deltaTime);

        //Set the new calculated speed from the SprintingFactor
        speed = Mathf.Lerp(walkingSpeed, sprintingSpeed, sprintingFactor);//Only sprinting factor this time
        movementScript.speed = speed;//Apply the speed

        //Set the new calculated FOV from the WalkingFactor and SprintingFactor
        playerCamera.fieldOfView = Mathf.Lerp(idleFOV, Mathf.Lerp(walkingFOV, sprintingFOV, sprintingFactor), walkingFactor);
        #endregion         
        #region Input

        //Read the input
        worldVelocity = Vector3.zero; worldVelocity.x = 0; worldVelocity.z = 0;
        wantsToJump = false;
        if (controllable)
        {
            //Movement left / right
            if (InputManager.GetKey("Left"))
            {
                worldVelocity.x = -speed;
            }
            else if (InputManager.GetKey("Right"))
            {
                worldVelocity.x = speed;
            }
            //Movement backwards / forwards
            if (InputManager.GetKey("Backward"))
            {
                worldVelocity.z = -speed;
            }
            else if (InputManager.GetKey("Forward"))
            {
                worldVelocity.z = speed;
            }
            wantsToJump = InputManager.GetKey("Jump");

            worldVelocity = transform.TransformDirection(worldVelocity);
            //Apply the velocity to the movement script

            //Rotate the player left and right
            playerRotationY += Input.GetAxis("Mouse X") * mouseSensivity;
            //Rotate the camera up and down
            cameraRotationX -= Input.GetAxis("Mouse Y") * mouseSensivity;
            //Clamp the head rotation because necks can absolutely bend over infinitely
            cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);
            //Rotate the player on the local client
            rb.rotation = Quaternion.Euler(0, playerRotationY, 0);
            //Rotate the camera on the local client
            playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);
            headObject.transform.localRotation = playerCamera.transform.localRotation;
            InvokeServerRpc(UpdatePlayerStateOnServer, cameraRotationX, sendChannel);
        }
        movementScript.inputVelocity.x = worldVelocity.x;
        movementScript.inputVelocity.y = worldVelocity.z;



        #endregion
    }
    private void OnCollisionStay(Collision collision)
    {
        float minDotNormal = 0;
        foreach (var contacts in collision.contacts)
        {
            Debug.DrawRay(contacts.point, contacts.normal);
            if (Vector3.Dot(contacts.normal, Vector3.up) > minDotNormal)
            {
                minDotNormal = Vector3.Dot(contacts.normal, Vector3.up);//Check if there is ground below us
            }
        }
        if(minDotNormal > 0.1f && wantsToJump) 
        {
            Vector3 vel = rb.velocity;
            vel.y = jump;
            rb.velocity = vel;
        }
    }
    #region Networking
    //Update the state of the player on the server
    [ServerRPC]
    public void UpdatePlayerStateOnServer(float headRotation) 
    {
        InvokeClientRpcOnEveryone(UpdatePlayerStateOnClient, headRotation);
    }
    //Update the state of the player on the clients
    [ClientRPC]
    private void UpdatePlayerStateOnClient(float headRotation) 
    {
        cameraRotationXServer = headRotation;
    }
    [ClientRPC]
    //Sets this player location (only ran on server)
    public void SetPlayerPositionOnServer(Vector3 _position) 
    {
        InvokeClientRpcOnClient(SetPlayerPositionOnOwnerClient, OwnerClientId, _position);
    }
    //Sets this player location (only ran on the owner client)
    [ClientRPC]
    private void SetPlayerPositionOnOwnerClient(Vector3 _position) 
    {
        rb.position = _position;
    }
    #endregion
}