using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI;
using MLAPI.Messaging;
using System;
using MLAPI.NetworkedVar.Collections;
using MLAPI.NetworkedVar;
//Well, a world manager, but for multiplayer. Yea, pretty epic
public class NetworkWorldManagerScript : NetworkedBehaviour
{
    public Transform playerSpawnPoint;//Position where the players will spawn
    public PlayerUIManagerScript playerUIManager;//Current local player UIManager
    public PlayerConfigScript playerConfigScript;//Current local player config
    private NetworkingManager singleton;
    private bool singleplayer;//Is the game in singleplayer ?
    private string currentScene;//The current scene that we are in
    private static NetworkedDictionary<ulong, string> players = new NetworkedDictionary<ulong, string>(new Dictionary<ulong, string>());

    // Start is called before the first frame update
    void Start()
    {   
        singleton = NetworkingManager.Singleton;

        currentScene = SceneManager.GetActiveScene().name;
        //If the player isnt a server and isnt a client (The only client (Basically in singleplayer))
        singleplayer = !singleton.IsHost && !singleton.IsClient;
        if (currentScene != "MainMenuMap")
        {
            if (singleplayer)
            {
                //If we arent in the main menu, then start the game as a host
                //MLAPI cant send any data if we are the only player, so we are going to be in singleplayer then
                singleton.StartHost(playerSpawnPoint.position);
                players = new NetworkedDictionary<ulong, string>(new Dictionary<ulong, string>());
                Debug.Log("Server has started");
                ChatLogger.StartLogger();
                ChatLogger.LogNewMessage("Server has started");                
            }
            else
            {
                
            }
        }
        singleton.OnClientDisconnectCallback += PlayerDisconnect;
    }

    #region Callbacks
    #endregion
    #region  Player
    //Respawns a certain player
    public void RespawnPlayer(PlayerControllerScript playerController, PlayerHealthScript playerHealth) 
    {
        if (!IsServer) return;
        //Reset position and velocity
        playerController.SetPlayerPosition(playerSpawnPoint.position);
        //Reset player health
        playerHealth.SetupPlayerHealth();

        InvokeClientRpcOnEveryone(RespawnPlayerOnClients, playerController, playerHealth);
    }
    [ClientRPC]
    //Respawns a certain player on all clients
    private void RespawnPlayerOnClients(PlayerControllerScript playerController, PlayerHealthScript playerHealth) 
    {
        playerController.SetPlayerPosition(playerSpawnPoint.position);
    }
    #endregion
    #region System Chat
    //Updates the system chat on all players (executed on server)
    [ServerRPC]
    public void UpdateSystemChat(string newChat, string newTextureName)
    {
        InvokeClientRpcOnEveryone(UpdateSystemChatOnClient, newChat, newTextureName);
        ChatLogger.LogNewMessage("SYSTEM CHAT: " + newChat);        
    }
    //Update the system chat on the local client
    [ClientRPC]
    private void UpdateSystemChatOnClient(string newChat, string newTextureName) 
    {
        playerUIManager.UpdateSystemChat(newChat, newTextureName);
    }
    #endregion
    //When a player wants to join the game (exectued only on server)
    public void PlayerJoin(string nickname, ulong clientID) 
    {
        ChatLogger.LogNewMessage("Player joining... ID: " + clientID + " User: " + nickname);
        UpdateSystemChat(RandomMessages.Player_Joingame(nickname), "systemchat_playerjoin.png");
        players.Add(clientID, nickname);
    }
    //When a player quits (exectued only on server)
    private void PlayerDisconnect(ulong clientID) 
    {
        //if (clientID == OwnerClientId && IsHost) return;
        if (IsHost)
        {
            NetworkWorldManagerScript instance = FindObjectOfType<NetworkWorldManagerScript>();
            if (!players.ContainsKey(clientID)) return;
            //Server code
            string nickname = players[clientID];           
            ChatLogger.LogNewMessage("Player leaving... ID: " + clientID + " User: " + nickname);
            instance.UpdateSystemChat(RandomMessages.Player_Leftgame(nickname), "systemchat_playerleaving.png");
            players.Remove(clientID);//This client disconnected, so we can remove them from the players list
        }
        else
        {
            //Client code
            SceneManager.LoadScene("MainMenuMap", LoadSceneMode.Single);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(InputManager.GetKeyPress("PauseMenu")) 
        {            
            if (!IsHost) //If we are a normal client
            {
                singleton.StopClient();
                SceneManager.LoadScene("MainMenuMap", LoadSceneMode.Single);
            }
            else
            {
                singleton.StopHost();
                SceneManager.LoadScene("MainMenuMap", LoadSceneMode.Single);
            }
        }        
    }
    //When the user closes the game
    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name != "MainMenuMap")//If we arent in the main menu / In single player
        {
            if (!IsHost) //If we are a normal client
            {
                singleton.StopClient();
            }
            else
            {
                singleton.StopHost();
                SceneManager.LoadScene("MainMenuMap", LoadSceneMode.Single);
            }
        }
    }
}
