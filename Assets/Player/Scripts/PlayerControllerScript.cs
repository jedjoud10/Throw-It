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
    [Range(1, 3)]
    public float WalkSprintSmoothness;//How much smoothness to apply between the sprinting and walking
    [Range(0.1f, 3)]
    public float WalkSprintSmoothnessFovInput;//How much smoothness to apply between the sprinting and walking when moving
    public float WalkingFov;//The FOV of the camera when walking
    public float WalkingSpeed; //The speed of movement of the player
    public float SprintingSpeed; //The sprinting speed of the movement of the player
    public float SprintingFovAdd;//The FOV of the camera that will be added to the base FOV
    public float Gravity;//How much gravity is applied to the player
    public float Jump;//How high we can jump
    private CharacterController characterController;//Variable for the charachter controller
    private Vector3 Movement;//Vector3 that is applied to charachterController
    private float Speed;//The overall speed of the player (Smoothed between the walking speed and sprinting speed)

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
        Camera.fieldOfView = Mathf.Lerp(WalkingFov, WalkingFov + (SprintingFovAdd * FOVFromInputMovement()), Input.GetAxis("Sprint") * WalkSprintSmoothness);//Changes the FOV of the player camera if sprinting. Also takes account the player velocity
        #endregion
        #region Player Movement Control
        Speed = Mathf.Lerp(WalkingSpeed, SprintingSpeed, Input.GetAxis("Sprint") * WalkSprintSmoothness);//Lerp between walking speed and sprinting speed with the left shift button axis to smooth out the transition
        Movement.x = Input.GetAxis("LeftRight") * Speed * Time.deltaTime;//Left/right movement
        Movement.z = Input.GetAxis("ForwardBackward") * Speed * Time.deltaTime;//Forward/backwawrd movement
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
        characterController.Move(Movement);//Moves the characterController by the Movement Vector
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
    //Returns the fov the camera of the player needs to be based on the speed of the player, so if the player does not move, the fov will be normal, if they do, it will increase
    private float FOVFromInputMovement()
    {
        //Inputs from keyboard
        //Take absolute number since range is from -1 to 1 and we need the range to be from 0 to 1
        float x = Mathf.Abs(Input.GetAxis("LeftRight"));
        float z = Mathf.Abs(Input.GetAxis("ForwardBackward"));
        return Mathf.Clamp((x + z) * WalkSprintSmoothnessFovInput, 0, 1);//Get the average of both speeds
    }
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
            GUI.Label(new Rect(100, 175, 100, 30), "Sprinting/Walking Smoothness");
            Sensivity = float.Parse(GUI.TextField(new Rect(0, 0, 100, 30), Sensivity.ToString()));
            WalkingSpeed = float.Parse(GUI.TextField(new Rect(0, 25, 100, 30), WalkingSpeed.ToString()));
            Jump = float.Parse(GUI.TextField(new Rect(0, 50, 100, 30), Jump.ToString()));
            Gravity = float.Parse(GUI.TextField(new Rect(0, 75, 100, 30), Gravity.ToString()));
            SprintingSpeed = float.Parse(GUI.TextField(new Rect(0, 100, 100, 30), SprintingSpeed.ToString()));
            WalkingFov = float.Parse(GUI.TextField(new Rect(0, 125, 100, 30), WalkingFov.ToString()));
            SprintingFovAdd = float.Parse(GUI.TextField(new Rect(0, 150, 100, 30), SprintingFovAdd.ToString()));
            WalkSprintSmoothness = float.Parse(GUI.TextField(new Rect(0, 175, 100, 30), WalkSprintSmoothness.ToString()));
        }
    }//Debugging GUI stuff
}
 