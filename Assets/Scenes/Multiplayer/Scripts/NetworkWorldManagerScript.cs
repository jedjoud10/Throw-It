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
    public Transform playerSpawnPoint;//Position where the players will spawn
    public PlayerUIManagerScript playerUIManager;//Current local player UIManager
    private NetworkingManager singleton;
    private bool singleplayer;//Is the game in singleplayer ?
    private string currentScene;//The current scene that we are in

    // Start is called before the first frame update
    void Start()
    {   
        singleton = NetworkingManager.Singleton;
        singleton.OnClientDisconnectCallback += OnClientDisconnect;//When a client disconnects callback
        singleton.OnServerStarted += OnServerStarted;//When the server starts callback
        singleplayer = !singleton.IsHost && !singleton.IsClient;
        //If the player isnt a server and isnt a client (The only client (Basically in singleplayer))
        currentScene = SceneManager.GetActiveScene().name;
        if (singleplayer)
        {
            if(currentScene != "MainMenuMap")
            {
                //If we arent in the main menu, then start the game as a host
                //MLAPI cant send any data if we are the only player, so we are going to be in singleplayer then
                singleton.StartHost(playerSpawnPoint.position);
            }
        }       
    }
    #region Scene management
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
        if(IsClient && !IsHost) SceneManager.LoadScene("MainMenuMap", LoadSceneMode.Single);//Return to main menu when a client disconnects
    }
    //When the server starts
    private void OnServerStarted() 
    {
        
    }
    //Move player to correct position
    #endregion

    #region  Player
    //Respawns a certain player
    public void RespawnPlayer(PlayerControllerScript playerController, PlayerHealthScript playerHealth) 
    {
        if (!IsServer) return;
        //Reset position and velocity
        playerController.ResetPlayer();
        //Reset player health
        playerHealth.SetupPlayerHealth();

        InvokeClientRpcOnEveryone(RespawnPlayerOnClients, playerController, playerHealth);
    }
    [ClientRPC]
    //Respawns a certain player on all clients
    private void RespawnPlayerOnClients(PlayerControllerScript playerController, PlayerHealthScript playerHealth) 
    {
        playerController.ResetPlayer();
    }
    #endregion

    #region System Chat
    //Updates the system chat on all players (executed on server)
    public void UpdateSystemChat(string newChat) 
    {
        InvokeClientRpcOnEveryone(UpdateSystemChatOnClient, newChat);
    }
    //Update the system chat on the local client
    [ClientRPC]
    private void UpdateSystemChatOnClient(string newChat) 
    {
        playerUIManager.UpdateSystemChat(newChat);
    }
    #endregion
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
    //When the user closes the game
    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name != "MainMenuMap")//If we arent in the main menu / In single player
        {
            if (IsHost) { ReturnMainMenu(); return; }
            singleton.StopClient();
        }
    }
}
