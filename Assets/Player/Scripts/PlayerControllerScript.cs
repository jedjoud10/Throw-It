using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Controls the camera and movement of the player from keyboard and mouse
public class PlayerControllerScript : NetworkedBehaviour
{
    [Header("Camera Movements")]
    public float Sensivity;//The sensivity of the camera rotation
    public Camera Camera;//The actual camera gameobject
    public float MinRotationDown;//The minimun rotation when looking down
    public float MaxRotationUp;//The maximum rotation when looking up
    private float CameraRotationXAxis;
    [Header("Player Movement")]
    [Header("----------------")]
    [Header("FOV Control")]
    public float IdleFov;//The FOV of the camera when the player is not moving (idle)
    public float WalkingFov;//The FOV of the camera when walking
    public float SprintingFov;//The FOV of the camera when sprinting
    [Header("Speed")]
    public float WalkingSpeed; //The speed of movement of the player
    public float SprintingSpeed; //The sprinting speed of the movement of the player
    public float walkingFactorSpeed;//How fast to make walking factor go to 1
    public float sprintingFactorSpeed;//How fast to make sprinting factor go to 1
    public float decelerationFactor;//Variable as base variable so we can always reset the decelerationFactor
    public float airControlDecelerationFactor;//The deceleration factor in air
    public float airControllSpeedLoss;//How fast we loose control (smaller decelerationFactor) when we are in air
    [Header("Gravity")]
    public float Gravity;//How much gravity is applied to the player
    public float Jump;//How high we can jump

    private CharacterController characterController;//Variable for the charachter controller
    private Vector3 Movement;//Vector3 that is applied to charachterController
    private Vector3 lastMovement;//Movement last frame
    private Vector2 inputMovement;//Input data from keyboard WASD
    private float Speed;//The overall speed of the player (Smoothed between the walking speed and sprinting speed)
    private float _decelerationFactor;//How much you decelerate in general (Used for ice and other physics materials)
    private bool isWalking;
    private bool isSprinting;
    private float walkingFactor;//Value used to lerp between fov when walking
    private float sprintingFactor;//Value used to lerp between fov when sprinting
    private float camFOV;//Current camera fov    

    // Start is called before the first frame update
    void Start()
    {
        transform.position = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform.position;//Set base position

        if (IsLocalPlayer)
        {

        }
        else
        {
            Camera.gameObject.SetActive(false);//Disable camera for non local players
        }
        
        #region Cursor Setup
        Cursor.lockState = CursorLockMode.Locked;//Locks the cursor to the middle of the screen
        Cursor.visible = false;//Make the cursor invisible
        #endregion
        characterController = GetComponent<CharacterController>();//Sets the CharachterController from the component
        _decelerationFactor = decelerationFactor;//Setup decelerationFactor
        InvokeRepeating("updateFPS", 0.0f, 0.1f);//Update FPS counter each 1/10 a second
    }

    // Update is called once per frame
    void Update()
    {
        #region Camera Control
        if (IsLocalPlayer)
        {
            transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * Sensivity));//Rotate the whole player around and around
            CameraRotationXAxis -= Input.GetAxis("Mouse Y") * Sensivity;//Sets the up-down value of camera rotation

            CameraRotationXAxis = Mathf.Clamp(CameraRotationXAxis, MinRotationDown, MaxRotationUp);
            Camera.transform.localEulerAngles = new Vector3(CameraRotationXAxis, 0, 0);//Rotates the camera up-down motion from variable
            Camera.fieldOfView = GetCameraFOV(IdleFov, WalkingFov, SprintingFov, walkingFactor, sprintingFactor);//Changes the FOV of the player camera if walking

            #endregion
            #region Player Movement Control
            inputMovement.x = Input.GetAxis("LeftRight"); inputMovement.y = Input.GetAxis("ForwardBackward");//Set input movement values
            Speed = Mathf.Lerp(WalkingSpeed, SprintingSpeed, sprintingFactor);//Lerp between walking speed and sprinting speed with the left shift button axis to smooth out the transition
            Movement.x = inputMovement.x * Speed;//Left/right movement
            Movement.z = inputMovement.y * Speed;//Forward/backwawrd movement

            #region Walking and Sprinting values
            isWalking = Mathf.Abs(inputMovement.magnitude) > 0;//Are we walking ?
            isSprinting = isWalking && Input.GetAxis("Sprint") > 0;
            if (isWalking) walkingFactor += (1 - walkingFactor) * walkingFactorSpeed * Time.deltaTime;//Smoothly go to 1
            else walkingFactor -= walkingFactor * walkingFactorSpeed * Time.deltaTime;//Smoothly go to 0
            if (isSprinting) sprintingFactor += (1 - sprintingFactor) * sprintingFactorSpeed * Time.deltaTime;//Smoothly go to 1
            else sprintingFactor -= sprintingFactor * sprintingFactorSpeed * Time.deltaTime;//Smoothly go to 0

