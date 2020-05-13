using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Hadnles communications between scripts and UI of the player
public class PlayerUIManagerScript : NetworkedBehaviour
{
    public GameObject UICanvas;//The whole UI
    [Header("Health")]
    public Text healthText;//Text showing current health
    public Slider healthBar;//The health bar
    public Animation healthBackAnimation;//The animation "controller"
    [Header("Snowball Charging")]
    public Text chargeText;//UI Text
    public Slider chargeBar;//The charge bar
    [Header("Billboard UIs")]
    public GameObject billboardCanvas;//The canvas that handles all the billboard UIs
    private Transform _camera;//The current camera that is rendering the scene
    public Slider billboardHealth;
    public Text billboardNickname;
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main.transform;
        if (!IsLocalPlayer) { HidePlayerUI(); }
        else { HideBillboardUI(); }
    }

    // Update is called once per frame
    void Update()
    {
        //Make the billboard have the same forward direction as the camera
        //TODO: Turn this into a main billboard manager to save on performance
        billboardCanvas.transform.forward = _camera.forward;
    }
    //Update the player health UI
    public void UpdatePlayerHealth(int health, int maxHealth) 
    {
        healthText.text = "Health : " + health;//Update health text
        healthBar.value = (float)health / (float)maxHealth;//Update health bar
        healthBackAnimation.Play();
    }
    //Updates the player snowball charge UI
    public void UpdatePlayerCharge(float ChargePercent) 
    {        
        chargeText.text = "Charge : " + (ChargePercent * 100.0f).ToString("F2");
        chargeBar.value = ChargePercent - 1;
    }
    //Hides the whole UI
    public void HidePlayerUI() { UICanvas.SetActive(false); }
    //Hides the whole billboard UI
    public void HideBillboardUI() { billboardCanvas.SetActive(false); }
    //Update the player health billboard (not networked)
    public void UpdatePlayerHealthBillboard(int health, int maxHealth) { billboardHealth.value = (float)health / (float)maxHealth; }
    //Update the player nickname billboard (not networked)
    public void UpdatePlayerNicknameBillboard(string nickname) { billboardNickname.text = nickname; }
}
