using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Tilts the bot in the slope that the terrain or object it is under so it is at always flat with the slope

//TODO : Networking support
public class BotTiltingScript : NetworkedBehaviour
{
    private BotScript botscript;//Script for this specific bot
    private float tiltingPositionY;//How much to tilt the bot by the Quaternion.lookAt() function
    private RaycastHit hit;//Raycast hit to get hit.point.y value and calculate slope
    public float forwardDistance;//How much to move the raySTartPos from the forward vector in that direction
    public float upDistance;//How much to move the raySTartPos up
    public float Speed;//The speed at how much we adjust our tilting to match up the terrain slope
    public float Offset;//Offset of y value of hit point
    // Start is called before the first frame update
    void Start()
    {
        botscript = GetComponent<BotScript>();//Auto set value
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(getPointTransformed() + transform.position, Vector3.down * 100, out hit)) 
        {
            tiltingPositionY = Mathf.Lerp(tiltingPositionY, hit.point.y - transform.position.y, Speed * Time.deltaTime);//Get end point height and give it to the movement script to handle tilting
            //botscript.movementScript.TiltPositionY = tiltingPositionY + Offset;
        }
    }
    //Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(getPointTransformed() + transform.position, 0.1f);
    }
    //Get position of certain point but relative only to direction of bot in xz
    private Vector3 getPointTransformed() 
    {
        Vector2 pos2D = new Vector2(transform.forward.normalized.x, transform.forward.normalized.z);
        return new Vector3(pos2D.normalized.x * forwardDistance, upDistance, pos2D.normalized.y * forwardDistance);
    }
}
