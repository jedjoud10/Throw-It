using MLAPI;
using MLAPI.Messaging;
using MLAPI.Serialization;
using MLAPI.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Snowball throwing
public class SnowballThrowingScript : NetworkedBehaviour
{
    public GameObject snowball;//The snowball prefab that we are going to throw
    public Transform throwPoint;//The point where the snowball is throwed
    private GameObject instanceSnowball;//The instance of the Snowball var    
    /*
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }*/
    //Spawns the snowball on the server (with random parameters) (not called from clients)
    public void ThrowSnowballOnServer(float speedFactor, string owner, ulong hostClientID)
    {
        instanceSnowball = Instantiate(snowball, throwPoint.position, throwPoint.rotation);//Throw snowball and set that spawned snoball as our variable so we can call the InitSnowball method
        SnowballPropertiesScript properties = instanceSnowball.GetComponent<SnowballPropertiesScript>();
        properties.RandomizeValues();
        properties.InitSnowball(owner);
        instanceSnowball.GetComponent<SnowballMovementScript>().InitSnowball(speedFactor);//Init the snowball with taking account the charging
        InvokeClientRpcOnEveryoneExcept(ThrowSnowballOnClient, hostClientID, speedFactor, throwPoint.position, throwPoint.rotation, properties.speed, properties.size, properties.angularVelocity, properties.rigidbodyForce, properties.damage, owner);
    }
    #region Player throwing snowballs
    [ServerRPC]
    //Spawns the snowball on the server
    public void ThrowSnowballOnServer(float speedFactor, Vector3 pos, Quaternion rot, ulong clientID, float _speed, float _size, Vector3 _angularVelocity, float _rigidbodyForce, int _damage, string owner)
    {
        InvokeClientRpcOnEveryoneExcept(ThrowSnowballOnClient, clientID, speedFactor, pos, rot, _speed, _size, _angularVelocity, _rigidbodyForce, _damage, owner);
    }
    public void ThrowSnowball(float speedFactor, string owner)//Throw snowball method (Client side only)
    {
        instanceSnowball = Instantiate(snowball, throwPoint.position, throwPoint.rotation);//Throw snowball and set that spawned snoball as our variable so we can call the InitSnowball method
        SnowballPropertiesScript properties = instanceSnowball.GetComponent<SnowballPropertiesScript>();
        properties.RandomizeValues();
        properties.InitSnowball(owner);
        instanceSnowball.GetComponent<SnowballMovementScript>().InitSnowball(speedFactor);//Init the snowball with taking account the charging
        InvokeServerRpc(ThrowSnowballOnServer, speedFactor, throwPoint.position, throwPoint.rotation, OwnerClientId, properties.speed, properties.size, properties.angularVelocity, properties.rigidbodyForce, properties.damage, owner);
    }
    [ClientRPC]
    //Spawns the snowball on all the clients except the owner
    private void ThrowSnowballOnClient(float speedFactor, Vector3 pos, Quaternion rot, float _speed, float _size, Vector3 _angularVelocity, float _rigidbodyForce, int _damage, string owner) 
    {
        instanceSnowball = Instantiate(snowball, pos, rot);//Throw snowball and set that spawned snoball as our variable so we can call the InitSnowball method
        SnowballPropertiesScript properties = instanceSnowball.GetComponent<SnowballPropertiesScript>();
        properties.SetValues(_speed, _size, _angularVelocity, _rigidbodyForce, _damage);
        properties.InitSnowball(owner);
        instanceSnowball.GetComponent<SnowballMovementScript>().InitSnowball(speedFactor);//Init the snowball with taking account the charging
    }
    #endregion
}