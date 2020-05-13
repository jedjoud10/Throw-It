using MLAPI;
using MLAPI.Messaging;
using MLAPI.Serialization;
using MLAPI.Spawning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Snowball throwing for player
public class SnowballThrowingScript : NetworkedBehaviour
{
    private PlayerUIManagerScript UIManager;//Handles UI for us
    public GameObject snowball;//The snowball prefab that we are going to throw
    public Transform throwPoint;//The point where the snowball is throwed
    private GameObject instanceSnowball;//The instance of the Snowball var
    private float chargePercent = 0;//How much did we charge the snowball ?
    public float chargeIncrement;//How fast does the charge rises ?
    public float chargeTimeThreshold;//How much time before starting the charging of the snowball
    private float chargeTime;//Variable taking track of the time the user held the mouse button
    public float chargeLerpSpeed = 1;//The speed of how fast the charge percent should change
    private bool isHolding;//If we are holding the left mouse button
    // Start is called before the first frame update
    void Start()
    {
        UIManager = GetComponent<PlayerUIManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer) return;
        if (Input.GetMouseButton(0))//Dedect if we are holding the left mouse button
        {
            isHolding = true;
            chargeTime += Time.deltaTime;//Add one second based on the delay between each frame
            if (chargeTime > chargeTimeThreshold)//If we held the button long enough, then start charging
            {
                chargePercent += chargeIncrement * Time.deltaTime; //Charge the throw
                chargePercent = Mathf.Clamp(chargePercent, 1f, 2f);//Clamp the value so we stay between a 0 - 1 range
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            ThrowSnowball();//Throw snowball
            chargeTime = 0;//Reset the time since we threw the snowball            
        }
        if (!isHolding)
        {
            chargePercent = Mathf.Lerp(chargePercent, 1.0f, chargeLerpSpeed * Time.deltaTime);//Make the charge percent go smoothly back to 1.0 since we arent charging the snowball anymore and 1.0 is the base value for throwing
            if (chargePercent < 1.01f) chargePercent = 1.0f;//ChargePercent is close enough to 1, so make it 1.0
        }
        UIManager.UpdatePlayerCharge(chargePercent);
    }
    public void ThrowSnowball()//Throw snowball method (Client side only)
    {
        instanceSnowball = Instantiate(snowball, throwPoint.position, throwPoint.rotation);//Throw snowball and set that spawned snoball as our variable so we can call the InitSnowball method
        instanceSnowball.GetComponent<SnowballMovementScript>().InitSnowball(chargePercent, this, true);//Init the snowball with taking account the charging
        SnowballPropertiesScript properties = instanceSnowball.GetComponent<SnowballPropertiesScript>();
        InvokeServerRpc(ThrowSnowballServer, chargePercent, throwPoint.position, throwPoint.rotation, OwnerClientId, properties.speed, properties.size, properties.angularVelocity, properties.rigidbodyForce, properties.damage);
    }
    [ServerRPC]
    //Spawns the snowball on the server
    private void ThrowSnowballServer(float chargePercent, Vector3 pos, Quaternion rot, ulong clientID, float _speed, float _size, Vector3 _angularVelocity, float _rigidbodyForce, int _damage)//Throw snowball method (On the server)
    {
        InvokeClientRpcOnEveryoneExcept(ThrowSnowballClient, clientID, chargePercent, pos, rot, _speed, _size, _angularVelocity, _rigidbodyForce, _damage);
    }
    [ClientRPC]
    //Spawns the snowball on all the clients except the owner
    private void ThrowSnowballClient(float chargePercent, Vector3 pos, Quaternion rot, float _speed, float _size, Vector3 _angularVelocity, float _rigidbodyForce, int _damage) 
    {
        instanceSnowball = Instantiate(snowball, pos, rot);//Throw snowball and set that spawned snoball as our variable so we can call the InitSnowball method
        instanceSnowball.GetComponent<SnowballPropertiesScript>().SetValues(_speed, _size, _angularVelocity, _rigidbodyForce, _damage);
        instanceSnowball.GetComponent<SnowballMovementScript>().InitSnowball(chargePercent, this, false);//Init the snowball with taking account the charging
    }
}