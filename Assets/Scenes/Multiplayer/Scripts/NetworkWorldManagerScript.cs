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
    public Transform PlayerSpawnPoint;//Position where the players will spawn
    public GameObject PlayerPrefab;//Prefab of the player
    private NetworkingManager singleton;
    private bool singleplayer;//Is the game in singleplayer ?
    private string currentScene;//The current scene that we are in
    // Start is called before the first frame update
    void Start()
    {
        singleton = NetworkingManager.Singleton;
        singleton.OnClientDisconnectCallback += OnClientDisconnect;//When a client disconnects callback
        singleplayer = !singleton.IsHost && !singleton.IsClient;
        currentScene = SceneManager.GetActiveScene().name;
        //If the player isnt a server and isnt a client
        PlayerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform;
        if (singleplayer)
        {
            if(currentScene != "MainMenuMap")
            {
                singleton.StartHost(PlayerSpawnPoint.position);//Spawn host in singleplayer/multiplayer
            }
        }       
    }
    #region Scene management
    //Start the host
    public void HostMultiplayerLobby() { SceneManager.LoadScene("MultiplayerLobbyMap", LoadSceneMode.Single); }
    //Return to the MainMenu (Tell all clients to return to main menu first, then wait to fully disconnect)
    public void ReturnMainMenu() 
    {
        if (!IsServer) return;
        Debug.Log("Clients connected : " + singleton.ConnectedClientsList.Count);
        if(singleton.ConnectedClientsList.Count == 1) //Run this code when the host is the only client in the session
        {
            singleton.StopHost();
            SceneManager.LoadScene("MainMenuMap", LoadSceneMode.Single);
        }
        InvokeClientRpcOnEveryoneExcept(ReturnMainMenuClient, OwnerClientId);
    }
    [ClientRPC]
    //Make all clients return to main menu
    private void ReturnMainMenuClient() 
    {
        singleton.StopClient();
        SceneManager.LoadScene("MainMenuMap", LoadSceneMode.Single);
    }
    #endregion

    #region Callbacks
    //When a client disconnects (ran on server and on local client machine)
    private void OnClientDisconnect(ulong clientID) 
    {
        if (singleton.ConnectedClientsList.Count == 1 && IsHost) //Return to the main menu map only when the host is the only client in the session
        {
            singleton.StopHost();
            SceneManager.LoadScene("MainMenuMap", LoadSceneMode.Single);
        }
    }
    //Move player to correct position
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
