using MLAPI;
using MLAPI.NetworkedVar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Tilts the bot in the slope that the terrain or object it is under so it is at always flat with the slope
public class BotTiltingScript : NetworkedBehaviour
{
    public Vector3 contactPoint;//The very bottom of the bot (Where the bot is supposed to touch the ground)
    public float smoothing;//How much to smooth the angle
    private NetworkedVarVector3 targetPoint = new NetworkedVarVector3();//The point that the raycast hit
    private BotScript botScript;//The script of the bot

    private Vector2 rayOriginDirection;//Used to calculate the ray origin
    private Vector3 rayOriginPosition;//The ray origin
    private RaycastHit hit;//no.
    private float angle;//The angle that was calculated
    // Start is called before the first frame update
    void Start()
    {        
        botScript = GetComponent<BotScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            //First calculate the direction of the origin since it might give wrong results (Because the Bot is rotated)
            rayOriginDirection = new Vector2(transform.forward.x, transform.forward.z);
            rayOriginDirection.Normalize();
            //Turn the 2D direction into a 3D position
            rayOriginPosition = new Vector3(rayOriginDirection.x + transform.position.x, transform.position.y + 1, rayOriginDirection.y + transform.position.z);
            if(Physics.Raycast(rayOriginPosition, Vector3.down * 2, out hit)) 
            {
                targetPoint.Value = hit.point;
            }
        }
        //Calculate the angle
        angle = Mathf.Lerp(angle, Vector3.Angle(transform.position + contactPoint - targetPoint.Value, Vector3.down) - 90, smoothing * Time.deltaTime);
        //Update the rotation offset of the bot
        botScript.rotationOffsetX = angle;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + contactPoint, 1);//Yes ? NOOOOOOOOOOOOOOOO
    }
}
