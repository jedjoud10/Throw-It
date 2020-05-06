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
    public Text HealthText;//Text showing current health
    public Slider HealthBar;//The health bar
    public Animation HealthBackAnimation;//The animation "controller"
    [Header("Snowball Charging")]
    public Text ChargeText;//UI Text
    public Slider ChargeBar;//The charge bar
    // Start is called before the first frame update
    void Start()
    {
        if (!IsLocalPlayer) { HidePlayerUI(); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Update the player health UI
    public void UpdatePlayerHealth(int health, int maxHealth) 
    {
        HealthText.text = "Health : " + health;//Update health text
        HealthBar.value = (float)health / (float)maxHealth;//Update health bar
        HealthBackAnimation.Play();
    }
    //Updates the player snowball charge UI
    public void UpdatePlayerCharge(float ChargePercent) 
    {        
        ChargeText.text = "Charge : " + (ChargePercent * 100.0f).ToString("F2");
        ChargeBar.value = ChargePercent - 1;
    }
    //Hides the whole UI
    public void HidePlayerUI() { UICanvas.SetActive(false); }
}
