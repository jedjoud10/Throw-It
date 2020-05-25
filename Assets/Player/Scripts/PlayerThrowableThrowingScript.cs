using UnityEngine;
using System.Collections;
using MLAPI;

public class PlayerThrowableThrowingScript : NetworkedBehaviour
{
    private PlayerUIManagerScript UIManager;//Handles UI for us
    private float charge = 1;
    const float chargeSpeed = 3f;//How fast the throwable charging is
    const float chargeTimeThreshold = 0.2f;//If the player holds the charging button more that this number (in seconds) it will start charging
    private float chargeTime;//The last time value that we updated when the player threw a throwable
    private bool canThrow;//can the player throw a throwable
    private ThrowableThrowingScript thrower;//The thrower that is going to throw. yes
    private PlayerConfigScript playerConfig;//The config holding the nickname for this player
    // Use this for initialization
    void Start()
    {
        if (IsLocalPlayer) 
        {
            UIManager = GetComponent<PlayerUIManagerScript>();
            thrower = GetComponent<ThrowableThrowingScript>();
            playerConfig = GetComponent<PlayerConfigScript>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer) return;
        UIManager.UpdatePlayerCharge(charge);
        if (Input.GetAxis("ChargeThrowable") > 0)
        {
            chargeTime += Time.deltaTime;
            //Start charging if time is over the threshold
            if (chargeTime > chargeTimeThreshold) charge = Mathf.Lerp(charge, 2, chargeSpeed * Time.deltaTime);//Slowly go to 2
            else charge = Mathf.Lerp(charge, 1, chargeSpeed * Time.deltaTime);//Slowly go back to 1

            canThrow = true;
        }
        else
        {
            charge = Mathf.Lerp(charge, 1, chargeSpeed * Time.deltaTime);//Slowly go back to 1
            if (canThrow)
            {
                //Reset charge timer and throw throwable
                thrower.Throw(charge, playerConfig.nickname.Value);
                chargeTime = 0;
            }
            canThrow = false;
        }
    }
}
