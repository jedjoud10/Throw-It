using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI;
using MLAPI.Messaging;
using System;
//Well, a world manager, but for multiplayer. Yea, pretty epic
public class NetworkWorldManagerScript : NetworkedBehaviour
{
    private Transform PlayerSpawnPoint;//Position where the players will spawn
    public GameObject PlayerPrefab;//Prefab of the player
    private NetworkingManager singleton;
    private WorldManager worldManager;//The world manager
    private bool singleplayer;//Is the game in singleplayer ?
    // Start is called before the first frame update
    void Start()
    {
        worldManager = FindObjectOfType<WorldManager>();
        singleton = NetworkingManager.Singleton;
        singleton.OnClientDisconnectCallback += OnClientDisconnect;//When a client disconnects callback
        singleton.ConnectionApprovalCallback += OnApprovalCheck;//When a client tries to approve and connect
        singleplayer = !singleton.IsHost && !singleton.IsClient;
        //If the player isnt a server and isnt a client
        if (singleplayer)
        {
            if (worldManager.IsScene("MultiplayerLobbyMap")) //Start hosting if we are in the multiplayer lobby
            {
                PlayerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform;
                singleton.StartHost(PlayerSpawnPoint.position);//Spawn host in soon to be multiplayer session                
            }
            else if(!worldManager.IsScene("MainMenuMap"))
            {
                PlayerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform;
                singleton.StartHost(PlayerSpawnPoint.position);//Spawn host in singleplayer
            }
        }
        else
        {
            PlayerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform;
        }
    }
    #region Scene management
    //Start the host
    public void HostMultiplayerLobby() { worldManager.ChangeScene("MultiplayerLobbyMap"); }
    //Return to the MainMenu (Tell all clients to return to main menu first, then wait to fully disconnect)
    public void ReturnMainMenu() 
    {
        if (!IsServer) return;
        Debug.Log("Clients connected : " + singleton.ConnectedClientsList.Count);
        if(singleton.ConnectedClientsList.Count == 1) //Run this code when the host is the only client in the session
        {
            singleton.StopHost();
            worldManager.ChangeScene("MainMenuMap");
        }
        InvokeClientRpcOnEveryoneExcept(ReturnMainMenuClient, OwnerClientId);
    }
    [ClientRPC]
    //Make all clients return to main menu
    private void ReturnMainMenuClient() 
    {
        singleton.StopClient();
        worldManager.ChangeScene("MainMenuMap");
    }

    #endregion
    #region Callbacks
    //When a client disconnects
    private void OnClientDisconnect(ulong clientID) 
    {
        if (singleton.ConnectedClientsList.Count == 1 && IsHost) //Return to the main menu map only when the host is the only client in the session
        {
            singleton.StopHost();
            worldManager.ChangeScene("MainMenuMap");
        }
    }
    //When a client wants to connect
    private void OnApprovalCheck(byte[] connectionData, ulong clientId, MLAPI.NetworkingManager.ConnectionApprovedDelegate callback) 
    {
        //If approve is true, the connection gets added. If it's false. The client gets disconnected
        callback(true, null, true, PlayerSpawnPoint.position, Quaternion.identity);
    }
    #endregion
    //Respawns a certain player
    public void RespawnPlayer(PlayerControllerScript playerController, PlayerHealthScript playerHealth) 
    {
        if (!IsServer) return;
        //Reset position and velocity
        playerController.ResetPositionAndVelocity(PlayerSpawnPoint.position);
        //Reset player health
        playerHealth.SetupPlayerHealth();

        InvokeClientRpcOnEveryone(RespawnPlayerOnClients, playerController, playerHealth);
    }
    [ClientRPC]
    //Respawns a certain player on all clients
    private void RespawnPlayerOnClients(PlayerControllerScript playerController, PlayerHealthScript playerHealth) 
    {
        playerController.ResetPositionAndVelocity(PlayerSpawnPoint.position);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O)) 
        {
            if (IsHost) 
            {            
                ReturnMainMenu();
                return;
            }
            if (IsClient) 
            {
                ReturnMainMenuClient();
                return;
            }
        }        
    }
}
