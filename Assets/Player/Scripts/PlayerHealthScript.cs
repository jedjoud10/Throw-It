using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar;
using MLAPI.Messaging;
//Controls health of player
public class PlayerHealthScript : NetworkedBehaviour
{
    public int MaxHealth;//Maximum health of player
    public NetworkedVarInt Health;//Current health
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
        Health.Value -= damage;//Apply damage to health            
        if(Health.Value < 0) 
        {
            wm.RespawnPlayer(GetComponent<PlayerControllerScript>(), this);
        }
        InvokeClientRpcOnClient(UpdateClientHealthBar, OwnerClientId, Health.Value, MaxHealth, UIManager);
    }
    //Executed on the client to update his UI health bar
    [ClientRPC]
    private void UpdateClientHealthBar(int currentHealth, int maxHealth, PlayerUIManagerScript _UIManager) 
    {
        _UIManager.UpdatePlayerHealth(currentHealth, maxHealth);
    }
    //Reset the player health
    public void SetupPlayerHealth() 
    {
        Health.Value = MaxHealth;
        InvokeClientRpcOnClient(UpdateClientHealthBar, OwnerClientId, MaxHealth, MaxHealth, UIManager);//Update UI
    }
}
