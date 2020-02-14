using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Snowball throwing for player
public class SnowballThrowingScript : MonoBehaviour
{
    public GameObject Snowball;//The snowball prefab that we are going to throw
    public Transform ThrowPoint;//The point where the snowball is throwed
    private GameObject InstanceSnowball;//The instance of the Snowball var
    private float ChargePercent = 0;//How much did we charge the snowball ?
    public float ChargeIncrement;//How fast does the charge rises ?
    public float ChargeTimeThreshold;//How much time before starting the charging of the snowball
    private float ChargeTime;//Variable taking track of the time the user held the mouse button
    public Text ChargeText;//UI Text
    public Slider ChargeBar;//The charge bar
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
                ChargePercent += ChargeIncrement * Time.deltaTime; //Charge the throw
                ChargePercent = Mathf.Clamp(ChargePercent, 1f, 2f);//Clamp the value so we stay between a 0 - 1 range
            }
            //Set UI
            ChargeText.text = "Charge : " + (ChargePercent * 100.0f).ToString("F2");
            ChargeBar.value = ChargePercent - 1;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (ChargeTime < ChargeTimeThreshold)
            {
                ChargePercent = 1;//Reset the charge since we are not charging
            }
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
