using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar;
using MLAPI.Messaging;
//Controls health of player
public class PlayerHealthScript : NetworkedBehaviour
{
    public int maxHealth;//Maximum health of player
    public NetworkedVarInt health;//Current health
    private PlayerUIManagerScript UIManager;//Handles UI for us
    private NetworkWorldManagerScript wm;//Our networked world manager

    // Start is called before the first frame update
    void Start()
    {
        wm = FindObjectOfType<NetworkWorldManagerScript>();
        UIManager = GetComponent<PlayerUIManagerScript>();
        if (IsServer)
        {
            //Setup health on server
            SetupPlayerHealth();
        }
        else
        {
            UIManager.UpdatePlayerHealthBillboard(health.Value, maxHealth);//Init the billboard of this player on other clients when they join the game late
        }
    }
    //Damage the player. Remove health out of player (Only executed on server)
    public void DamagePlayer(int damage, string reason, string deathMessage) 
    {
        if (!IsServer) return;
        health.Value -= damage;//Apply damage to health            
        if(health.Value < 0) 
        {
            wm.RespawnPlayer(GetComponent<PlayerControllerScript>(), this);
            //We can do this since we are running as server
            FindObjectOfType<NetworkWorldManagerScript>().UpdateSystemChat(deathMessage);
        }
        InvokeClientRpcOnClient(UpdateHealthbarOnClient, OwnerClientId, health.Value, maxHealth, 0, UIManager);
        InvokeClientRpcOnEveryoneExcept(UpdateBillboardHealthbarOnClients, OwnerClientId, health.Value, maxHealth, UIManager);
    }
    //Heal the player. Add health to player (Only executed on server)
    public bool HealPlayer(int healthRegeneration) 
    {
        if (!IsServer) return false;
        if(health.Value >= maxHealth) 
        {
            return false;//The player is already at full health
        }
        health.Value += healthRegeneration;//Apply healing to health  
        health.Value = Mathf.Min(health.Value, maxHealth);//Dont let the health exceed the max health
        return true;
    }
    //Executed on the client to update his UI health bar
    [ClientRPC]
    private void UpdateHealthbarOnClient(int currentHealth, int _maxHealth, int type, PlayerUIManagerScript _UIManager) 
    {
        _UIManager.UpdatePlayerHealth(currentHealth, _maxHealth, type);
    }
    //Executed on the clients to update the player health billboard
    [ClientRPC]
    private void UpdateBillboardHealthbarOnClients(int currentHealth, int _maxHealth, PlayerUIManagerScript _UIManager) 
    {
        _UIManager.UpdatePlayerHealthBillboard(currentHealth, _maxHealth);
    }
    //Reset the player health
    public void SetupPlayerHealth() 
    {
        health.Value = maxHealth;
        //Setup UI
        InvokeClientRpcOnClient(UpdateHealthbarOnClient, OwnerClientId, maxHealth, maxHealth, 2, UIManager);
        InvokeClientRpcOnEveryoneExcept(UpdateBillboardHealthbarOnClients, OwnerClientId, maxHealth, maxHealth, UIManager);
    }
}
