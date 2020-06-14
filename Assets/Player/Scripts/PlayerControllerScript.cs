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
    private Rigidbody rb;//The rigidbody of this player
    private float playerRotationY;//The Y rotation of the player
    private float cameraRotationX;//The X rotation of the camera
    private Vector2 inputData;//The input velocity plane (x, z) from the keyboard
    private bool jumping;//If the local player is jumping
    private float walkingFactor, sprintingFactor;//Two factors for the player to smoothly transition between values
    private float speed;//The current speed smoothed from WalkingSpeed and SprintingSpeed
    const string sendChannel = "UnreliableOrdered";//The channel where we are going to send the player data
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
    }
    // Update is called once per frame
    void Update()
    {
        //This code is only ran on the local client machine
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
        #region Input
        //Read the input
        inputData.x = 0; inputData.y = 0; jumping = false;
        if (controllable)
        {
            //Movement left / right
            if (InputManager.GetKey("Left")) 
            {
                inputData.x = -speed;
            }
            else if(InputManager.GetKey("Right"))
            {
                inputData.x = speed;
            }
            //Movement backwards / forwards
            if (InputManager.GetKey("Backward"))
            {
                inputData.y = -speed;
            }
            else if (InputManager.GetKey("Forward"))
            {
                inputData.y = speed;
            }
            jumping = InputManager.GetKeyPress("Jump");

            //Rotate the player left and right
            playerRotationY += Input.GetAxis("Mouse X") * mouseSensivity;
            //Rotate the camera up and down
            cameraRotationX -= Input.GetAxis("Mouse Y") * mouseSensivity;
            //Clamp the head rotation because necks can absolutely bend over infinitely
            cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);
        }
        if (jumping)
        {
            rb.AddForce(0, jump, 0, ForceMode.Impulse);
        }
        //Rotate the player on the local client
        rb.rotation = Quaternion.Euler(0, playerRotationY, 0);
        //Rotate the camera on the local client
        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);
        headObject.transform.localRotation = playerCamera.transform.localRotation;
        #endregion

    }
    // FixedUpdate is called each physics timestep
    void FixedUpdate()
    {
        Vector3 playerSpeed = rb.velocity;
        playerSpeed.y = 0;
        rb.AddRelativeForce(((new Vector3(inputData.x, 0, inputData.y)) - playerSpeed) * Time.fixedDeltaTime, ForceMode.Force);
    }
    //Sets this player location (only ran on local client)
    public void SetPlayerPosition(Vector3 _position) 
    {
        rb.position = _position;
    }
}