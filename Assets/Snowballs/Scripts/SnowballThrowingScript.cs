using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Snowball throwing for player
public class SnowballThrowingScript : MonoBehaviour
{
    public GameObject Snowball;//The snowball prefab that we are going to throw
    public Transform ThrowPoint;//The point where the snowball is throwed
    private GameObject InstanceSnowball;//The instance of the Snowball var
    private float ChargePercent;//How much did we charge the snowball ?
    public float ChargeIncement;//How fast does the charge rises ?
    public float ChargeTimeThreshold;//How much time before starting the charging of the snowball
    private float ChargeTime;//Variable taking track of the time the user held the mouse button

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))//Dedect if we holding the left mouse button
        {
            ChargeTime += Time.deltaTime;//Add one second based on the delay between each frame
            if (ChargeTime > ChargeTimeThreshold)//If we held the button long enough, then start charging
            {
                ChargePercent = ChargePercent + ChargeIncement * Time.deltaTime; //Charge the throw
                ChargePercent = Mathf.Clamp(ChargePercent, 0, 1);//Clamp the value so we stay between a 0 - 1 range
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ThrowSnowball();//Throw snowball
            ChargePercent = 0;//Reset the charge since we threw the snowball
            ChargeTime = 0;//Reset the time since we threw the snowball
        }
    }
    public void ThrowSnowball()//Throw snowball method
    {
        InstanceSnowball = Instantiate(Snowball, ThrowPoint.position, ThrowPoint.rotation);//Throw snowball and set that spawned snoball as our variable so we can call the InitSnowball method
        InstanceSnowball.GetComponent<SnowballMovementScript>().InitSnowball(ChargePercent);//Init the snowball with taking account the charging
    }
}