            if (walkingFactor > 0.99) walkingFactor = 1;//Snap the value since it will never be 1.0
            if (walkingFactor < 0.01) walkingFactor = 0;//Snap the value since it will never be 0.0
            if (sprintingFactor > 0.99) sprintingFactor = 1;//Snap the value since it will never be 1.0
            if (sprintingFactor < 0.01) sprintingFactor = 0;//Snap the value since it will never be 0.0
            #endregion
            Movement = transform.TransformDirection(Movement);//Takes account the rotation of the player when moving
            if (characterController.isGrounded)//Only allows us to jump when we are touching ground
            {
                Movement.y = 0;//Sets the movement in the Y axis to stop, thus allowing us in-air movement
                if (Input.GetAxis("Jump") > 0.5)//Jumping by the Jump axis
                {
                    Movement.y = Jump;
                }
            }
            else
            {
                _decelerationFactor = Mathf.Lerp(_decelerationFactor, airControlDecelerationFactor, airControllSpeedLoss * Time.deltaTime);
            }
        }
        Movement.y -= Gravity * Time.deltaTime;//Applies gravity as acceleration        
        lastMovement.y = Movement.y;//Same gravity so it doesnt lerp between gravities
        lastMovement = Vector3.Lerp(lastMovement, Movement, _decelerationFactor * Time.deltaTime);//Set last frame movement
        characterController.Move(lastMovement * Time.deltaTime);//Moves the characterController by the Movement Vector and the deceleration
        if(IsLocalPlayer) InvokeServerRpc(UpdatePlayerServer, Movement, lastMovement, transform.rotation, transform.position, OwnerClientId);
        #endregion
    }
    [ServerRPC]
    //Update this player on the server
    public void UpdatePlayerServer(Vector3 velocity, Vector3 lastVelocity, Quaternion rot, Vector3 position, ulong clientID) 
    {
        InvokeClientRpcOnEveryoneExcept(UpdatePlayerClient, clientID, velocity, lastVelocity, rot, position);
        if (Vector3.Distance(transform.position, position) > 2)
        {
            transform.position = position;
        }
    }
    [ClientRPC]
    //Update this player on other clients
    public void UpdatePlayerClient(Vector3 velocity, Vector3 lastVelocity, Quaternion rot, Vector3 position) 
    {
        lastMovement = lastVelocity;
        Movement = velocity;
        transform.rotation = rot;
        if(Vector3.Distance(transform.position, position) > 2) 
        {
            transform.position = position;
        }
        Debug.Log("UpdatePlayerClient distance: " + Vector3.Distance(transform.position, position));
    }
    //Three value interpolation for idle, walking and sprinting fov values
    private float GetCameraFOV(float idle, float walk, float sprint, float walkfactor, float sprintfactor)
    {
        camFOV = Mathf.Lerp(Mathf.Lerp(idle, walk, walkfactor), sprint, sprintfactor);//Lerp of lerp
        return camFOV;
    }
    float fps;//Frames per second
    float deltatime;//Delay in seconds between each frame
    //Updates the fps counter with smoothed values
    private void updateFPS()
    {
        fps = Mathf.Lerp(fps, (1f / Time.unscaledDeltaTime), 0.5f);
        deltatime = Mathf.Lerp(deltatime, Time.unscaledDeltaTime, 0.5f);
    }
    void OnGUI()
    {
        if (Debug.isDebugBuild && IsLocalPlayer)
        {
            float space = 15;
            GUI.Box(new Rect(0, 0, 200, space * 16), "");
            GUI.Label(new Rect(0, 0, 500, 100), "PlayerControllerScript : ");
            GUI.Label(new Rect(10, space * 1, 500, 100), "Movement :");
            GUI.Label(new Rect(30, space * 2, 500, 100), "X : " + lastMovement.x.ToString("F2"));
            GUI.Label(new Rect(30, space * 3, 500, 100), "Y : " + lastMovement.y.ToString("F2"));
            GUI.Label(new Rect(30, space * 4, 500, 100), "Z : " + lastMovement.z.ToString("F2"));
            GUI.Label(new Rect(30, space * 5, 500, 100), "Walking : " + walkingFactor.ToString("F2"));
            GUI.Label(new Rect(30, space * 6, 500, 100), "Sprint : " + sprintingFactor.ToString("F2"));
            GUI.Label(new Rect(30, space * 7, 500, 100), "Speed : " + Speed.ToString("F2"));
            GUI.Label(new Rect(30, space * 8, 500, 100), "Deceleration Factor : " + _decelerationFactor.ToString("F2"));
            GUI.Label(new Rect(10, space * 9, 500, 100), "Input :");
            GUI.Label(new Rect(30, space * 10, 500, 100), "X : " + inputMovement.x.ToString("F2"));
            GUI.Label(new Rect(30, space * 11, 500, 100), "Y : " + inputMovement.y.ToString("F2"));
            GUI.Label(new Rect(10, space * 12, 500, 100), "Performance :");
            GUI.Label(new Rect(30, space * 13, 500, 100), "FPS : " + Mathf.RoundToInt(fps));
            GUI.Label(new Rect(30, space * 14, 500, 100), "Delay : " + deltatime);

        }
    }//Debugging GUI stuff

    #region Collisions
    //When collision happens
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!IsLocalPlayer) return;
        if (hit.normal.y < 0.9f) return;//If object we hit isnt underground us, then discard the collision
        GameObject otherObject = hit.gameObject;
        if (otherObject.GetComponent<PhysicsObjectScript>() != null) //Is physics object
        {
            _decelerationFactor = otherObject.gameObject.GetComponent<PhysicsObjectScript>().DecelerationFactor;//Set new deceleration factor
        }
        else
        {
            _decelerationFactor = decelerationFactor;//Reset the decelerationFactor to base
        }
    }
    #endregion
}