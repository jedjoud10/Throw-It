using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
[RequireComponent(typeof(EntityMovementScript))]
//A script that handles communications between the EntityMovementScript and other scripts. It allows us to move this gameObject to specified position
public class BotMovementScript : NetworkedBehaviour
{
    [Header("Movement")]
    public float rotationSmoothing;//How much to smooth between the current rotation and the target rotation

    #region Literal Hell : The Sequel    
    private EntityMovementScript movementScript;
    [HideInInspector]
    public BotScript botScript;//The bot script of this bot
    private Vector3 destination;//The position that we want the bot to go to
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        movementScript = GetComponent<EntityMovementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        Vector3 localVelocity = Vector3.right;
        localVelocity = transform.TransformDirection(localVelocity);
        movementScript.inputVelocity = new Vector2(localVelocity.x, localVelocity.z);
    }
    #region Positioning
    //Moves the bot to a specified position with a constant speed (Only on server)
    public void MoveToPosition(Vector3 _position) 
    { 
        if (!IsServer) return;
        destination = _position;
    }
    #endregion
}
