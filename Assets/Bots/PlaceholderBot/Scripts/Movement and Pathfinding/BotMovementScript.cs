using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
//A script that handles comunication between the Charachter Controller and other scripts. It allows us to move this gameObject into a specified position
public class BotMovementScript : MonoBehaviour
{
    [Header("Bot Movement")]
    public bool move = true;//Are we allowed to move ?
    public float Speed = 1;//How fast does the bot move
    public float RotationSpeed = 1;//How fast does the bot turn directions
    const float Gravity = 0.1050505f;//How much gravity is applied to this bot
    public float decelerationFactor;//Variable as base variable so we can always reset the decelerationFactor

    const float MoveSmoothness = 0.8f;//Smoothness when changing from idle to moving or vice versa
    private CharacterController cr;//The character controller of this bot
    private Vector3 position;//The chosen position to head to
    private Vector3 currentPosition;//The current position of the bot
    private Vector3 Movement;//The movement applied to the character controller
    private Vector2 UnscaledMovement;//The movement unscaled from the time.deltaTime
    private Vector3 HeadingMovement = Vector3.zero;//The x and z movement in a vector 2d
    private Quaternion Rotation;//The target rotation of the bot
    private float _decelerationFactor;//How much you decelerate in general (Used for ice and other physics materials)
    private Vector3 lastMovement;//Movement last fram
    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;//Sets the target pos as our own so we dont go to the middle of the world
        cr = GetComponent<CharacterController>();//Set the character controller to our own
        _decelerationFactor = decelerationFactor;//Setup decelerationFactor
    }

    // Update is called once per frame
    void Update()
    {
        #region Movement & Rotation
        #region Normalization of position & movement
        currentPosition = transform.position;//Save to variable to save on performence
        //We use the normalized value so when the bot gets closer to the position, it's speed stays constant and does not decrease
        if (move)
        {
            Movement.x = (position - currentPosition).normalized.x * Time.deltaTime * Speed;//Delta movement of the position that we want to go in X axis
            Movement.z = (position - currentPosition).normalized.z * Time.deltaTime * Speed;//Delta movement of the position that we want to go in Z axis
        }
        else Movement.x = Mathf.Lerp(Movement.x, 0, MoveSmoothness * Time.deltaTime); Movement.z = Mathf.Lerp(Movement.z, 0, MoveSmoothness * Time.deltaTime);//Stop moving but allow gravity. Also it is smoothed out
        #endregion
        if (UnscaledMovement != Vector2.zero)//Checks if the movement is higher than 0 in x and z axis so we dont get an error when we try to look at rotation
        {
            HeadingMovement.x = Movement.x;
            HeadingMovement.z = Movement.z;            
            Rotation = Quaternion.LookRotation(HeadingMovement);//Target rotation without Y axis
        }
        transform.rotation = Quaternion.Slerp(Rotation, transform.rotation, RotationSpeed);//Smoothes the rotation
        if (cr.isGrounded)
        {
            Movement.y = 0;//Dont apply gravity if already in ground
        }
        Debug.DrawRay(currentPosition, Movement * 100);
        Movement.y -= Gravity * Time.deltaTime;//Apply gravity
        lastMovement.y = Movement.y;//Same gravity so it doesnt lerp between gravities
        lastMovement = Vector3.Lerp(lastMovement, Movement, _decelerationFactor * Time.deltaTime);//Set last frame movement
        cr.Move(lastMovement);//Apply gravity & position direction movement to charachter controller. Also with deceleration
        UnscaledMovement.x = cr.velocity.x; UnscaledMovement.y = cr.velocity.z;//Unscaled movement from velocity 
        #endregion
    }
    public void MoveToPosition(Vector3 _position) 
    {
        position = _position;//set the position
    }
    #region Collisions
    //When collision happens
    /*
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GameObject otherObject = hit.gameObject;
        if (otherObject.GetComponent<PhysicsObjectScript>() != null) //Is physics object
        {
            _decelerationFactor = otherObject.gameObject.GetComponent<PhysicsObjectScript>().DecelerationFactor;//Set new deceleration factor
        }
        else
        {
            _decelerationFactor = decelerationFactor;//Reset the decelerationFactor to base
        }
    }*/
    #endregion
}
