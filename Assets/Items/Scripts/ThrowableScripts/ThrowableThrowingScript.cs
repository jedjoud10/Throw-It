using MLAPI;
using MLAPI.Messaging;
using MLAPI.Serialization;
using MLAPI.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//throwable throwing
public class ThrowableThrowingScript : NetworkedBehaviour
{
    public Transform throwPoint;//The point where the throwable is throwed
    private GameObject instanceThrowObject;//The instance of the instanced object
    // Start is called before the first frame update
    void Start()
    {
        
    }
    //Spawns the object on the server (with random parameters) (not called from clients)
    public void ThrowOnServer(float speedFactor, string owner, ulong hostClientID, int throwableID)
    {
        //Spawn the specific item with a specific id
        Throwable throwable = (Throwable)ItemsHandler.ID2Item(throwableID);
        GameObject prefab = throwable.throwableGameObject;

        instanceThrowObject = Instantiate(prefab, throwPoint.position, throwPoint.rotation);//Throw throwable and set that spawned snoball as our variable so we can call the Initthrowable method
        ThrowablePropertiesScript properties = instanceThrowObject.GetComponent<ThrowablePropertiesScript>();
        properties.LoadItemData(throwableID);
        properties.RandomizeValues();
        properties.InitThrowable(owner);
        instanceThrowObject.GetComponent<ThrowableMovementScript>().InitThrowable(speedFactor);//Init the throwable with taking account the charging
        InvokeClientRpcOnEveryoneExcept(ThrowOnClient, hostClientID, speedFactor, throwPoint.position, throwPoint.rotation, properties.speed, properties.size, properties.angularVelocity, properties.rigidbodyForce, properties.damage, owner, throwableID);
    }
    #region Player throwing
    [ServerRPC]
    //Spawns the object on the server
    public void ThrowOnServer(float speedFactor, Vector3 pos, Quaternion rot, ulong clientID, float _speed, float _size, Vector3 _angularVelocity, float _rigidbodyForce, int _damage, string owner, int throwableID)
    {
        InvokeClientRpcOnEveryoneExcept(ThrowOnClient, clientID, speedFactor, pos, rot, _speed, _size, _angularVelocity, _rigidbodyForce, _damage, owner, throwableID);
    }
    public void Throw(float speedFactor, string owner, int throwableID)//Throw throwable method (Client side only)
    {
        //Spawn the specific item with a specific id
        Throwable throwable = (Throwable)ItemsHandler.ID2Item(throwableID);
        GameObject prefab = throwable.throwableGameObject;

        instanceThrowObject = Instantiate(prefab, throwPoint.position, throwPoint.rotation);//Throw throwable and set that spawned snoball as our variable so we can call the Initthrowable method
        ThrowablePropertiesScript properties = instanceThrowObject.GetComponent<ThrowablePropertiesScript>();
        properties.LoadItemData(throwableID);
        properties.RandomizeValues();
        properties.InitThrowable(owner);
        instanceThrowObject.GetComponent<ThrowableMovementScript>().InitThrowable(speedFactor);//Init the throwable with taking account the charging
        InvokeServerRpc(ThrowOnServer, speedFactor, throwPoint.position, throwPoint.rotation, OwnerClientId, properties.speed, properties.size, properties.angularVelocity, properties.rigidbodyForce, properties.damage, owner, throwableID);
    }
    [ClientRPC]
    //Spawns the object on all the clients except the owner
    private void ThrowOnClient(float speedFactor, Vector3 pos, Quaternion rot, float _speed, float _size, Vector3 _angularVelocity, float _rigidbodyForce, int _damage, string owner, int throwableID) 
    {
        //Spawn the specific item with a specific id
        Throwable throwable = (Throwable)ItemsHandler.ID2Item(throwableID);
        GameObject prefab = throwable.throwableGameObject;

        instanceThrowObject = Instantiate(prefab, pos, rot);//Throw throwable and set that spawned snoball as our variable so we can call the Initthrowable method
        ThrowablePropertiesScript properties = instanceThrowObject.GetComponent<ThrowablePropertiesScript>();
        //Override the data on the client since it is already generated on the server
        properties.LoadItemData(throwableID);
        properties.SetValues(_speed, _size, _angularVelocity, _rigidbodyForce, _damage);
        properties.InitThrowable(owner);
        instanceThrowObject.GetComponent<ThrowableMovementScript>().InitThrowable(speedFactor);//Init the throwable with taking account the charging
    }
    #endregion
}