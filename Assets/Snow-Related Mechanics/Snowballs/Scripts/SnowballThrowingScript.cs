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
    public float ChargeLerpSpeed = 1;//The speed of how fast the charge percent should change
    public Text ChargeText;//UI Text
    public Slider ChargeBar;//The charge bar
    private bool isHolding;//If we are holding the left mouse button
    private bool releaseHold;//When we stop holding the left mouse button
    private PlayerControls playerControls;
    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.ChargeSnowball.performed += ctx => isHolding = ctx.ReadValueAsButton();
        playerControls.Player.ReleaseSnowball.performed += ctx => releaseHold = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isHolding)//Dedect if we are holding the left mouse button
        {            
            ChargeTime += Time.deltaTime;//Add one second based on the delay between each frame
            if (ChargeTime > ChargeTimeThreshold)//If we held the button long enough, then start charging
            {
                ChargePercent += ChargeIncrement * Time.deltaTime; //Charge the throw
                ChargePercent = Mathf.Clamp(ChargePercent, 1f, 2f);//Clamp the value so we stay between a 0 - 1 range
            }
        }
        else if (releaseHold)
        {
            ThrowSnowball();//Throw snowball
            ChargeTime = 0;//Reset the time since we threw the snowball            
        }
        if (!isHolding) 
        {
            ChargePercent = Mathf.Lerp(ChargePercent, 1.0f, ChargeLerpSpeed * Time.deltaTime);//Make the charge percent go smoothly back to 1.0 since we arent charging the snowball anymore and 1.0 is the base value for throwing
            if (ChargePercent < 1.01f) ChargePercent = 1.0f;//ChargePercent is close enough to 1, so make it 1.0
        }
        //Set UI
        ChargeText.text = "Charge : " + (ChargePercent * 100.0f).ToString("F2");
        ChargeBar.value = ChargePercent - 1;
        releaseHold = false;//Reset release button
    }
    public void ThrowSnowball()//Throw snowball method
    {
        InstanceSnowball = Instantiate(Snowball, ThrowPoint.position, ThrowPoint.rotation);//Throw snowball and set that spawned snoball as our variable so we can call the InitSnowball method
        InstanceSnowball.GetComponent<SnowballMovementScript>().InitSnowball(ChargePercent, this);//Init the snowball with taking account the charging
    }
    #region Enable/Disable
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    #endregion 
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
