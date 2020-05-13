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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Damage the player. Remove health out of player (Only executed on server)
    public void DamagePlayer(int damage) 
    {
        if (!IsServer) return;
        health.Value -= damage;//Apply damage to health            
        if(health.Value < 0) 
        {
            wm.RespawnPlayer(GetComponent<PlayerControllerScript>(), this);
        }
        InvokeClientRpcOnClient(UpdateHealthbarOnClient, OwnerClientId, health.Value, maxHealth, UIManager);
        InvokeClientRpcOnEveryoneExcept(UpdateBillboardHealthbarOnClients, OwnerClientId, health.Value, maxHealth, UIManager);
    }
    //Executed on the client to update his UI health bar
    [ClientRPC]
    private void UpdateHealthbarOnClient(int currentHealth, int maxHealth, PlayerUIManagerScript _UIManager) 
    {
        _UIManager.UpdatePlayerHealth(currentHealth, maxHealth);
    }
    //Executed on the clients to update the player health billboard
    [ClientRPC]
    private void UpdateBillboardHealthbarOnClients(int currentHealth, int maxHealth, PlayerUIManagerScript _UIManager) 
    {
        _UIManager.UpdatePlayerHealthBillboard(currentHealth, maxHealth);
    }
    //Reset the player health
    public void SetupPlayerHealth() 
    {
        health.Value = maxHealth;
        //Setup UI
        InvokeClientRpcOnClient(UpdateHealthbarOnClient, OwnerClientId, maxHealth, maxHealth, UIManager);
        InvokeClientRpcOnEveryoneExcept(UpdateBillboardHealthbarOnClients, OwnerClientId, maxHealth, maxHealth, UIManager);
    }
}
