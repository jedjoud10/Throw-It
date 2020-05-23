using MLAPI;
using MLAPI.Messaging;
using MLAPI.Serialization;
using MLAPI.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Snowball throwing
public class ThrowableThrowingScript : NetworkedBehaviour
{
    public GameObject prefab;//The snowball prefab that we are going to throw
    public Transform throwPoint;//The point where the snowball is throwed
    private GameObject instanceThrowObject;//The instance of the instanced object    
    /*
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }*/
    //Spawns the object on the server (with random parameters) (not called from clients)
    public void ThrowOnServer(float speedFactor, string owner, ulong hostClientID)
    {
        instanceThrowObject = Instantiate(prefab, throwPoint.position, throwPoint.rotation);//Throw snowball and set that spawned snoball as our variable so we can call the InitSnowball method
        ThrowablePropertiesScript properties = instanceThrowObject.GetComponent<ThrowablePropertiesScript>();
        properties.RandomizeValues();
        properties.InitSnowball(owner);
        instanceThrowObject.GetComponent<ThrowableMovementScript>().InitSnowball(speedFactor);//Init the snowball with taking account the charging
        InvokeClientRpcOnEveryoneExcept(ThrowOnClient, hostClientID, speedFactor, throwPoint.position, throwPoint.rotation, properties.speed, properties.size, properties.angularVelocity, properties.rigidbodyForce, properties.damage, owner);
    }
    #region Player throwing
    [ServerRPC]
    //Spawns the object on the server
    public void ThrowOnServer(float speedFactor, Vector3 pos, Quaternion rot, ulong clientID, float _speed, float _size, Vector3 _angularVelocity, float _rigidbodyForce, int _damage, string owner)
    {
        InvokeClientRpcOnEveryoneExcept(ThrowOnClient, clientID, speedFactor, pos, rot, _speed, _size, _angularVelocity, _rigidbodyForce, _damage, owner);
    }
    public void Throw(float speedFactor, string owner)//Throw snowball method (Client side only)
    {
        instanceThrowObject = Instantiate(prefab, throwPoint.position, throwPoint.rotation);//Throw snowball and set that spawned snoball as our variable so we can call the InitSnowball method
        ThrowablePropertiesScript properties = instanceThrowObject.GetComponent<ThrowablePropertiesScript>();
        properties.RandomizeValues();
        properties.InitSnowball(owner);
        instanceThrowObject.GetComponent<ThrowableMovementScript>().InitSnowball(speedFactor);//Init the snowball with taking account the charging
        InvokeServerRpc(ThrowOnServer, speedFactor, throwPoint.position, throwPoint.rotation, OwnerClientId, properties.speed, properties.size, properties.angularVelocity, properties.rigidbodyForce, properties.damage, owner);
    }
    [ClientRPC]
    //Spawns the object on all the clients except the owner
    private void ThrowOnClient(float speedFactor, Vector3 pos, Quaternion rot, float _speed, float _size, Vector3 _angularVelocity, float _rigidbodyForce, int _damage, string owner) 
    {
        instanceThrowObject = Instantiate(prefab, pos, rot);//Throw snowball and set that spawned snoball as our variable so we can call the InitSnowball method
        ThrowablePropertiesScript properties = instanceThrowObject.GetComponent<ThrowablePropertiesScript>();
        properties.SetValues(_speed, _size, _angularVelocity, _rigidbodyForce, _damage);
        properties.InitSnowball(owner);
        instanceThrowObject.GetComponent<ThrowableMovementScript>().InitSnowball(speedFactor);//Init the snowball with taking account the charging
    }
    #endregion
}