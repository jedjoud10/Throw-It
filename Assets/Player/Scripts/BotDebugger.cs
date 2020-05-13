using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Shows bot stat when the crosshair hovers over a bot
public class BotDebugger : NetworkedBehaviour
{
    //UI
    public Slider BotHealth;
    public Text BotName;
    public Transform Camera;//the camera of the player
    private RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer) return;//Only debug for the local player
        //Check if there is a bot infront of the player
        if (Physics.Raycast(Camera.position + Camera.forward, Camera.forward * 10, out hit))
        {
            BotScript bot = hit.collider.gameObject.GetComponent<BotScript>();
            if (bot != null) 
            {
                //Update the UI
                BotHealth.value = bot.healthScript.HealthPercentage.Value;
                BotName.text = "Name" + bot.gameObject.name;
            }
        }
    }
}
