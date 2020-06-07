using MLAPI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//Handles communications between scripts and UI of the player
public class PlayerUIManagerScript : NetworkedBehaviour
{
    public GameObject UICanvas;//The whole UI
    [Header("Health")]
    public Slider healthBar;//The health bar
    public Animation healthBarAnimation;//The animation "controller"
    [Header("Snowball Charging")]
    public RawImage chargeBar;//The charge bar
    [Header("Billboard UIs")]
    public GameObject billboardCanvas;//The canvas that handles all the billboard UIs
    public Slider billboardHealth;
    public TMP_Text billboardNickname;
    [Header("System Chat")]
    public TMP_Text systemChatText;
    public RectTransform systemChatTargetPos;
    public RectTransform systemChat;
    public RawImage systemChatIcon;
    public Animation systemChatPanelAnimation;
    public float systemChatSmoothness = 5;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsLocalPlayer) { HidePlayerUI(); }
        else 
        { 
            HideBillboardUI();
            FindObjectOfType<NetworkWorldManagerScript>().playerUIManager = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Smooth out the system chat in code because unity animations cant do *prefect* exponential curves (maybe they do and i am very bumbum)
        systemChat.anchoredPosition = Vector2.Lerp(systemChat.anchoredPosition, systemChatTargetPos.anchoredPosition, systemChatSmoothness * Time.deltaTime);
    }
    //Update the player health UI
    //Types of player health updates are:
    //0: Damage player
    //1: Heal player
    //2: Just update, without animations
    public void UpdatePlayerHealth(int health, int maxHealth, int type) 
    {
        healthBar.value = (float)health / (float)maxHealth;//Update health bar
        if (type == 0) healthBarAnimation.Play("HealthBarDamage");
        if (type == 1) healthBarAnimation.Play("HealthBarHeal");
    }
    //Updates the player snowball charge UI
    public void UpdatePlayerCharge(float chargePercent) 
    {
        chargeBar.material.SetFloat("_Percent", chargePercent - 1);
    }
    //Hides the whole UI
    public void HidePlayerUI() { UICanvas.SetActive(false); }
    //Hides the whole billboard UI
    public void HideBillboardUI() { billboardCanvas.SetActive(false); }
    //Update the player health billboard (not networked)
    public void UpdatePlayerHealthBillboard(int health, int maxHealth) { billboardHealth.value = (float)health / (float)maxHealth; }
    //Update the player nickname billboard (not networked)
    public void UpdatePlayerNicknameBillboard(string nickname) { billboardNickname.text = nickname; }
    //Update the system chat on this player
    public void UpdateSystemChat(string newChat, string textureName) 
    { 
        systemChatText.text = newChat;
        systemChatIcon.texture = TexturesManager.LoadTexture(textureName);
        systemChatPanelAnimation.Play();
    }
}
