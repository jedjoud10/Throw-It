using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Shows bot stat when the crosshair hovers over a bot
public class BotDebugger : NetworkedBehaviour
{
    //UI
    public Slider botHealth;
    public Text botName;
    public Transform _camera;//the camera of the player
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
        if (Physics.Raycast(_camera.position + _camera.forward, _camera.forward * 10, out hit))
        {
            BotScript bot = hit.collider.gameObject.GetComponent<BotScript>();
            if (bot != null) 
            {
                //Update the UI
                botHealth.value = bot.healthScript.healthPercentage.Value;
                botName.text = "Name" + bot.gameObject.name;
            }
        }
    }
}
