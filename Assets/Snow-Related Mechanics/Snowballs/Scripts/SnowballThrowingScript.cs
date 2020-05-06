using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Snowball throwing for player
public class SnowballThrowingScript : NetworkedBehaviour
{
    private PlayerUIManagerScript UIManager;//Handles UI for us
    public GameObject Snowball;//The snowball prefab that we are going to throw
    public Transform ThrowPoint;//The point where the snowball is throwed
    private GameObject InstanceSnowball;//The instance of the Snowball var
    private float ChargePercent = 0;//How much did we charge the snowball ?
    public float ChargeIncrement;//How fast does the charge rises ?
    public float ChargeTimeThreshold;//How much time before starting the charging of the snowball
    private float ChargeTime;//Variable taking track of the time the user held the mouse button
    public float ChargeLerpSpeed = 1;//The speed of how fast the charge percent should change
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
            ChargeTime += Time.deltaTime;//Add one second based on the delay between each frame
            if (ChargeTime > ChargeTimeThreshold)//If we held the button long enough, then start charging
            {
                ChargePercent += ChargeIncrement * Time.deltaTime; //Charge the throw
                ChargePercent = Mathf.Clamp(ChargePercent, 1f, 2f);//Clamp the value so we stay between a 0 - 1 range
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            ThrowSnowball();//Throw snowball
            ChargeTime = 0;//Reset the time since we threw the snowball            
        }
        if (!isHolding)
        {
            ChargePercent = Mathf.Lerp(ChargePercent, 1.0f, ChargeLerpSpeed * Time.deltaTime);//Make the charge percent go smoothly back to 1.0 since we arent charging the snowball anymore and 1.0 is the base value for throwing
            if (ChargePercent < 1.01f) ChargePercent = 1.0f;//ChargePercent is close enough to 1, so make it 1.0
        }
        UIManager.UpdatePlayerCharge(ChargePercent);
    }
    public void ThrowSnowball()//Throw snowball method (Client side only)
    {
        /*
        InstanceSnowball = Instantiate(Snowball, ThrowPoint.position, ThrowPoint.rotation);//Throw snowball and set that spawned snoball as our variable so we can call the InitSnowball method
        InstanceSnowball.GetComponent<SnowballMovementScript>().InitSnowball(ChargePercent, this);//Init the snowball with taking account the charging
        */
        InvokeServerRpc(ThrowSnowballServer, ThrowPoint.position, ThrowPoint.rotation, ChargePercent);
    }
    [ServerRPC]
    private void ThrowSnowballServer(Vector3 pos, Quaternion rot, float chargePercent) //Throw snowball method (On the server)
    {
        InstanceSnowball = Instantiate(Snowball, pos, rot);//Throw snowball and set that spawned snoball as our variable so we can call the InitSnowball method
        InstanceSnowball.GetComponent<SnowballMovementScript>().InitSnowball(chargePercent, this);//Init the snowball with taking account the charging

        InstanceSnowball.GetComponent<NetworkedObject>().Spawn();
    }
    //Debug
    public float lastSnowballDamage;
    public Vector3 lastSnowballVelocity;
    public string lastSnowballHitObject;
    private void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            float space = 15;
            float offset = space * 26;
            GUI.Box(new Rect(0, offset, 250, space * 14), "");
            GUI.Label(new Rect(0, offset, 500, 100), "SnowballThrowingScript : ");
            GUI.Label(new Rect(10, offset + space * 1, 500, 100), "Charge : ");
            GUI.Label(new Rect(30, offset + space * 2, 500, 100), "Charge Percent :" + ChargePercent);
            GUI.Label(new Rect(30, offset + space * 3, 500, 100), "Charge Time :" + ChargeTime);
            GUI.Label(new Rect(30, offset + space * 4, 500, 100), "Is Charging/Holding :" + isHolding);
            GUI.Label(new Rect(10, offset + space * 5, 500, 100), "Last Snowball : ");
            GUI.Label(new Rect(30, offset + space * 6, 500, 100), "LastSnowball Damage :" + lastSnowballDamage);
            GUI.Label(new Rect(30, offset + space * 7, 500, 100), "LastSnowball Velocity :");
            GUI.Label(new Rect(60, offset + space * 8, 500, 100), "X : " + lastSnowballVelocity.x.ToString("F2"));
            GUI.Label(new Rect(60, offset + space * 9, 500, 100), "Y : " + lastSnowballVelocity.y.ToString("F2"));
            GUI.Label(new Rect(60, offset + space * 10, 500, 100), "Z : " + lastSnowballVelocity.z.ToString("F2"));
            GUI.Label(new Rect(30, offset + space * 11, 500, 100), "LastSnowball HitObject :" + lastSnowballHitObject);
        }
    }
}