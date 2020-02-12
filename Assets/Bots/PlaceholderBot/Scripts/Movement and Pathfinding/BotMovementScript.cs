using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
//A script that handles comunication between the Charachter Controller and other scripts. It allows us to move this gameObject into a specified position
public class BotMovementScript : MonoBehaviour
{
    [Tooltip("How fast does the bot move")]
    public float Speed = 1;//How fast does the bot move
    [Tooltip("How fast does the bot turn directions")]
    public float RotationSpeed = 1;
    const float Gravity = 0.1050505f;//How much gravity is applied to this bot
    const float MoveSmoothness = 0.8f;//Smoothness when changing from idle to moving or vice versa
    private CharacterController cr;//The character controller of this bot
    private Vector3 position;//The chosen position to head to
    private Vector3 Movement;//The movement applied to the character controller
    private Vector2 UnscaledMovement;//The movement unscaled from the time.deltaTime
    private Quaternion Rotation;//The target rotation of the bot
    public bool move = true;//Are we allowed to move ?
    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;//Sets the target pos as our own so we dont go to the middle of the world
        cr = GetComponent<CharacterController>();//Set the character controller to our own
    }

    // Update is called once per frame
    void Update()
    {
        #region Movement & Rotation
        #region Normalization of position & movement
        //We use the normalized value so when the bot gets closer to the position, it's speed stays constant and does not decrease
        if (move)
        {
            Movement.x = (position - transform.position).normalized.x * Time.deltaTime * Speed;//Delta movement of the position that we want to go in X axis
            Movement.z = (position - transform.position).normalized.z * Time.deltaTime * Speed;//Delta movement of the position that we want to go in Z axis
        }
        else Movement.x = Mathf.Lerp(Movement.x, 0, MoveSmoothness * Time.deltaTime); Movement.z = Mathf.Lerp(Movement.z, 0, MoveSmoothness * Time.deltaTime);//Stop moving but allow gravity. Also it is smoothed out
        #endregion
        if (UnscaledMovement != Vector2.zero)//Checks if the movement is higher than 0 in x and z axis so we dont get an error when we try to look at rotation
        {
            Rotation = Quaternion.LookRotation(new Vector3(Movement.x, 0, Movement.z));//Target rotation without Y axis
        }
        transform.rotation = Quaternion.Slerp(Rotation, transform.rotation, RotationSpeed);//Smoothes the rotation
        if (cr.isGrounded)
        {
            Movement.y = 0;//Dont apply gravity if already in ground
        }
        Debug.DrawRay(transform.position, Movement * 100);
        Movement.y -= Gravity * Time.deltaTime;//Apply gravity
        cr.Move(Movement);//Apply gravity & position direction movement to charachter controller
        UnscaledMovement.x = cr.velocity.x; UnscaledMovement.y = cr.velocity.z;//Unscaled movement from velocity 
        #endregion
    }
    public void MoveToPosition(Vector3 _position) 
    {
        position = _position;//set the position
    }
}
