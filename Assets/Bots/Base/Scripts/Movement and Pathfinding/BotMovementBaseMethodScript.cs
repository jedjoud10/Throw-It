using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
[RequireComponent(typeof(EntityMovementScript))]
//How a bot can move using a specific method
public class BotMovementBaseMethodScript : NetworkedBehaviour
{
    [Header("Movement")]
    public float rotationSmoothing;//How much to smooth between the current rotation and the target rotation

    #region Literal Hell : The Sequel    
    protected EntityMovementScript movementScript;
    [HideInInspector]
    public BotScript botScript;//The bot script of this bot
    protected Quaternion targetRotation = Quaternion.identity;
    #endregion
    // Start is called before the first frame update
    virtual public void Start()
    {
        movementScript = GetComponent<EntityMovementScript>();
    }

    // Update is called once per frame
    virtual public void Update()
    {
        if (!IsServer) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothing * Time.deltaTime);
    }
}
