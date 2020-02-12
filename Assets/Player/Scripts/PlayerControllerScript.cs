using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Controls the camera and movement of the player from keyboard and mouse
public class PlayerControllerScript : MonoBehaviour
{
    [Header("Camera Movements")]
    public float Sensivity;//The sensivity of the camera rotation
    public Camera Camera;//The actual camera gameobject
    public float MinRotationDown;//The minimun rotation when looking down
    public float MaxRotationUp;//The maximum rotation when looking up
    private float CameraRotationXAxis;
    [Header("Player Movement")]
    public float IdleFov;//The FOV of the camera when the player is not moving (idle)
    public float WalkingFov;//The FOV of the camera when walking
    public float SprintingFov;//The FOV of the camera when sprinting
    public float WalkingSpeed; //The speed of movement of the player
    public float SprintingSpeed; //The sprinting speed of the movement of the player
    public float Gravity;//How much gravity is applied to the player
    public float Jump;//How high we can jump
    private CharacterController characterController;//Variable for the charachter controller
    private Vector3 Movement;//Vector3 that is applied to charachterController
    private Vector2 inputMovement;//Input data from keyboard WASD
    private float Speed;//The overall speed of the player (Smoothed between the walking speed and sprinting speed)
    public float decelerationFactor;//How much you decelerate in general (Used for ice and other physics materials)
    public float maxSpeed;//Max speed allowed to walk
    private bool isWalking;
    private bool isSprinting;
    private float walkingFactor;//Value used to lerp between fov when walking
    public float walkingFactorSpeed;//How fast to make walking factor go to 1
    private float sprintingFactor;//Value used to lerp between fov when sprinting
    public float sprintingFactorSpeed;//How fast to make sprinting factor go to 1
    private float camFOV;//Current camera fov

    // Start is called before the first frame update
    void Start()
    {
        #region Cursor Setup
        Cursor.lockState = CursorLockMode.Locked;//Locks the cursor to the middle of the screen
        Cursor.visible = false;//Make the cursor invisible
        #endregion
        characterController = GetComponent<CharacterController>();//Sets the CharachterController from the component
    }

    // Update is called once per frame
    void Update()
    {
        #region Camera Control
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * Sensivity));//Rotate the whole player around and around
        CameraRotationXAxis -= Input.GetAxis("Mouse Y") * Sensivity;//Sets the up-down value of camera rotation
        CameraRotationXAxis = Mathf.Clamp(CameraRotationXAxis, MinRotationDown, MaxRotationUp);
        Camera.transform.localEulerAngles = new Vector3(CameraRotationXAxis, 0, 0);//Rotates the camera up-down motion from variable
        Camera.fieldOfView = GetCameraFOV(IdleFov, WalkingFov, SprintingFov, walkingFactor, sprintingFactor);//Changes the FOV of the player camera if walking
        #endregion
        #region Player Movement Control
        inputMovement.x = Input.GetAxis("LeftRight"); inputMovement.y = Input.GetAxis("ForwardBackward");//Set input movement values
        Speed = Mathf.Lerp(WalkingSpeed, SprintingSpeed, sprintingFactor);//Lerp between walking speed and sprinting speed with the left shift button axis to smooth out the transition
        Movement.x = inputMovement.x * Speed * Time.deltaTime;//Left/right movement
        Movement.z = inputMovement.y * Speed * Time.deltaTime;//Forward/backwawrd movement
     
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
        }//Only allows us to jump when we are touching ground
        Movement.y -= Gravity * Time.deltaTime;//Applies gravity as acceleration
        characterController.Move(Movement + (1-decelerationFactor) * new Vector3(Mathf.Clamp(characterController.velocity.x, -maxSpeed, maxSpeed), 0, Mathf.Clamp(characterController.velocity.z, -maxSpeed, maxSpeed)) * Time.deltaTime);//Moves the characterController by the Movement Vector
        #endregion
        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Cursor.lockState = CursorLockMode.None;//Unlocks the cursor from the middle of the screen
                Cursor.visible = true;//Make the cursor visible
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Cursor.lockState = CursorLockMode.Locked;//Locks the cursor to the middle of the screen
                Cursor.visible = false;//Make the cursor invisible
            }
        }//Allow debug stuff
    }
    //Three value interpolation for idle, walking and sprinting fov values
    private float GetCameraFOV(float idle, float walk, float sprint, float walkfactor, float sprintfactor) 
    {
        camFOV = Mathf.Lerp(Mathf.Lerp(idle, walk, walkfactor), sprint, sprintfactor);//Lerp of lerp
        return camFOV;
    }
    /*
    void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            GUI.Box(new Rect(0, 0, 200, 200), "");
            GUI.Label(new Rect(100, 0, 100, 30), "Sensivity");
            GUI.Label(new Rect(100, 25, 100, 30), "Walking Speed");
            GUI.Label(new Rect(100, 50, 100, 30), "Jump");
            GUI.Label(new Rect(100, 75, 100, 30), "Gravity");
            GUI.Label(new Rect(100, 100, 100, 30), "Sprinting Speed");
            GUI.Label(new Rect(100, 125, 100, 30), "Walking FOV");
            GUI.Label(new Rect(100, 150, 100, 30), "Sprinting FOV");
            Sensivity = float.Parse(GUI.TextField(new Rect(0, 0, 100, 30), Sensivity.ToString()));
            WalkingSpeed = float.Parse(GUI.TextField(new Rect(0, 25, 100, 30), WalkingSpeed.ToString()));
            Jump = float.Parse(GUI.TextField(new Rect(0, 50, 100, 30), Jump.ToString()));
            Gravity = float.Parse(GUI.TextField(new Rect(0, 75, 100, 30), Gravity.ToString()));
            SprintingSpeed = float.Parse(GUI.TextField(new Rect(0, 100, 100, 30), SprintingSpeed.ToString()));
            WalkingFov = float.Parse(GUI.TextField(new Rect(0, 125, 100, 30), WalkingFov.ToString()));
            SprintingFovAdd = float.Parse(GUI.TextField(new Rect(0, 150, 100, 30), SprintingFovAdd.ToString()));
        }
    }//Debugging GUI stuff*/
}
 